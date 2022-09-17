using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;


namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class LiteralWriterTests
    {
        private Column ParseColumn(string sql) =>
            SelectParser.ParseColumn(sql);

        private string Sql(Column c, SqlWriterType db) => CommonMother.Sql(c, db);

        [TestCase("1")]
        [TestCase("-3")]
        [TestCase("1.23")]
        [TestCase("-4.5")]
        [TestCase("'abc'")]
        [TestCase("'abc''def'''''")]
        public void SqlSameOnAllDbs(string sql)
        {
            var column = ParseColumn(sql);

            foreach (var x in CommonMother.AllCommonWriters)
            {
                string generatedSql = x.GenerateSql(column);
                Assert.AreEqual(sql, generatedSql);
            }
        }

        [Test]
        public void DateLiteral()
        {
            var c = ParseColumn("{d '2020-12-31'}");

            Assert.AreEqual("{d '2020-12-31'}", Sql(c, SqlWriterType.SqlServer));
            Assert.AreEqual("DATE '2020-12-31'", Sql(c, SqlWriterType.Oracle));
            Assert.AreEqual("DATE('2020-12-31')", Sql(c, SqlWriterType.MySql));
            Assert.AreEqual("DATE('2020-12-31')", Sql(c, SqlWriterType.Sqlite));
            Assert.AreEqual("DATE '2020-12-31'", Sql(c, SqlWriterType.Postgres));
        }

        [Test]
        public void DateTimeLiteral()
        {
            var c = ParseColumn("{ts '2020-12-31 23:58:59'}");

            Assert.AreEqual("{ts '2020-12-31 23:58:59'}", Sql(c, SqlWriterType.SqlServer));
            Assert.AreEqual("TIMESTAMP '2020-12-31 23:58:59'", Sql(c, SqlWriterType.Oracle));
            Assert.AreEqual("TIMESTAMP('2020-12-31  23:58:59')", Sql(c, SqlWriterType.MySql));
            Assert.AreEqual("DATETIME('2020-12-31 23:58:59')", Sql(c, SqlWriterType.Sqlite));
            Assert.AreEqual("TIMESTAMP '2020-12-31 23:58:59'", Sql(c, SqlWriterType.Postgres));
        }

        [Test]
        public void DateTimeNanosecondsLiteral()
        {
            var c = ParseColumn("{ts '2020-12-31 23:58:59.1234567'}");

            Assert.AreEqual("{ts '2020-12-31 23:58:59.1234567'}", Sql(c, SqlWriterType.SqlServer));
            Assert.AreEqual("TIMESTAMP '2020-12-31 23:58:59.1234567'", Sql(c, SqlWriterType.Oracle));
            Assert.AreEqual("TIMESTAMP('2020-12-31  23:58:59.1234567')", Sql(c, SqlWriterType.MySql));
            Assert.AreEqual("DATETIME('2020-12-31 23:58:59')", Sql(c, SqlWriterType.Sqlite));
            Assert.AreEqual("TIMESTAMP '2020-12-31 23:58:59.1234567'", Sql(c, SqlWriterType.Postgres));
        }
    }
}
