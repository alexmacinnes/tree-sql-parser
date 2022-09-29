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
                .WithSql(SqlWriterType.SqlServer, "{d '2020-12-31'}")
                .WithSql(SqlWriterType.Oracle, "DATE '2020-12-31'")
                .WithSql(SqlWriterType.MySql, "DATE('2020-12-31')")
                .WithSql(SqlWriterType.MariaDb, "DATE('2020-12-31')")
                .WithSql(SqlWriterType.Sqlite, "DATE('2020-12-31')")
                .WithSql(SqlWriterType.Postgres, "DATE '2020-12-31'")
                .WithSql(SqlWriterType.Db2, "DATE '2020-12-31'");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DateTimeLiteral()
        {
            var c = ParseColumn("{ts '2020-12-31 23:58:59'}");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "{ts '2020-12-31 23:58:59'}")
                .WithSql(SqlWriterType.Oracle, "TIMESTAMP '2020-12-31 23:58:59'")
                .WithSql(SqlWriterType.MySql, "TIMESTAMP('2020-12-31  23:58:59')")
                .WithSql(SqlWriterType.MariaDb, "TIMESTAMP('2020-12-31  23:58:59')")
                .WithSql(SqlWriterType.Sqlite, "DATETIME('2020-12-31 23:58:59')")
                .WithSql(SqlWriterType.Postgres, "TIMESTAMP '2020-12-31 23:58:59'")
                .WithSql(SqlWriterType.Db2, "TIMESTAMP '2020-12-31 23:58:59'");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DateTimeNanosecondsLiteral()
        {
            var c = ParseColumn("{ts '2020-12-31 23:58:59.1234567'}");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "{ts '2020-12-31 23:58:59.1234567'}")
                .WithSql(SqlWriterType.Oracle, "TIMESTAMP '2020-12-31 23:58:59.1234567'")
                .WithSql(SqlWriterType.MySql, "TIMESTAMP('2020-12-31  23:58:59.1234567')")
                .WithSql(SqlWriterType.MariaDb, "TIMESTAMP('2020-12-31  23:58:59.1234567')")
                .WithSql(SqlWriterType.Sqlite, "DATETIME('2020-12-31 23:58:59')")
                .WithSql(SqlWriterType.Postgres, "TIMESTAMP '2020-12-31 23:58:59.1234567'")
                .WithSql(SqlWriterType.Db2, "TIMESTAMP '2020-12-31 23:58:59.1234567'");

            CommonMother.AssertSql(c, expected);
        }
    }
}
