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
                .WithSql("SELECT 1, 2 AS [foo]", SqlWriterType.SqlServer)
                .WithSql("SELECT 1, 2 AS \"foo\" FROM dual", SqlWriterType.Oracle)
                .WithSql("SELECT 1, 2 AS `foo`", SqlWriterType.MySql)
                .WithSql("SELECT 1, 2 AS `foo`", SqlWriterType.MariaDb)
                .WithSql("SELECT 1, 2 AS [foo]", SqlWriterType.Sqlite)
                .WithSql("SELECT 1, 2 AS \"foo\"", SqlWriterType.Postgres)
                .WithSql("SELECT 1, 2 AS \"foo\"", SqlWriterType.Db2);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFrom()
        {
            var s = ParseSelectStatement("SELECT 1 FROM x, y LEFT JOIN z ON 2=3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 FROM \"x\", \"y\" LEFT JOIN \"z\" ON 2 = 3")
                .WithSql("SELECT 1 FROM [x], [y] LEFT JOIN [z] ON 2 = 3", SqlWriterType.SqlServer)
                .WithSql("SELECT 1 FROM `x`, `y` LEFT JOIN `z` ON 2 = 3", SqlWriterType.MySql)
                .WithSql("SELECT 1 FROM `x`, `y` LEFT JOIN `z` ON 2 = 3", SqlWriterType.MariaDb)
                .WithSql("SELECT 1 FROM [x], [y] LEFT JOIN [z] ON 2 = 3", SqlWriterType.Sqlite);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectWhere()
        {
            var s = ParseSelectStatement("SELECT 1 WHERE 2 = 3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 WHERE 2 = 3")
                .WithSql("SELECT 1 FROM dual WHERE 2 = 3", SqlWriterType.Oracle);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectGroupBy()
        {
            var s = ParseSelectStatement("SELECT 1 GROUP BY 2, 3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 GROUP BY 2, 3")
                .WithSql("SELECT 1 FROM dual GROUP BY 2, 3", SqlWriterType.Oracle);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectHaving()
        {
            var s = ParseSelectStatement("SELECT 1 HAVING COUNT(*) > 1");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 HAVING COUNT(*) > 1")
                .WithSql("SELECT 1 FROM dual HAVING COUNT(*) > 1", SqlWriterType.Oracle);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFromWhereGroupByHaving()
        {
            var s = ParseSelectStatement("SELECT 1 FROM foo WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 FROM \"foo\" WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6")
                .WithSql("SELECT 1 FROM [foo] WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6", SqlWriterType.SqlServer)
                .WithSql("SELECT 1 FROM `foo` WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6", SqlWriterType.MySql)
                .WithSql("SELECT 1 FROM `foo` WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6", SqlWriterType.MariaDb)
                .WithSql("SELECT 1 FROM [foo] WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6", SqlWriterType.Sqlite);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFromAnonymousSubselect()
        {
            var s = ParseSelectStatement("SELECT * FROM (SELECT 1)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT * FROM (SELECT 1)")
                .WithSql("SELECT * FROM (SELECT 1 FROM dual)", SqlWriterType.Oracle);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFetch()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 FETCH NEXT 500 ROWS ONLY ");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT [x] ORDER BY 1 FETCH NEXT 500 ROWS ONLY", SqlWriterType.SqlServer)
                .WithSql("SELECT \"x\" FROM dual ORDER BY 1 FETCH NEXT 500 ROWS ONLY", SqlWriterType.Oracle)
                .WithSql("SELECT `x` ORDER BY 1 LIMIT 500", SqlWriterType.MySql)
                .WithSql("SELECT `x` ORDER BY 1 LIMIT 500", SqlWriterType.MariaDb)
                .WithSql("SELECT [x] ORDER BY 1 LIMIT 500", SqlWriterType.Sqlite)
                .WithSql("SELECT \"x\" ORDER BY 1 LIMIT 500", SqlWriterType.Postgres)
                .WithSql("SELECT \"x\" ORDER BY 1 FETCH NEXT 500 ROWS ONLY", SqlWriterType.Db2);

            CommonMother.AssertSql(s, expected); 
        }

        [Test]
        public void SelectOffset()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 OFFSET 500 ROWS");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT [x] ORDER BY 1 OFFSET 500 ROWS", SqlWriterType.SqlServer)
                .WithSql("SELECT \"x\" FROM dual ORDER BY 1 OFFSET 500 ROWS", SqlWriterType.Oracle)
                .WithSql("SELECT `x` ORDER BY 1 LIMIT 500, 2147483647", SqlWriterType.MySql)
                .WithSql("SELECT `x` ORDER BY 1 LIMIT 500, 2147483647", SqlWriterType.MariaDb)
                .WithSql("SELECT [x] ORDER BY 1 LIMIT 2147483647 OFFSET 500", SqlWriterType.Sqlite)
                .WithSql("SELECT \"x\" ORDER BY 1 OFFSET 500", SqlWriterType.Postgres)
                .WithSql("SELECT \"x\" ORDER BY 1 OFFSET 500 ROWS", SqlWriterType.Db2);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectOffsetFetch()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT [x] ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY", SqlWriterType.SqlServer)
                .WithSql("SELECT \"x\" FROM dual ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY", SqlWriterType.Oracle)
                .WithSql("SELECT `x` ORDER BY 1 LIMIT 1500, 500", SqlWriterType.MySql)
                .WithSql("SELECT `x` ORDER BY 1 LIMIT 1500, 500", SqlWriterType.MariaDb)
                .WithSql("SELECT [x] ORDER BY 1 LIMIT 500 OFFSET 1500", SqlWriterType.Sqlite)
                .WithSql("SELECT \"x\" ORDER BY 1 LIMIT 500 OFFSET 1500", SqlWriterType.Postgres)
                .WithSql("SELECT \"x\" ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY", SqlWriterType.Db2);

            CommonMother.AssertSql(s, expected);
        }
    }
}
