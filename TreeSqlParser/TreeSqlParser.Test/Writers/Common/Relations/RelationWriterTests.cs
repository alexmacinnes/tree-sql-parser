using NUnit.Framework;
using TreeSqlParser.Model.Relations;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Relations
{
    public class RelationWriterTests
    { 
        private Relation ParseRelation(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select * from " + sql).Child).Selects[0].From[0];

        [Test]
        public void SingleTable()
        {
            var relation = ParseRelation("x");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\"")
                .WithSql("[x]", SqlWriterType.SqlServer)
                .WithSql("`x`", SqlWriterType.MySql)
                .WithSql("`x`", SqlWriterType.MariaDb)
                .WithSql("[x]", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void TwoPartTable()
        {
            var relation = ParseRelation("x.y");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\".\"y\"")
                .WithSql("[x].[y]", SqlWriterType.SqlServer)
                .WithSql("`x`.`y`", SqlWriterType.MySql)
                .WithSql("`x`.`y`", SqlWriterType.MariaDb)
                .WithSql("[x].[y]", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void BracketRelation()
        {
            var relation = ParseRelation("(x)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("(\"x\")")
                .WithSql("([x])", SqlWriterType.SqlServer)
                .WithSql("(`x`)", SqlWriterType.MySql)
                .WithSql("(`x`)", SqlWriterType.MariaDb)
                .WithSql("([x])", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void SubselectRelation()
        {
            var relation = ParseRelation("(select * from x) AS foo");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("(SELECT * FROM \"x\") AS \"foo\"")
                .WithSql("(SELECT * FROM [x]) AS [foo]", SqlWriterType.SqlServer)
                .WithSql("(SELECT * FROM `x`) AS `foo`", SqlWriterType.MySql)
                .WithSql("(SELECT * FROM `x`) AS `foo`", SqlWriterType.MariaDb)
                .WithSql("(SELECT * FROM [x]) AS [foo]", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void InnerJoin()
        {
            var relation = ParseRelation("x INNER JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" INNER JOIN \"y\" ON 1 = 2")
                .WithSql("[x] INNER JOIN [y] ON 1 = 2", SqlWriterType.SqlServer)
                .WithSql("`x` INNER JOIN `y` ON 1 = 2", SqlWriterType.MySql)
                .WithSql("`x` INNER JOIN `y` ON 1 = 2", SqlWriterType.MariaDb)
                .WithSql("[x] INNER JOIN [y] ON 1 = 2", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void LeftJoin()
        {
            var relation = ParseRelation("x LEFT JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" LEFT JOIN \"y\" ON 1 = 2")
                .WithSql("[x] LEFT JOIN [y] ON 1 = 2", SqlWriterType.SqlServer)
                .WithSql("`x` LEFT JOIN `y` ON 1 = 2", SqlWriterType.MySql)
                .WithSql("`x` LEFT JOIN `y` ON 1 = 2", SqlWriterType.MariaDb)
                .WithSql("[x] LEFT JOIN [y] ON 1 = 2", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RightJoin()
        {
            var relation = ParseRelation("x RIGHT JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" RIGHT JOIN \"y\" ON 1 = 2")
                .WithSql("[x] RIGHT JOIN [y] ON 1 = 2", SqlWriterType.SqlServer)
                .WithSql("`x` RIGHT JOIN `y` ON 1 = 2", SqlWriterType.MySql)
                .WithSql("`x` RIGHT JOIN `y` ON 1 = 2", SqlWriterType.MariaDb)
                .WithSql("[y] LEFT JOIN [x] ON 1 = 2", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void FullJoin()
        {
            var relation = ParseRelation("x FULL JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" FULL JOIN \"y\" ON 1 = 2")
                .WithSql("[x] FULL JOIN [y] ON 1 = 2", SqlWriterType.SqlServer)
                .WithSql("EXCEPTION: Full Join not supported", SqlWriterType.MySql)
                .WithSql("EXCEPTION: Full Join not supported", SqlWriterType.MariaDb)
                .WithSql("EXCEPTION: Full Join not supported", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void CrossJoin()
        {
            var relation = ParseRelation("x CROSS JOIN y");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" CROSS JOIN \"y\"")
                .WithSql("[x] CROSS JOIN [y]", SqlWriterType.SqlServer)
                .WithSql("`x` CROSS JOIN `y`", SqlWriterType.MySql)
                .WithSql("`x` CROSS JOIN `y`", SqlWriterType.MariaDb)
                .WithSql("[x] CROSS JOIN [y]", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void ChainedJoin()
        {
            var relation = ParseRelation("x INNER JOIN y ON 1 = 2 LEFT JOIN z ON 3 = 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" INNER JOIN \"y\" ON 1 = 2 LEFT JOIN \"z\" ON 3 = 4")
                .WithSql("[x] INNER JOIN [y] ON 1 = 2 LEFT JOIN [z] ON 3 = 4", SqlWriterType.SqlServer)
                .WithSql("`x` INNER JOIN `y` ON 1 = 2 LEFT JOIN `z` ON 3 = 4", SqlWriterType.MySql)
                .WithSql("`x` INNER JOIN `y` ON 1 = 2 LEFT JOIN `z` ON 3 = 4", SqlWriterType.MariaDb)
                .WithSql("[x] INNER JOIN [y] ON 1 = 2 LEFT JOIN [z] ON 3 = 4", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void NestedJoin()
        {
            var relation = ParseRelation("x LEFT JOIN y INNER JOIN z ON 1 = 2 ON 3 = 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" LEFT JOIN \"y\" INNER JOIN \"z\" ON 1 = 2 ON 3 = 4")
                .WithSql("[x] LEFT JOIN [y] INNER JOIN [z] ON 1 = 2 ON 3 = 4", SqlWriterType.SqlServer)
                .WithSql("`x` LEFT JOIN `y` INNER JOIN `z` ON 1 = 2 ON 3 = 4", SqlWriterType.MySql)
                .WithSql("`x` LEFT JOIN `y` INNER JOIN `z` ON 1 = 2 ON 3 = 4", SqlWriterType.MariaDb)
                .WithSql("[x] LEFT JOIN [y] INNER JOIN [z] ON 1 = 2 ON 3 = 4", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteChainedRightJoins()
        {
            var relation = ParseRelation("x RIGHT JOIN y ON 1 = 2 RIGHT JOIN z ON 3 = 4");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql("[z] LEFT JOIN ([y] LEFT JOIN [x] ON 1 = 2) ON 3 = 4", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteSeparatedRightJoins()
        {
            var relation = ParseRelation("a RIGHT JOIN b ON 1 = 2 LEFT JOIN c ON 3 = 4 RIGHT JOIN d ON 5 = 6");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql("[d] LEFT JOIN ([b] LEFT JOIN [a] ON 1 = 2 LEFT JOIN [c] ON 3 = 4) ON 5 = 6", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteRightJoinWithFollowingJoins()
        {
            var relation = ParseRelation("a LEFT JOIN b ON 1 = 2 RIGHT JOIN c ON 3 = 4 LEFT JOIN d ON 5 = 6");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql("[c] LEFT JOIN ([a] LEFT JOIN [b] ON 1 = 2) ON 3 = 4 LEFT JOIN [d] ON 5 = 6", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteNestedRightJoins()
        {
            var relation = ParseRelation("a LEFT JOIN b RIGHT JOIN c ON 3 = 4 ON 1 = 2");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql("[a] LEFT JOIN [c] LEFT JOIN [b] ON 3 = 4 ON 1 = 2", SqlWriterType.Sqlite);

            CommonMother.AssertSql(relation, expected);
        }
    }
}
