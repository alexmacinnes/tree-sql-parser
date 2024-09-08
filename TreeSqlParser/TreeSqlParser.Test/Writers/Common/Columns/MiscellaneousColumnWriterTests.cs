using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using swt = TreeSqlParser.Writers.SqlWriterType;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class MiscellaneousColumnWriterTests
    {
        private static SelectParser SelectParser = new SelectParser();

        private string Sql(Column c, SqlWriterType db) => CommonMother.Sql(c, db);

        private Column ParseColumn(string sql) =>
            (Column) SelectParser.ParseColumn(sql).Child;

        [TestCase("*")]
        [TestCase("(1)")]
        [TestCase("CASE WHEN 1 = 2 THEN 3 WHEN 4 = 5 THEN 6 END")]
        [TestCase("CASE WHEN 1 = 2 THEN 3 WHEN 4 = 5 THEN 6 ELSE 7 END")]
        public void SqlSameOnAllDbs(string sql)
        {
            var c = ParseColumn(sql);

            var expected = new ExpectedSqlResult()
                .WithDefaultSql(sql);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void IifColumn()
        {
            var c = ParseColumn("IIF(1 = 2, 3, 4)");

            var expected = new ExpectedSqlResult()
                .WithSql("IIF(1 = 2, 3, 4)", swt.SqlServer, swt.Oracle, swt.Postgres)
                .WithSql("IF(1 = 2, 3, 4)", swt.MySql, swt.MariaDb, swt.Db2)
                .WithSql("CASE WHEN 1 = 2 THEN 3 ELSE 4 END", swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void SubselectColumn()
        {
            var c = ParseColumn("(SELECT 1)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("(SELECT 1)")
                .WithSql("(SELECT 1 FROM dual)", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void AliasColumn()
        {
            var c = ParseColumn("1 AS foo");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1 AS \"foo\"")
                .WithSql("1 AS [foo]", swt.SqlServer, swt.Sqlite)
                .WithSql("1 AS `foo`", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void PrimitiveColumnWithTable()
        {
            var c = ParseColumn("x.y");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\".\"y\"")
                .WithSql("[x].[y]", swt.SqlServer, swt.Sqlite)
                .WithSql("`x`.`y`", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void PrimitiveColumnWithoutTable()
        {
            var c = ParseColumn("x");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\"")
                .WithSql("[x]", swt.SqlServer, swt.Sqlite)
                .WithSql("`x`", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void StarColumn()
        {
            var c = ParseColumn("*");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("*");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void TableStarColumn()
        {
            var c = ParseColumn("tbl.*");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"tbl\".*")
                .WithSql("[tbl].*", swt.SqlServer, swt.Sqlite)
                .WithSql("`tbl`.*", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(c, expected);
        }
    }
}
