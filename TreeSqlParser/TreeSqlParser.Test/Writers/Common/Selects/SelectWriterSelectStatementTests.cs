using NUnit.Framework;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using swt = TreeSqlParser.Writers.SqlWriterType;

namespace TreeSqlParser.Writers.Test.Common.Selects
{
    public class SelectWriterSelectStatementTests
    {
        private string Sql(SelectStatement s, SqlWriterType db) => CommonMother.Sql(s, db);

        private SelectStatement ParseSelectStatement(string sql) =>
            (SelectStatement)SelectParser.ParseSelectStatement(sql).Child;

        [Test]
        public void SelectWithCtes()
        {
            string sql =
                "WITH " +
                "cte1 AS (SELECT 1), " +
                "cte2 AS (SELECT 2 UNION ALL SELECT 3) " +
                "SELECT 4";

            var s = ParseSelectStatement(sql);

            string expectedSqlServerSqlite =
                "WITH " +
                "[cte1] AS (SELECT 1), " +
                "[cte2] AS (SELECT 2 UNION ALL SELECT 3) " +
                "SELECT 4";

            string expectedOracle =
                "WITH " +
                "\"cte1\" AS (SELECT 1 FROM dual), " +
                "\"cte2\" AS (SELECT 2 FROM dual UNION ALL SELECT 3 FROM dual) " +
                "SELECT 4 FROM dual";

            string expectedMySqlMariaDb =
                "WITH " +
                "`cte1` AS (SELECT 1), " +
                "`cte2` AS (SELECT 2 UNION ALL SELECT 3) " +
                "SELECT 4";

            string expectedPostgresDb2 =
                "WITH " +
                "\"cte1\" AS (SELECT 1), " +
                "\"cte2\" AS (SELECT 2 UNION ALL SELECT 3) " +
                "SELECT 4";


            var expected = new ExpectedSqlResult()
                .WithSql(expectedSqlServerSqlite, swt.SqlServer, swt.Sqlite)
                .WithSql(expectedOracle, swt.Oracle)
                .WithSql(expectedMySqlMariaDb, swt.MySql, swt.MariaDb)
                .WithSql(expectedPostgresDb2, swt.Postgres, swt.Db2);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectWithOrderBy()
        {
            string sql = "SELECT 1 ORDER BY 2, 3 DESC";

            var s = ParseSelectStatement(sql);

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 ORDER BY 2, 3 DESC")
                .WithSql("SELECT 1 FROM dual ORDER BY 2, 3 DESC", swt.Oracle);

            CommonMother.AssertSql(s, expected);
        }
    }
}
