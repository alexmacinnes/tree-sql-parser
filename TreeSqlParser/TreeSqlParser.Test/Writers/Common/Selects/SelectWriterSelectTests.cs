using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Writers.Safe;

namespace TreeSqlParser.Writers.Test.Safe.Selects
{
    public class SelectWriterSelectTests
    {
        private readonly IReadOnlyDictionary<DbFamily, ISqlWriter> writers = new Dictionary<DbFamily, ISqlWriter>
        {
            { DbFamily.SqlServer, new SafeSqlServerWriter() },
            { DbFamily.Oracle, new SafeOracleSqlWriter() }
        };

        private string Sql(SelectStatement s, DbFamily db) => writers[db].GenerateSql(s);

        private SelectStatement ParseSelectStatement(string sql) =>
            (SelectStatement)SelectParser.ParseSelectStatement(sql).Child;

        [Test]
        public void SelectOnly()
        {
            var s = ParseSelectStatement("SELECT 1, 2 AS foo");

            Assert.AreEqual("SELECT 1, 2 AS [foo]", Sql(s, DbFamily.SqlServer));
            Assert.AreEqual("SELECT 1, 2 AS \"foo\" FROM dual", Sql(s, DbFamily.Oracle));
        }

        [Test]
        public void SelectFrom()
        {
            var s = ParseSelectStatement("SELECT 1 FROM x, y LEFT JOIN z ON 2=3");

            Assert.AreEqual(
                "SELECT 1 FROM [x], [y] LEFT JOIN [z] ON 2 = 3", 
                Sql(s, DbFamily.SqlServer));
            Assert.AreEqual(
                "SELECT 1 FROM \"x\", \"y\" LEFT JOIN \"z\" ON 2 = 3", 
                Sql(s, DbFamily.Oracle));
        }

        [Test]
        public void SelectWhere()
        {
            var s = ParseSelectStatement("SELECT 1 FROM x, y LEFT JOIN z ON 2=3");

            Assert.AreEqual(
                "SELECT 1 FROM [x], [y] LEFT JOIN [z] ON 2 = 3",
                Sql(s, DbFamily.SqlServer));
            Assert.AreEqual(
                "SELECT 1 FROM \"x\", \"y\" LEFT JOIN \"z\" ON 2 = 3",
                Sql(s, DbFamily.Oracle));
        }

        [Test]
        public void SelectGroupBy()
        {
            var s = ParseSelectStatement("SELECT 1 GROUP BY 2, 3");

            Assert.AreEqual(
                "SELECT 1 GROUP BY 2, 3",
                Sql(s, DbFamily.SqlServer));
            Assert.AreEqual(
                "SELECT 1 FROM dual GROUP BY 2, 3",
                Sql(s, DbFamily.Oracle));
        }

        [Test]
        public void SelectHaving()
        {
            var s = ParseSelectStatement("SELECT 1 HAVING COUNT(*) > 1");

            Assert.AreEqual(
                "SELECT 1 HAVING COUNT(*) > 1",
                Sql(s, DbFamily.SqlServer));
            Assert.AreEqual(
                "SELECT 1 FROM dual HAVING COUNT(*) > 1",
                Sql(s, DbFamily.Oracle));
        }

        [Test]
        public void SelectFromWhereGroupByHaving()
        {
            var s = ParseSelectStatement("SELECT 1 FROM foo WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6");

            Assert.AreEqual(
                "SELECT 1 FROM [foo] WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6",
                Sql(s, DbFamily.SqlServer));
            Assert.AreEqual(
                "SELECT 1 FROM \"foo\" WHERE 2 = 3 GROUP BY 4 HAVING SUM(5) > 6",
                Sql(s, DbFamily.Oracle));
        }

        [Test]
        public void SelectFromAnonymousSubselect()
        {
            var s = ParseSelectStatement("SELECT * FROM (SELECT 1)");

            Assert.AreEqual(
                "SELECT * FROM (SELECT 1)",
                Sql(s, DbFamily.SqlServer));
            Assert.AreEqual(
                "SELECT * FROM (SELECT 1 FROM dual)",
                Sql(s, DbFamily.Oracle));
        }

        [Test]
        public void SelectFetch()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 FETCH NEXT 500 ROWS ONLY ");

            Assert.AreEqual(
                "SELECT [x] ORDER BY 1 FETCH NEXT 500 ROWS ONLY",
                Sql(s, DbFamily.SqlServer));
            Assert.AreEqual(
                "SELECT \"x\" FROM dual ORDER BY 1 FETCH NEXT 500 ROWS ONLY",
                Sql(s, DbFamily.Oracle));
        }

        [Test]
        public void SelectOffset()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 OFFSET 500 ROWS");

            Assert.AreEqual(
                "SELECT [x] ORDER BY 1 OFFSET 500 ROWS",
                Sql(s, DbFamily.SqlServer));
            Assert.AreEqual(
                "SELECT \"x\" FROM dual ORDER BY 1 OFFSET 500 ROWS",
                Sql(s, DbFamily.Oracle));
        }

        [Test]
        public void SelectOffsetFetch()
        {
            var s = ParseSelectStatement("SELECT x ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY");

            Assert.AreEqual(
                "SELECT [x] ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY",
                Sql(s, DbFamily.SqlServer));
            Assert.AreEqual(
                "SELECT \"x\" FROM dual ORDER BY 1 OFFSET 1500 ROWS FETCH NEXT 500 ROWS ONLY",
                Sql(s, DbFamily.Oracle));
        }
    }
}
