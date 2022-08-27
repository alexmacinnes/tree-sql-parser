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
            { DbFamily.SqlServer, new CommonSqlServerWriter() },
            { DbFamily.Oracle, new CommonOracleSqlWriter() }
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
        }

        [Test]
        public void CastAsVarChar()
        {
            var c = ParseColumn("CAST(NULL AS varchar)");

            AssertSql(c, DbFamily.SqlServer, "CAST(NULL AS varchar)");
            AssertSql(c, DbFamily.Oracle, "CAST(NULL AS varchar2(255))");
        }

        [Test]
        public void CastAsInt()
        {
            var c = ParseColumn("CAST(NULL AS int)");

            AssertSql(c, DbFamily.SqlServer, "CAST(NULL AS int)");
            AssertSql(c, DbFamily.Oracle, "CAST(NULL AS int)");
        }

        [Test]
        public void CastAsReal()
        {
            var c = ParseColumn("CAST(NULL AS real)");

            AssertSql(c, DbFamily.SqlServer, "CAST(NULL AS real)");
            AssertSql(c, DbFamily.Oracle, "CAST(NULL AS number)");
        }

        [Test]
        public void CastAsTimestamp()
        {
            var c = ParseColumn("CAST(NULL AS timestamp)");

            AssertSql(c, DbFamily.SqlServer, "CAST(NULL AS timestamp)");
            AssertSql(c, DbFamily.Oracle, "CAST(NULL AS datetime)");
        }
    }
}
