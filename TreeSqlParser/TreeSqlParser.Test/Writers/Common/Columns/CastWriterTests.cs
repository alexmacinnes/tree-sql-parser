using NUnit.Framework;
using System.Collections.Generic;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using TreeSqlParser.Writers.Common.MySql;
using TreeSqlParser.Writers.Common.Oracle;
using TreeSqlParser.Writers.Common.Sqlite;
using TreeSqlParser.Writers.Common.SqlServer;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class CastWriterTests
    {
        private Column ParseColumn(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select " + sql).Child).Selects[0].Columns[0];

        private void AssertSql(Column c, SqlWriterType db, string expected) =>
            Assert.AreEqual(expected, CommonMother.Sql(c, db));

        [Test]
        public void CastAsNVarChar()
        {
            var c = ParseColumn("CAST(NULL AS nvarchar)");

            AssertSql(c, SqlWriterType.SqlServer, "CAST(NULL AS nvarchar)");
            AssertSql(c, SqlWriterType.Oracle, "CAST(NULL AS nvarchar2(255))");
            AssertSql(c, SqlWriterType.MySql, "CAST(NULL AS nchar)");
            AssertSql(c, SqlWriterType.Sqlite, "CAST(NULL AS nvarchar)");
        }

        [Test]
        public void CastAsVarChar()
        {
            var c = ParseColumn("CAST(NULL AS varchar)");

            AssertSql(c, SqlWriterType.SqlServer, "CAST(NULL AS varchar)");
            AssertSql(c, SqlWriterType.Oracle, "CAST(NULL AS varchar2(255))");
            AssertSql(c, SqlWriterType.MySql, "CAST(NULL AS char)");
            AssertSql(c, SqlWriterType.Sqlite, "CAST(NULL AS varchar)");
        }

        [Test]
        public void CastAsInt()
        {
            var c = ParseColumn("CAST(NULL AS int)");

            AssertSql(c, SqlWriterType.SqlServer, "CAST(NULL AS int)");
            AssertSql(c, SqlWriterType.Oracle, "CAST(NULL AS int)");
            AssertSql(c, SqlWriterType.MySql, "CAST(NULL AS signed)");
            AssertSql(c, SqlWriterType.Sqlite, "CAST(NULL AS int)");
        }

        [Test]
        public void CastAsReal()
        {
            var c = ParseColumn("CAST(NULL AS real)");

            AssertSql(c, SqlWriterType.SqlServer, "CAST(NULL AS real)");
            AssertSql(c, SqlWriterType.Oracle, "CAST(NULL AS number)");
            AssertSql(c, SqlWriterType.MySql, "CAST(NULL AS decimal)");
            AssertSql(c, SqlWriterType.Sqlite, "CAST(NULL AS real)");
        }

        [Test]
        public void CastAsTimestamp()
        {
            var c = ParseColumn("CAST(NULL AS timestamp)");

            AssertSql(c, SqlWriterType.SqlServer, "CAST(NULL AS timestamp)");
            AssertSql(c, SqlWriterType.Oracle, "CAST(NULL AS datetime)");
            AssertSql(c, SqlWriterType.MySql, "CAST(NULL AS datetime)");
            AssertSql(c, SqlWriterType.Sqlite, "DATETIME(NULL)");
        }
    }
}
