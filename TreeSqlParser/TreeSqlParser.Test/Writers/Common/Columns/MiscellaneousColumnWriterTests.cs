using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class MiscellaneousColumnWriterTests
    {
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
                .WithSql("IIF(1 = 2, 3, 4)", SqlWriterType.SqlServer)
                .WithSql("IIF(1 = 2, 3, 4)", SqlWriterType.Oracle)
                .WithSql("IF(1 = 2, 3, 4)", SqlWriterType.MySql)
                .WithSql("IF(1 = 2, 3, 4)", SqlWriterType.MariaDb)
                .WithSql("CASE WHEN 1 = 2 THEN 3 ELSE 4 END", SqlWriterType.Sqlite)
                .WithSql("IIF(1 = 2, 3, 4)", SqlWriterType.Postgres)
                .WithSql("IF(1 = 2, 3, 4)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void SubselectColumn()
        {
            var c = ParseColumn("(SELECT 1)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("(SELECT 1)")
                .WithSql("(SELECT 1 FROM dual)", SqlWriterType.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void AliasColumn()
        {
            var c = ParseColumn("1 AS foo");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1 AS \"foo\"")
                .WithSql("1 AS [foo]", SqlWriterType.SqlServer)
                .WithSql("1 AS `foo`", SqlWriterType.MySql)
                .WithSql("1 AS `foo`", SqlWriterType.MariaDb)
                .WithSql("1 AS [foo]", SqlWriterType.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void PrimitiveColumnWithTable()
        {
            var c = ParseColumn("x.y");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\".\"y\"")
                .WithSql("[x].[y]", SqlWriterType.SqlServer)
                .WithSql("`x`.`y`", SqlWriterType.MySql)
                .WithSql("`x`.`y`", SqlWriterType.MariaDb)
                .WithSql("[x].[y]", SqlWriterType.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void PrimitiveColumnWithoutTable()
        {
            var c = ParseColumn("x");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\"")
                .WithSql("[x]", SqlWriterType.SqlServer)
                .WithSql("`x`", SqlWriterType.MySql)
                .WithSql("`x`", SqlWriterType.MariaDb)
                .WithSql("[x]", SqlWriterType.Sqlite);

            CommonMother.AssertSql(c, expected);
        }
    }
}
