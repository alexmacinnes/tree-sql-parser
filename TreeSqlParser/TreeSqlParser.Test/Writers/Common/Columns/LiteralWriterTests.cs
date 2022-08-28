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
    class LiteralWriterTests
    {
        private readonly IReadOnlyDictionary<DbFamily, ISqlWriter> writers = new Dictionary<DbFamily, ISqlWriter>
        {
            { DbFamily.SqlServer, new CommonSqlServerSqlWriter() },
            { DbFamily.Oracle, new CommonOracleSqlWriter() }
        };

        private Column ParseColumn(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select " + sql).Child).Selects[0].Columns[0];

        private string Sql(Column c, DbFamily db) => writers[db].GenerateSql(c);

        [TestCase("1")]
        [TestCase("-3")]
        [TestCase("1.23")]
        [TestCase("-4.5")]
        [TestCase("'abc'")]
        [TestCase("'abc''def'''''")]
        public void SqlSameOnAllDbs(string sql)
        {
            var column = ParseColumn(sql);

            foreach (var x in writers.Values)
            {
                string generatedSql = x.GenerateSql(column);
                Assert.AreEqual(sql, generatedSql);
            }
        }

        [Test]
        public void DateLiteral()
        {
            var c = ParseColumn("{d '2020-12-31'}");

            Assert.AreEqual("{d '2020-12-31'}", Sql(c, DbFamily.SqlServer));
            Assert.AreEqual("DATE '2020-12-31'", Sql(c, DbFamily.Oracle));
        }

        [Test]
        public void DateTimeLiteral()
        {
            var c = ParseColumn("{ts '2020-12-31 23:58:59'}");

            Assert.AreEqual("{ts '2020-12-31 23:58:59'}", Sql(c, DbFamily.SqlServer));
            Assert.AreEqual("TIMESTAMP '2020-12-31 23:58:59'", Sql(c, DbFamily.Oracle));
        }

        [Test]
        public void DateTimeNanosecondsLiteral()
        {
            var c = ParseColumn("{ts '2020-12-31 23:58:59.1234567'}");

            Assert.AreEqual("{ts '2020-12-31 23:58:59.1234567'}", Sql(c, DbFamily.SqlServer));
            Assert.AreEqual("TIMESTAMP '2020-12-31 23:58:59.1234567'", Sql(c, DbFamily.Oracle));
        }
    }
}
