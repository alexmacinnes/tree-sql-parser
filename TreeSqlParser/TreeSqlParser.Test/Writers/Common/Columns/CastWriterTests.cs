using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class CastWriterTests
    {
        private readonly IReadOnlyDictionary<DbFamily, ISqlWriter> writers = new Dictionary<DbFamily, ISqlWriter>
        {
            { DbFamily.SqlServer, new CommonSqlServerSqlWriter() },
            { DbFamily.Oracle, new CommonOracleSqlWriter() },
            { DbFamily.MySql, new CommonMySqlSqlWriter() },
            { DbFamily.Sqlite, new CommonSqliteSqlWriter() }
        };

        private Column ParseColumn(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select " + sql).Child).Selects[0].Columns[0];

        private string Sql(Column c, DbFamily db) => writers[db].GenerateSql(c);

        private void AssertSql(Column c, DbFamily db, string expected) =>
            Assert.AreEqual(expected, Sql(c, db));

        [Test]
        public void CastAsNVarChar()
        {
            var c = ParseColumn("CAST(NULL AS nvarchar)");

            AssertSql(c, DbFamily.SqlServer, "CAST(NULL AS nvarchar)");
            AssertSql(c, DbFamily.Oracle, "CAST(NULL AS nvarchar2(255))");
            AssertSql(c, DbFamily.MySql, "CAST(NULL AS nchar)");
            AssertSql(c, DbFamily.Sqlite, "CAST(NULL AS nvarchar)");
        }

        [Test]
        public void CastAsVarChar()
        {
            var c = ParseColumn("CAST(NULL AS varchar)");

            AssertSql(c, DbFamily.SqlServer, "CAST(NULL AS varchar)");
            AssertSql(c, DbFamily.Oracle, "CAST(NULL AS varchar2(255))");
            AssertSql(c, DbFamily.MySql, "CAST(NULL AS char)");
            AssertSql(c, DbFamily.Sqlite, "CAST(NULL AS varchar)");
        }

        [Test]
        public void CastAsInt()
        {
            var c = ParseColumn("CAST(NULL AS int)");

            AssertSql(c, DbFamily.SqlServer, "CAST(NULL AS int)");
            AssertSql(c, DbFamily.Oracle, "CAST(NULL AS int)");
            AssertSql(c, DbFamily.MySql, "CAST(NULL AS signed)");
            AssertSql(c, DbFamily.Sqlite, "CAST(NULL AS int)");
        }

        [Test]
        public void CastAsReal()
        {
            var c = ParseColumn("CAST(NULL AS real)");

            AssertSql(c, DbFamily.SqlServer, "CAST(NULL AS real)");
            AssertSql(c, DbFamily.Oracle, "CAST(NULL AS number)");
            AssertSql(c, DbFamily.MySql, "CAST(NULL AS decimal)");
            AssertSql(c, DbFamily.Sqlite, "CAST(NULL AS real)");
        }

        [Test]
        public void CastAsTimestamp()
        {
            var c = ParseColumn("CAST(NULL AS timestamp)");

            AssertSql(c, DbFamily.SqlServer, "CAST(NULL AS timestamp)");
            AssertSql(c, DbFamily.Oracle, "CAST(NULL AS datetime)");
            AssertSql(c, DbFamily.MySql, "CAST(NULL AS datetime)");
            AssertSql(c, DbFamily.Sqlite, "DATETIME(NULL)");
        }
    }
}
