using NUnit.Framework;
using System.Collections.Generic;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;
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
                .WithSql(SqlWriterType.SqlServer, "IIF(1 = 2, 3, 4)")
                .WithSql(SqlWriterType.Oracle, "IIF(1 = 2, 3, 4)")
                .WithSql(SqlWriterType.MySql, "IF(1 = 2, 3, 4)")
                .WithSql(SqlWriterType.MariaDb, "IF(1 = 2, 3, 4)")
                .WithSql(SqlWriterType.Sqlite, "CASE WHEN 1 = 2 THEN 3 ELSE 4 END")
                .WithSql(SqlWriterType.Postgres, "IIF(1 = 2, 3, 4)")
                .WithSql(SqlWriterType.Db2, "IF(1 = 2, 3, 4)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void SubselectColumn()
        {
            var c = ParseColumn("(SELECT 1)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("(SELECT 1)")
                .WithSql(SqlWriterType.Oracle, "(SELECT 1 FROM dual)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void AliasColumn()
        {
            var c = ParseColumn("1 AS foo");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1 AS \"foo\"")
                .WithSql(SqlWriterType.SqlServer, "1 AS [foo]")
                .WithSql(SqlWriterType.MySql, "1 AS `foo`")
                .WithSql(SqlWriterType.MariaDb, "1 AS `foo`")
                .WithSql(SqlWriterType.Sqlite, "1 AS [foo]");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void PrimitiveColumnWithTable()
        {
            var c = ParseColumn("x.y");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\".\"y\"")
                .WithSql(SqlWriterType.SqlServer, "[x].[y]")
                .WithSql(SqlWriterType.MySql, "`x`.`y`")
                .WithSql(SqlWriterType.MariaDb, "`x`.`y`")
                .WithSql(SqlWriterType.Sqlite, "[x].[y]");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void PrimitiveColumnWithoutTable()
        {
            var c = ParseColumn("x");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\"")
                .WithSql(SqlWriterType.SqlServer, "[x]")
                .WithSql(SqlWriterType.MySql, "`x`")
                .WithSql(SqlWriterType.MariaDb, "`x`")
                .WithSql(SqlWriterType.Sqlite, "[x]");

            CommonMother.AssertSql(c, expected);
        }
    }
}
