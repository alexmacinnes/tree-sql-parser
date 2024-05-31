using NUnit.Framework;
using TreeSqlParser.Model.Relations;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using swt = TreeSqlParser.Writers.SqlWriterType;

namespace TreeSqlParser.Writers.Test.Common.Relations
{
    public class RelationWriterTests
    {
        private static SelectParser SelectParser = new SelectParser();

        private Relation ParseRelation(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select * from " + sql).Child).Selects[0].From[0];

        [Test]
        public void SingleTable()
        {
            var relation = ParseRelation("x");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\"")
                .WithSql("[x]", swt.SqlServer, swt.Sqlite)
                .WithSql("`x`", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void TwoPartTable()
        {
            var relation = ParseRelation("x.y");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\".\"y\"")
                .WithSql("[x].[y]", swt.SqlServer, swt.Sqlite)
                .WithSql("`x`.`y`", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void BracketRelation()
        {
            var relation = ParseRelation("(x)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("(\"x\")")
                .WithSql("([x])", swt.SqlServer, swt.Sqlite)
                .WithSql("(`x`)", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void SubselectRelation()
        {
            var relation = ParseRelation("(select * from x) AS foo");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("(SELECT * FROM \"x\") AS \"foo\"")
                .WithSql("(SELECT * FROM [x]) AS [foo]", swt.SqlServer, swt.Sqlite)
                .WithSql("(SELECT * FROM `x`) AS `foo`", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void InnerJoin()
        {
            var relation = ParseRelation("x INNER JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" INNER JOIN \"y\" ON 1 = 2")
                .WithSql("[x] INNER JOIN [y] ON 1 = 2", swt.SqlServer, swt.Sqlite)
                .WithSql("`x` INNER JOIN `y` ON 1 = 2", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void LeftJoin()
        {
            var relation = ParseRelation("x LEFT JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" LEFT JOIN \"y\" ON 1 = 2")
                .WithSql("[x] LEFT JOIN [y] ON 1 = 2", swt.SqlServer, swt.Sqlite)
                .WithSql("`x` LEFT JOIN `y` ON 1 = 2", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RightJoin()
        {
            var relation = ParseRelation("x RIGHT JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" RIGHT JOIN \"y\" ON 1 = 2")
                .WithSql("[x] RIGHT JOIN [y] ON 1 = 2", swt.SqlServer)
                .WithSql("`x` RIGHT JOIN `y` ON 1 = 2", swt.MySql, swt.MariaDb)
                .WithSql("[y] LEFT JOIN [x] ON 1 = 2", swt.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void FullJoin()
        {
            var relation = ParseRelation("x FULL JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" FULL JOIN \"y\" ON 1 = 2")
                .WithSql("[x] FULL JOIN [y] ON 1 = 2", swt.SqlServer)
                .WithSql("EXCEPTION: Full Join not supported", swt.MySql, swt.MariaDb, swt.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void CrossJoin()
        {
            var relation = ParseRelation("x CROSS JOIN y");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" CROSS JOIN \"y\"")
                .WithSql("[x] CROSS JOIN [y]", swt.SqlServer, swt.Sqlite)
                .WithSql("`x` CROSS JOIN `y`", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void ChainedJoin()
        {
            var relation = ParseRelation("x INNER JOIN y ON 1 = 2 LEFT JOIN z ON 3 = 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" INNER JOIN \"y\" ON 1 = 2 LEFT JOIN \"z\" ON 3 = 4")
                .WithSql("[x] INNER JOIN [y] ON 1 = 2 LEFT JOIN [z] ON 3 = 4", swt.SqlServer, swt.Sqlite)
                .WithSql("`x` INNER JOIN `y` ON 1 = 2 LEFT JOIN `z` ON 3 = 4", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void NestedJoin()
        {
            var relation = ParseRelation("x LEFT JOIN y INNER JOIN z ON 1 = 2 ON 3 = 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" LEFT JOIN \"y\" INNER JOIN \"z\" ON 1 = 2 ON 3 = 4")
                .WithSql("[x] LEFT JOIN [y] INNER JOIN [z] ON 1 = 2 ON 3 = 4", swt.SqlServer, swt.Sqlite)
                .WithSql("`x` LEFT JOIN `y` INNER JOIN `z` ON 1 = 2 ON 3 = 4", swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteChainedRightJoins()
        {
            var relation = ParseRelation("x RIGHT JOIN y ON 1 = 2 RIGHT JOIN z ON 3 = 4");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql("[z] LEFT JOIN ([y] LEFT JOIN [x] ON 1 = 2) ON 3 = 4", swt.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteSeparatedRightJoins()
        {
            var relation = ParseRelation("a RIGHT JOIN b ON 1 = 2 LEFT JOIN c ON 3 = 4 RIGHT JOIN d ON 5 = 6");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql("[d] LEFT JOIN ([b] LEFT JOIN [a] ON 1 = 2 LEFT JOIN [c] ON 3 = 4) ON 5 = 6", swt.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteRightJoinWithFollowingJoins()
        {
            var relation = ParseRelation("a LEFT JOIN b ON 1 = 2 RIGHT JOIN c ON 3 = 4 LEFT JOIN d ON 5 = 6");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql("[c] LEFT JOIN ([a] LEFT JOIN [b] ON 1 = 2) ON 3 = 4 LEFT JOIN [d] ON 5 = 6", swt.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteNestedRightJoins()
        {
            var relation = ParseRelation("a LEFT JOIN b RIGHT JOIN c ON 3 = 4 ON 1 = 2");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql("[a] LEFT JOIN [c] LEFT JOIN [b] ON 3 = 4 ON 1 = 2", swt.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }
    }
}
