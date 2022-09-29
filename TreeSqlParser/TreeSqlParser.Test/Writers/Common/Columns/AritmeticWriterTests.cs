using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class AritmeticWriterTests
    {
        private Column ParseColumn(string sql) =>
            (Column) SelectParser.ParseColumn(sql).Child;

        [Test]
        public void SimpleModulo()
        {
            var c = ParseColumn("1 % 2");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1%2")
                .WithSql(SqlWriterType.Oracle, "MOD(1, 2)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void SimpleModuloAtEnd()
        {
            var c = ParseColumn("1 + 2 % 3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1+2%3")
                .WithSql(SqlWriterType.Oracle, "1+MOD(2, 3)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ChainedModulo()
        {
            var c = ParseColumn("1 % 2 % 3 % 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1%2%3%4")
                .WithSql(SqlWriterType.Oracle, "MOD(MOD(MOD(1, 2), 3), 4)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void MultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 * 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1*2%3*4")
                .WithSql(SqlWriterType.Oracle, "MOD(1*2, 3)*4");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DoubleMultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 * 4 % 5");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1*2%3*4%5")
                .WithSql(SqlWriterType.Oracle, "MOD(MOD(1*2, 3)*4, 5)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DoubleSplitMultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 + 4 % 5");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1*2%3+4%5")
                .WithSql(SqlWriterType.Oracle, "MOD(1*2, 3)+MOD(4, 5)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void MultiplyChainedModuloWithManyFollowing()
        {
            var c = ParseColumn("1 * 2 % 3 * 4 + 5 * 6");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1*2%3*4+5*6")
                .WithSql(SqlWriterType.Oracle, "MOD(1*2, 3)*4+5*6");

            CommonMother.AssertSql(c, expected);
        }


        [Test]
        public void EmbeddedSingleModulo()
        {
            var c = ParseColumn("1 + 2 % 3 - 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1+2%3-4")
                .WithSql(SqlWriterType.Oracle, "1+MOD(2, 3)-4");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void EmbeddedChainedModulo()
        {
            var c = ParseColumn("1 + 2 % 3 % 4 - 5");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1+2%3%4-5")
                .WithSql(SqlWriterType.Oracle, "1+MOD(MOD(2, 3), 4)-5");

            CommonMother.AssertSql(c, expected);
        }
    }
}
