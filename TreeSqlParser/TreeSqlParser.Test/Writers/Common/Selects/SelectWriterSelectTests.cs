using NUnit.Framework;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Selects
{
    public class SelectWriterSelectTests
    {
        private string Sql(SelectStatement s, SqlWriterType db) => CommonMother.Sql(s, db);

        private SelectStatement ParseSelectStatement(string sql) =>
            (SelectStatement)SelectParser.ParseSelectStatement(sql).Child;

        [Test]
        public void SelectOnly()
        {
            var s = ParseSelectStatement("SELECT 1, 2 AS foo");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "SELECT 1, 2 AS [foo]")
                .WithSql(SqlWriterType.Oracle, "SELECT 1, 2 AS \"foo\" FROM dual")
                .WithSql(SqlWriterType.MySql, "SELECT 1, 2 AS `foo`")
                .WithSql(SqlWriterType.MariaDb, "SELECT 1, 2 AS `foo`")
                .WithSql(SqlWriterType.Sqlite, "SELECT 1, 2 AS [foo]")
                .WithSql(SqlWriterType.Postgres, "SELECT 1, 2 AS \"foo\"")
                .WithSql(SqlWriterType.Db2, "SELECT 1, 2 AS \"foo\"");

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFrom()
        {
            var s = ParseSelectStatement("SELECT 1 FROM x, y LEFT JOIN z ON 2=3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 FROM \"x\", \"y\" LEFT JOIN \"z\" ON 2 = 3")
                .WithSql(SqlWriterType.SqlServer, "SELECT 1 FROM [x], [y] LEFT JOIN [z] ON 2 = 3")
                .WithSql(SqlWriterType.MySql, "SELECT 1 FROM `x`, `y` LEFT JOIN `z` ON 2 = 3")
                .WithSql(SqlWriterType.MariaDb, "SELECT 1 FROM `x`, `y` LEFT JOIN `z` ON 2 = 3")
                .WithSql(SqlWriterType.Sqlite, "SELECT 1 FROM [x], [y] LEFT JOIN [z] ON 2 = 3");

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectWhere()
        {
            var s = ParseSelectStatement("SELECT 1 WHERE 2 = 3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 WHERE 2 = 3")
                .WithSql(SqlWriterType.Oracle, "SELECT 1 FROM dual WHERE 2 = 3");

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectGroupBy()
        {
            var s = ParseSelectStatement("SELECT 1 GROUP BY 2, 3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 GROUP BY 2, 3")
                .WithSql(SqlWriterType.Oracle, "SELECT 1 FROM dual GROUP BY 2, 3");

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectHaving()
        {
            var s = ParseSelectStatement("SELECT 1 HAVING COUNT(*) > 1");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 HAVING COUNT(*) > 1")
                .WithSql(SqlWriterType.Oracle, "SELECT 1 FROM dual HAVING COUNT(*) > 1");

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFromWhereGroupByHaving()
        {
            var s = ParseSelectStatement("SELECT 1 FROM foo WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 FROM \"foo\" WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6")
                .WithSql(SqlWriterType.SqlServer, "SELECT 1 FROM [foo] WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6")
                .WithSql(SqlWriterType.MySql, "SELECT 1 FROM `foo` WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6")
                .WithSql(SqlWriterType.MariaDb, "SELECT 1 FROM `foo` WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6")
                .WithSql(SqlWriterType.Sqlite, "SELECT 1 FROM [foo] WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6");

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFromAnonymousSubselect()
        {
            var s = ParseSelectStatement("SELECT * FROM (SELECT 1)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT * FROM (SELECT 1)")
                .WithSql(SqlWriterType.Oracle, "SELECT * FROM (SELECT 1 FROM dual)");

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFetch()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 FETCH NEXT 500 ROWS ONLY ");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "SELECT [x] ORDER BY 1 FETCH NEXT 500 ROWS ONLY")
                .WithSql(SqlWriterType.Oracle, "SELECT \"x\" FROM dual ORDER BY 1 FETCH NEXT 500 ROWS ONLY")
                .WithSql(SqlWriterType.MySql, "SELECT `x` ORDER BY 1 LIMIT 500")
                .WithSql(SqlWriterType.MariaDb, "SELECT `x` ORDER BY 1 LIMIT 500")
                .WithSql(SqlWriterType.Sqlite, "SELECT [x] ORDER BY 1 LIMIT 500")
                .WithSql(SqlWriterType.Postgres, "SELECT \"x\" ORDER BY 1 LIMIT 500")
                .WithSql(SqlWriterType.Db2, "SELECT \"x\" ORDER BY 1 FETCH NEXT 500 ROWS ONLY");

            CommonMother.AssertSql(s, expected); 
        }

        [Test]
        public void SelectOffset()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 OFFSET 500 ROWS");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "SELECT [x] ORDER BY 1 OFFSET 500 ROWS")
                .WithSql(SqlWriterType.Oracle, "SELECT \"x\" FROM dual ORDER BY 1 OFFSET 500 ROWS")
                .WithSql(SqlWriterType.MySql, "SELECT `x` ORDER BY 1 LIMIT 500, 2147483647")
                .WithSql(SqlWriterType.MariaDb, "SELECT `x` ORDER BY 1 LIMIT 500, 2147483647")
                .WithSql(SqlWriterType.Sqlite, "SELECT [x] ORDER BY 1 LIMIT 2147483647 OFFSET 500")
                .WithSql(SqlWriterType.Postgres, "SELECT \"x\" ORDER BY 1 OFFSET 500")
                .WithSql(SqlWriterType.Db2, "SELECT \"x\" ORDER BY 1 OFFSET 500 ROWS");

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectOffsetFetch()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "SELECT [x] ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY")
                .WithSql(SqlWriterType.Oracle, "SELECT \"x\" FROM dual ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY")
                .WithSql(SqlWriterType.MySql, "SELECT `x` ORDER BY 1 LIMIT 1500, 500")
                .WithSql(SqlWriterType.MariaDb, "SELECT `x` ORDER BY 1 LIMIT 1500, 500")
                .WithSql(SqlWriterType.Sqlite, "SELECT [x] ORDER BY 1 LIMIT 500 OFFSET 1500")
                .WithSql(SqlWriterType.Postgres, "SELECT \"x\" ORDER BY 1 LIMIT 500 OFFSET 1500")
                .WithSql(SqlWriterType.Db2, "SELECT \"x\" ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY");

            CommonMother.AssertSql(s, expected);
        }
    }
}
