using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using swt = TreeSqlParser.Writers.SqlWriterType;

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
                .WithSql("MOD(1, 2)", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void SimpleModuloAtEnd()
        {
            var c = ParseColumn("1 + 2 % 3");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1+2%3")
                .WithSql("1+MOD(2, 3)", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ChainedModulo()
        {
            var c = ParseColumn("1 % 2 % 3 % 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1%2%3%4")
                .WithSql("MOD(MOD(MOD(1, 2), 3), 4)", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void MultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 * 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1*2%3*4")
                .WithSql("MOD(1*2, 3)*4", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DoubleMultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 * 4 % 5");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1*2%3*4%5")
                .WithSql("MOD(MOD(1*2, 3)*4, 5)", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DoubleSplitMultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 + 4 % 5");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1*2%3+4%5")
                .WithSql("MOD(1*2, 3)+MOD(4, 5)", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void MultiplyChainedModuloWithManyFollowing()
        {
            var c = ParseColumn("1 * 2 % 3 * 4 + 5 * 6");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1*2%3*4+5*6")
                .WithSql("MOD(1*2, 3)*4+5*6", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }


        [Test]
        public void EmbeddedSingleModulo()
        {
            var c = ParseColumn("1 + 2 % 3 - 4");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1+2%3-4")
                .WithSql("1+MOD(2, 3)-4", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void EmbeddedChainedModulo()
        {
            var c = ParseColumn("1 + 2 % 3 % 4 - 5");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1+2%3%4-5")
                .WithSql("1+MOD(MOD(2, 3), 4)-5", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }
    }
}
