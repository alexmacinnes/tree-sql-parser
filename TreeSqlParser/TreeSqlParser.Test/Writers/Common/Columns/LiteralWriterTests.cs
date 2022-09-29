using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class LiteralWriterTests
    {
        private Column ParseColumn(string sql) =>
            (Column) SelectParser.ParseColumn(sql).Child;

        [TestCase("1")]
        [TestCase("-3")]
        [TestCase("1.23")]
        [TestCase("-4.5")]
        [TestCase("'abc'")]
        [TestCase("'abc''def'''''")]
        public void SqlSameOnAllDbs(string sql)
        {
            var c = ParseColumn(sql);

            var expected = new ExpectedSqlResult()
                .WithDefaultSql(sql);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DateLiteral()
        {
            var c = ParseColumn("{d '2020-12-31'}");

            var expected = new ExpectedSqlResult()
                .WithSql("{d '2020-12-31'}", SqlWriterType.SqlServer)
                .WithSql("DATE '2020-12-31'", SqlWriterType.Oracle)
                .WithSql("DATE('2020-12-31')", SqlWriterType.MySql)
                .WithSql("DATE('2020-12-31')", SqlWriterType.MariaDb)
                .WithSql("DATE('2020-12-31')", SqlWriterType.Sqlite)
                .WithSql("DATE '2020-12-31'", SqlWriterType.Postgres)
                .WithSql("DATE '2020-12-31'", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DateTimeLiteral()
        {
            var c = ParseColumn("{ts '2020-12-31 23:58:59'}");

            var expected = new ExpectedSqlResult()
                .WithSql("{ts '2020-12-31 23:58:59'}", SqlWriterType.SqlServer)
                .WithSql("TIMESTAMP '2020-12-31 23:58:59'", SqlWriterType.Oracle)
                .WithSql("TIMESTAMP('2020-12-31  23:58:59')", SqlWriterType.MySql)
                .WithSql("TIMESTAMP('2020-12-31  23:58:59')", SqlWriterType.MariaDb)
                .WithSql("DATETIME('2020-12-31 23:58:59')", SqlWriterType.Sqlite)
                .WithSql("TIMESTAMP '2020-12-31 23:58:59'", SqlWriterType.Postgres)
                .WithSql("TIMESTAMP '2020-12-31 23:58:59'", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DateTimeNanosecondsLiteral()
        {
            var c = ParseColumn("{ts '2020-12-31 23:58:59.1234567'}");

            var expected = new ExpectedSqlResult()
                .WithSql("{ts '2020-12-31 23:58:59.1234567'}", SqlWriterType.SqlServer)
                .WithSql("TIMESTAMP '2020-12-31 23:58:59.1234567'", SqlWriterType.Oracle)
                .WithSql("TIMESTAMP('2020-12-31  23:58:59.1234567')", SqlWriterType.MySql)
                .WithSql("TIMESTAMP('2020-12-31  23:58:59.1234567')", SqlWriterType.MariaDb)
                .WithSql("DATETIME('2020-12-31 23:58:59')", SqlWriterType.Sqlite)
                .WithSql("TIMESTAMP '2020-12-31 23:58:59.1234567'", SqlWriterType.Postgres)
                .WithSql("TIMESTAMP '2020-12-31 23:58:59.1234567'", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }
    }
}
