using NUnit.Framework;
using System.Collections.Generic;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Writers.Common.MySql;
using TreeSqlParser.Writers.Common.Oracle;
using TreeSqlParser.Writers.Common.Sqlite;
using TreeSqlParser.Writers.Common.SqlServer;

namespace TreeSqlParser.Writers.Test.Common.Selects
{
    public class SelectWriterSelectStatementTests
    {
        private readonly IReadOnlyDictionary<DbFamily, ISqlWriter> writers = new Dictionary<DbFamily, ISqlWriter>
        {
            { DbFamily.SqlServer, new CommonSqlServerSqlWriter() },
            { DbFamily.Oracle, new CommonOracleSqlWriter() },
            { DbFamily.MySql, new CommonMySqlSqlWriter() },
            { DbFamily.Sqlite, new CommonSqliteSqlWriter() }
        };

        private string Sql(SelectStatement s, DbFamily db) => writers[db].GenerateSql(s);

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

            Assert.AreEqual(expectedSqlServer, Sql(s, DbFamily.SqlServer));
            Assert.AreEqual(expectedOracle, Sql(s, DbFamily.Oracle));
            Assert.AreEqual(expectedMySql, Sql(s, DbFamily.MySql));
            Assert.AreEqual(expectedSqlite, Sql(s, DbFamily.Sqlite));
        }

        [Test]
        public void SelectWithOrderBy()
        {
            string sql = "SELECT 1 ORDER BY 2, 3 DESC";

            var s = ParseSelectStatement(sql);

            Assert.AreEqual("SELECT 1 ORDER BY 2, 3 DESC", Sql(s, DbFamily.SqlServer));
            Assert.AreEqual("SELECT 1 FROM dual ORDER BY 2, 3 DESC", Sql(s, DbFamily.Oracle));
            Assert.AreEqual("SELECT 1 ORDER BY 2, 3 DESC", Sql(s, DbFamily.MySql));
            Assert.AreEqual("SELECT 1 ORDER BY 2, 3 DESC", Sql(s, DbFamily.Sqlite));
        }
    }
}
