using NUnit.Framework;
using System.Collections.Generic;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using TreeSqlParser.Writers.Common.MySql;
using TreeSqlParser.Writers.Common.Oracle;
using TreeSqlParser.Writers.Common.Sqlite;
using TreeSqlParser.Writers.Common.SqlServer;

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

            string expectedMySql =
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

            Assert.AreEqual(expectedSqlServer, Sql(s, SqlWriterType.SqlServer));
            Assert.AreEqual(expectedOracle, Sql(s, SqlWriterType.Oracle));
            Assert.AreEqual(expectedMySql, Sql(s, SqlWriterType.MySql));
            Assert.AreEqual(expectedSqlite, Sql(s, SqlWriterType.Sqlite));
            Assert.AreEqual(expectedPostgres, Sql(s, SqlWriterType.Postgres));
            Assert.AreEqual(expectedPostgres, Sql(s, SqlWriterType.Db2));
        }

        [Test]
        public void SelectWithOrderBy()
        {
            string sql = "SELECT 1 ORDER BY 2, 3 DESC";

            var s = ParseSelectStatement(sql);

            Assert.AreEqual("SELECT 1 ORDER BY 2, 3 DESC", Sql(s, SqlWriterType.SqlServer));
            Assert.AreEqual("SELECT 1 FROM dual ORDER BY 2, 3 DESC", Sql(s, SqlWriterType.Oracle));
            Assert.AreEqual("SELECT 1 ORDER BY 2, 3 DESC", Sql(s, SqlWriterType.MySql));
            Assert.AreEqual("SELECT 1 ORDER BY 2, 3 DESC", Sql(s, SqlWriterType.Sqlite));
            Assert.AreEqual("SELECT 1 ORDER BY 2, 3 DESC", Sql(s, SqlWriterType.Postgres));
            Assert.AreEqual("SELECT 1 ORDER BY 2, 3 DESC", Sql(s, SqlWriterType.Db2));
        }
    }
}
