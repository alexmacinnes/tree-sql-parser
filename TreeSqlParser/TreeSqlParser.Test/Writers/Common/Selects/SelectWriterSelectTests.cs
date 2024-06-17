using NUnit.Framework;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using swt = TreeSqlParser.Writers.SqlWriterType;

namespace TreeSqlParser.Writers.Test.Common.Selects
{
    public class SelectWriterSelectTests
    {
        private static SelectParser SelectParser = new SelectParser();

        private string Sql(SelectStatement s, SqlWriterType db) => CommonMother.Sql(s, db);

        private SelectStatement ParseSelectStatement(string sql) =>
            (SelectStatement)SelectParser.ParseSelectStatement(sql).Child;

        [Test]
        public void SelectOnly()
        {
            var s = ParseSelectStatement("SELECT 1, 2 AS foo");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT 1, 2 AS [foo]", swt.SqlServer, swt.Sqlite)
                .WithSql("SELECT 1, 2 AS \"foo\" FROM dual", swt.Oracle)
                .WithSql("SELECT 1, 2 AS `foo`", swt.MySql, swt.MariaDb)
                .WithSql("SELECT 1, 2 AS \"foo\"", swt.Postgres, swt.Db2);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFrom()
        {
            var s = ParseSelectStatement("SELECT 1 FROM x, y LEFT JOIN z ON 2=3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 FROM \"x\", \"y\" LEFT JOIN \"z\" ON 2 = 3")
                .WithSql("SELECT 1 FROM [x], [y] LEFT JOIN [z] ON 2 = 3", swt.SqlServer, swt.Sqlite)
                .WithSql("SELECT 1 FROM `x`, `y` LEFT JOIN `z` ON 2 = 3", swt.MySql, swt.MariaDb); 

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectWhere()
        {
            var s = ParseSelectStatement("SELECT 1 WHERE 2 = 3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 WHERE 2 = 3")
                .WithSql("SELECT 1 FROM dual WHERE 2 = 3", swt.Oracle);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectGroupBy()
        {
            var s = ParseSelectStatement("SELECT 1 GROUP BY 2, 3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 GROUP BY 2, 3")
                .WithSql("SELECT 1 FROM dual GROUP BY 2, 3", swt.Oracle);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectHaving()
        {
            var s = ParseSelectStatement("SELECT 1 HAVING COUNT(*) > 1");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 HAVING COUNT(*) > 1")
                .WithSql("SELECT 1 FROM dual HAVING COUNT(*) > 1", swt.Oracle);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFromWhereGroupByHaving()
        {
            var s = ParseSelectStatement("SELECT 1 FROM foo WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT 1 FROM \"foo\" WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6")
                .WithSql("SELECT 1 FROM [foo] WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6", swt.SqlServer, swt.Sqlite)
                .WithSql("SELECT 1 FROM `foo` WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFromAnonymousSubselect()
        {
            var s = ParseSelectStatement("SELECT * FROM (SELECT 1)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SELECT * FROM (SELECT 1)")
                .WithSql("SELECT * FROM (SELECT 1 FROM dual)", swt.Oracle);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectFetch()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 FETCH NEXT 500 ROWS ONLY ");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT [x] ORDER BY 1 FETCH NEXT 500 ROWS ONLY", swt.SqlServer)
                .WithSql("SELECT \"x\" FROM dual ORDER BY 1 FETCH NEXT 500 ROWS ONLY", swt.Oracle)
                .WithSql("SELECT `x` ORDER BY 1 LIMIT 500", swt.MySql, swt.MariaDb)
                .WithSql("SELECT [x] ORDER BY 1 LIMIT 500", swt.Sqlite)
                .WithSql("SELECT \"x\" ORDER BY 1 LIMIT 500", swt.Postgres)
                .WithSql("SELECT \"x\" ORDER BY 1 FETCH NEXT 500 ROWS ONLY", swt.Db2);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectTop()
        {
            var s = ParseSelectStatement("SELECT TOP 3 x");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT TOP (3) [x]", swt.SqlServer)
                .WithDefaultSql("EXCEPTION: Select Top is not supported");

            CommonMother.AssertSql(s, expected); 
        }

        [Test]
        public void SelectTopPercent()
        {
            var s = ParseSelectStatement("SELECT TOP 3 PERCENT x");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT TOP (3) PERCENT [x]", swt.SqlServer)
                .WithDefaultSql("EXCEPTION: Select Top is not supported");

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectTopWithTies()
        {
            var s = ParseSelectStatement("SELECT TOP 3 WITH TIES x");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT TOP (3) WITH TIES [x]", swt.SqlServer)
                .WithDefaultSql("EXCEPTION: Select Top is not supported");

            CommonMother.AssertSql(s, expected);
        }
        [Test]
        public void SelectTopComplete()
        {
            var s = ParseSelectStatement("SELECT TOP 3 PERCENT WITH TIES x");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT TOP (3) PERCENT WITH TIES [x]", swt.SqlServer)
                .WithDefaultSql("EXCEPTION: Select Top is not supported");

            CommonMother.AssertSql(s, expected);
        }


        [Test]
        public void SelectOffset()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 OFFSET 500 ROWS");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT [x] ORDER BY 1 OFFSET 500 ROWS", swt.SqlServer)
                .WithSql("SELECT \"x\" FROM dual ORDER BY 1 OFFSET 500 ROWS", swt.Oracle)
                .WithSql("SELECT `x` ORDER BY 1 LIMIT 500, 2147483647", swt.MySql, swt.MariaDb)
                .WithSql("SELECT [x] ORDER BY 1 LIMIT 2147483647 OFFSET 500", swt.Sqlite)
                .WithSql("SELECT \"x\" ORDER BY 1 OFFSET 500", swt.Postgres)
                .WithSql("SELECT \"x\" ORDER BY 1 OFFSET 500 ROWS", swt.Db2);

            CommonMother.AssertSql(s, expected);
        }

        [Test]
        public void SelectOffsetFetch()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY");

            var expected = new ExpectedSqlResult()
                .WithSql("SELECT [x] ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY", swt.SqlServer)
                .WithSql("SELECT \"x\" FROM dual ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY", swt.Oracle)
                .WithSql("SELECT `x` ORDER BY 1 LIMIT 1500, 500", swt.MySql, swt.MariaDb)
                .WithSql("SELECT [x] ORDER BY 1 LIMIT 500 OFFSET 1500", swt.Sqlite)
                .WithSql("SELECT \"x\" ORDER BY 1 LIMIT 500 OFFSET 1500", swt.Postgres)
                .WithSql("SELECT \"x\" ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY", swt.Db2);

            CommonMother.AssertSql(s, expected);
        }
    }
}
