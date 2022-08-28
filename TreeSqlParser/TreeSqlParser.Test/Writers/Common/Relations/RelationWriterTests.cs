using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Relations;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Relations
{
    public class RelationWriterTests
    {
        private readonly IReadOnlyDictionary<DbFamily, ISqlWriter> writers = new Dictionary<DbFamily, ISqlWriter>
        {
            { DbFamily.SqlServer, new CommonSqlServerSqlWriter() },
            { DbFamily.Oracle, new CommonOracleSqlWriter() }
        };

        private string Sql(Relation r, DbFamily db) => writers[db].GenerateSql(r);

        private Relation ParseRelation(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select * from " + sql).Child).Selects[0].From[0];

        [Test]
        public void SingleTable()
        {
            var relation = ParseRelation("x");

            Assert.AreEqual("[x]", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("\"x\"", Sql(relation, DbFamily.Oracle));
        }

        [Test]
        public void TwoPartTable()
        {
            var relation = ParseRelation("x.y");

            Assert.AreEqual("[x].[y]", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("\"x\".\"y\"", Sql(relation, DbFamily.Oracle));
        }

        [Test]
        public void BracketRelation()
        {
            var relation = ParseRelation("(x)");

            Assert.AreEqual("([x])", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("(\"x\")", Sql(relation, DbFamily.Oracle));
        }

        [Test]
        public void SubselectRelation()
        {
            var relation = ParseRelation("(select * from x) AS foo");

            Assert.AreEqual("(SELECT * FROM [x]) AS [foo]", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("(SELECT * FROM \"x\") AS \"foo\"", Sql(relation, DbFamily.Oracle));
        }

        [Test]
        public void InnerJoin()
        {
            var relation = ParseRelation("x INNER JOIN y ON 1=2");

            Assert.AreEqual("[x] INNER JOIN [y] ON 1 = 2", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("\"x\" INNER JOIN \"y\" ON 1 = 2", Sql(relation, DbFamily.Oracle));
        }

        [Test]
        public void LeftJoin()
        {
            var relation = ParseRelation("x LEFT JOIN y ON 1=2");

            Assert.AreEqual("[x] LEFT JOIN [y] ON 1 = 2", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("\"x\" LEFT JOIN \"y\" ON 1 = 2", Sql(relation, DbFamily.Oracle));
        }

        [Test]
        public void RightJoin()
        {
            var relation = ParseRelation("x RIGHT JOIN y ON 1=2");

            Assert.AreEqual("[x] RIGHT JOIN [y] ON 1 = 2", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("\"x\" RIGHT JOIN \"y\" ON 1 = 2", Sql(relation, DbFamily.Oracle));
        }

        [Test]
        public void FullJoin()
        {
            var relation = ParseRelation("x FULL JOIN y ON 1=2");

            Assert.AreEqual("[x] FULL JOIN [y] ON 1 = 2", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("\"x\" FULL JOIN \"y\" ON 1 = 2", Sql(relation, DbFamily.Oracle));
        }

        [Test]
        public void CrossJoin()
        {
            var relation = ParseRelation("x CROSS JOIN y");

            Assert.AreEqual("[x] CROSS JOIN [y]", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("\"x\" CROSS JOIN \"y\"", Sql(relation, DbFamily.Oracle));
        }

        [Test]
        public void ChainedJoin()
        {
            var relation = ParseRelation("x INNER JOIN y ON 1 = 2 LEFT JOIN z ON 3 = 4");

            Assert.AreEqual("[x] INNER JOIN [y] ON 1 = 2 LEFT JOIN [z] ON 3 = 4", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("\"x\" INNER JOIN \"y\" ON 1 = 2 LEFT JOIN \"z\" ON 3 = 4", Sql(relation, DbFamily.Oracle));
        }

        [Test]
        public void NestedJoin()
        {
            var relation = ParseRelation("x LEFT JOIN y INNER JOIN z ON 1 = 2 ON 3 = 4");

            Assert.AreEqual("[x] LEFT JOIN [y] INNER JOIN [z] ON 1 = 2 ON 3 = 4", Sql(relation, DbFamily.SqlServer));
            Assert.AreEqual("\"x\" LEFT JOIN \"y\" INNER JOIN \"z\" ON 1 = 2 ON 3 = 4", Sql(relation, DbFamily.Oracle));
        }
    }
}
