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
                .WithSql(SqlWriterType.SqlServer, "[x]")
                .WithSql(SqlWriterType.MySql, "`x`")
                .WithSql(SqlWriterType.MariaDb, "`x`")
                .WithSql(SqlWriterType.Sqlite, "[x]");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void TwoPartTable()
        {
            var relation = ParseRelation("x.y");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\".\"y\"")
                .WithSql(SqlWriterType.SqlServer, "[x].[y]")
                .WithSql(SqlWriterType.MySql, "`x`.`y`")
                .WithSql(SqlWriterType.MariaDb, "`x`.`y`")
                .WithSql(SqlWriterType.Sqlite, "[x].[y]");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void BracketRelation()
        {
            var relation = ParseRelation("(x)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("(\"x\")")
                .WithSql(SqlWriterType.SqlServer, "([x])")
                .WithSql(SqlWriterType.MySql, "(`x`)")
                .WithSql(SqlWriterType.MariaDb, "(`x`)")
                .WithSql(SqlWriterType.Sqlite, "([x])");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void SubselectRelation()
        {
            var relation = ParseRelation("(select * from x) AS foo");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("(SELECT * FROM \"x\") AS \"foo\"")
                .WithSql(SqlWriterType.SqlServer, "(SELECT * FROM [x]) AS [foo]")
                .WithSql(SqlWriterType.MySql, "(SELECT * FROM `x`) AS `foo`")
                .WithSql(SqlWriterType.MariaDb, "(SELECT * FROM `x`) AS `foo`")
                .WithSql(SqlWriterType.Sqlite, "(SELECT * FROM [x]) AS [foo]");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void InnerJoin()
        {
            var relation = ParseRelation("x INNER JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" INNER JOIN \"y\" ON 1 = 2")
                .WithSql(SqlWriterType.SqlServer, "[x] INNER JOIN [y] ON 1 = 2")
                .WithSql(SqlWriterType.MySql, "`x` INNER JOIN `y` ON 1 = 2")
                .WithSql(SqlWriterType.MariaDb, "`x` INNER JOIN `y` ON 1 = 2")
                .WithSql(SqlWriterType.Sqlite, "[x] INNER JOIN [y] ON 1 = 2");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void LeftJoin()
        {
            var relation = ParseRelation("x LEFT JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" LEFT JOIN \"y\" ON 1 = 2")
                .WithSql(SqlWriterType.SqlServer, "[x] LEFT JOIN [y] ON 1 = 2")
                .WithSql(SqlWriterType.MySql, "`x` LEFT JOIN `y` ON 1 = 2")
                .WithSql(SqlWriterType.MariaDb, "`x` LEFT JOIN `y` ON 1 = 2")
                .WithSql(SqlWriterType.Sqlite, "[x] LEFT JOIN [y] ON 1 = 2");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RightJoin()
        {
            var relation = ParseRelation("x RIGHT JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" RIGHT JOIN \"y\" ON 1 = 2")
                .WithSql(SqlWriterType.SqlServer, "[x] RIGHT JOIN [y] ON 1 = 2")
                .WithSql(SqlWriterType.MySql, "`x` RIGHT JOIN `y` ON 1 = 2")
                .WithSql(SqlWriterType.MariaDb, "`x` RIGHT JOIN `y` ON 1 = 2")
                .WithSql(SqlWriterType.Sqlite, "[y] LEFT JOIN [x] ON 1 = 2");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void FullJoin()
        {
            var relation = ParseRelation("x FULL JOIN y ON 1=2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" FULL JOIN \"y\" ON 1 = 2")
                .WithSql(SqlWriterType.SqlServer, "[x] FULL JOIN [y] ON 1 = 2")
                .WithSql(SqlWriterType.MySql, "EXCEPTION: Full Join not supported")
                .WithSql(SqlWriterType.MariaDb, "EXCEPTION: Full Join not supported")
                .WithSql(SqlWriterType.Sqlite, "EXCEPTION: Full Join not supported");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void CrossJoin()
        {
            var relation = ParseRelation("x CROSS JOIN y");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" CROSS JOIN \"y\"")
                .WithSql(SqlWriterType.SqlServer, "[x] CROSS JOIN [y]")
                .WithSql(SqlWriterType.MySql, "`x` CROSS JOIN `y`")
                .WithSql(SqlWriterType.MariaDb, "`x` CROSS JOIN `y`")
                .WithSql(SqlWriterType.Sqlite, "[x] CROSS JOIN [y]");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void ChainedJoin()
        {
            var relation = ParseRelation("x INNER JOIN y ON 1 = 2 LEFT JOIN z ON 3 = 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" INNER JOIN \"y\" ON 1 = 2 LEFT JOIN \"z\" ON 3 = 4")
                .WithSql(SqlWriterType.SqlServer, "[x] INNER JOIN [y] ON 1 = 2 LEFT JOIN [z] ON 3 = 4")
                .WithSql(SqlWriterType.MySql, "`x` INNER JOIN `y` ON 1 = 2 LEFT JOIN `z` ON 3 = 4")
                .WithSql(SqlWriterType.MariaDb, "`x` INNER JOIN `y` ON 1 = 2 LEFT JOIN `z` ON 3 = 4")
                .WithSql(SqlWriterType.Sqlite, "[x] INNER JOIN [y] ON 1 = 2 LEFT JOIN [z] ON 3 = 4");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void NestedJoin()
        {
            var relation = ParseRelation("x LEFT JOIN y INNER JOIN z ON 1 = 2 ON 3 = 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("\"x\" LEFT JOIN \"y\" INNER JOIN \"z\" ON 1 = 2 ON 3 = 4")
                .WithSql(SqlWriterType.SqlServer, "[x] LEFT JOIN [y] INNER JOIN [z] ON 1 = 2 ON 3 = 4")
                .WithSql(SqlWriterType.MySql, "`x` LEFT JOIN `y` INNER JOIN `z` ON 1 = 2 ON 3 = 4")
                .WithSql(SqlWriterType.MariaDb, "`x` LEFT JOIN `y` INNER JOIN `z` ON 1 = 2 ON 3 = 4")
                .WithSql(SqlWriterType.Sqlite, "[x] LEFT JOIN [y] INNER JOIN [z] ON 1 = 2 ON 3 = 4");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteChainedRightJoins()
        {
            var relation = ParseRelation("x RIGHT JOIN y ON 1 = 2 RIGHT JOIN z ON 3 = 4");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql(SqlWriterType.Sqlite, "[z] LEFT JOIN ([y] LEFT JOIN [x] ON 1 = 2) ON 3 = 4");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteSeparatedRightJoins()
        {
            var relation = ParseRelation("a RIGHT JOIN b ON 1 = 2 LEFT JOIN c ON 3 = 4 RIGHT JOIN d ON 5 = 6");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql(SqlWriterType.Sqlite, "[d] LEFT JOIN ([b] LEFT JOIN [a] ON 1 = 2 LEFT JOIN [c] ON 3 = 4) ON 5 = 6");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteRightJoinWithFollowingJoins()
        {
            var relation = ParseRelation("a LEFT JOIN b ON 1 = 2 RIGHT JOIN c ON 3 = 4 LEFT JOIN d ON 5 = 6");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql(SqlWriterType.Sqlite, "[c] LEFT JOIN ([a] LEFT JOIN [b] ON 1 = 2) ON 3 = 4 LEFT JOIN [d] ON 5 = 6");

            CommonMother.AssertSql(relation, expected);
        }

        [Test]
        public void RewriteNestedRightJoins()
        {
            var relation = ParseRelation("a LEFT JOIN b RIGHT JOIN c ON 3 = 4 ON 1 = 2");

            var expected = new ExpectedSqlResult(skipUnlistedConversions: true)
                .WithSql(SqlWriterType.Sqlite, "[a] LEFT JOIN [c] LEFT JOIN [b] ON 3 = 4 ON 1 = 2");

            CommonMother.AssertSql(relation, expected);
        }
    }
}
