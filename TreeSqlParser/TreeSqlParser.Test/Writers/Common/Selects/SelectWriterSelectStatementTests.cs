using NUnit.Framework;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

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

            string expectedSqlServer =
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

            string expectedSqlite =
                "WITH " +
                "[cte1] AS (SELECT 1), " +
                "[cte2] AS (SELECT 2 UNION ALL SELECT 3) " +
                "SELECT 4";

            string expectedPostgres =
                "WITH " +
                "\"cte1\" AS (SELECT 1), " +
                "\"cte2\" AS (SELECT 2 UNION ALL SELECT 3) " +
                "SELECT 4";

            string expectedDb2 =
                "WITH " +
                "\"cte1\" AS (SELECT 1), " +
                "\"cte2\" AS (SELECT 2 UNION ALL SELECT 3) " +
                "SELECT 4";

            var expected = new ExpectedSqlResult()
                .WithSql(expectedSqlServer, SqlWriterType.SqlServer)
                .WithSql(expectedOracle, SqlWriterType.Oracle)
                .WithSql(expectedMySqlMariaDb, SqlWriterType.MySql)
                .WithSql(expectedMySqlMariaDb, SqlWriterType.MariaDb)
                .WithSql(expectedSqlite, SqlWriterType.Sqlite)
                .WithSql(expectedPostgres, SqlWriterType.Postgres)
                .WithSql(expectedDb2, SqlWriterType.Db2);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectWithOrderBy()
        {
            string sql = "SELECT 1 ORDER BY 2, 3 DESC";

            var s = ParseSelectStatement(sql);

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 ORDER BY 2, 3 DESC")
                .WithSql("SELECT 1 FROM dual ORDER BY 2, 3 DESC", SqlWriterType.Oracle);

            CommonMother.AssertSql(s, expected);
        }
    }
}
