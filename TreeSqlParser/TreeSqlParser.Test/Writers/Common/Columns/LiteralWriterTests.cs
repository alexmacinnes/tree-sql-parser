using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using swt = TreeSqlParser.Writers.SqlWriterType;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class LiteralWriterTests
    {
        private static SelectParser SelectParser = new SelectParser();

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
                .WithSql("{d '2020-12-31'}", swt.SqlServer)
                .WithSql("DATE '2020-12-31'", swt.Oracle, swt.Postgres, swt.Db2)
                .WithSql("DATE('2020-12-31')", swt.MySql, swt.MariaDb, swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DateTimeLiteral()
        {
            var c = ParseColumn("{ts '2020-12-31 23:58:59'}");

            var expected = new ExpectedSqlResult()
                .WithSql("{ts '2020-12-31 23:58:59'}", swt.SqlServer)
                .WithSql("TIMESTAMP '2020-12-31 23:58:59'", swt.Oracle, swt.Postgres, swt.Db2)
                .WithSql("TIMESTAMP('2020-12-31 23:58:59')", swt.MySql, swt.MariaDb)
                .WithSql("DATETIME('2020-12-31 23:58:59')", swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DateTimeNanosecondsLiteral()
        {
            var c = ParseColumn("{ts '2020-12-31 23:58:59.1234567'}");

            var expected = new ExpectedSqlResult()
                .WithSql("{ts '2020-12-31 23:58:59.1234567'}", swt.SqlServer)
                .WithSql("TIMESTAMP '2020-12-31 23:58:59.1234567'", swt.Oracle, swt.Postgres, swt.Db2)
                .WithSql("TIMESTAMP('2020-12-31 23:58:59.1234567')", swt.MySql, swt.MariaDb)
                .WithSql("DATETIME('2020-12-31 23:58:59')", swt.Sqlite); ;

            CommonMother.AssertSql(c, expected);
        }
    }
}
