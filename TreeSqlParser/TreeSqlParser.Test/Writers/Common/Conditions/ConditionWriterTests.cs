using NUnit.Framework;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using swt = TreeSqlParser.Writers.SqlWriterType;

namespace TreeSqlParser.Writers.Test.Common.Conditions
{
    public class ConditionWriterTests
    {
        private static SelectParser SelectParser = new SelectParser();

        private Condition ParseCondition(string sql) =>
            (Condition) SelectParser.ParseCondition(sql).Child;

        [TestCase("1 = 2")]
        [TestCase("1 < 2")]
        [TestCase("1 <= 2")]
        [TestCase("1 > 2")]
        [TestCase("1 >= 2")]
        [TestCase("1 <> 2")]
        [TestCase("1 IS NULL")]
        [TestCase("1 IS NOT NULL")]
        [TestCase("'abc' LIKE '%b%'")]
        [TestCase("1 = 2 AND 3 = 4 OR 5 = 6")]
        [TestCase("NOT 1 = 2")]
        [TestCase("(1 = 2)")]
        [TestCase("1 IN (2, 3)")]
        [TestCase("1 BETWEEN 2 AND 3")]
        public void SqlSameOnAllDbs(string sql)
        {
            var condition = ParseCondition(sql);

            var expected = new ExpectedSqlResult()
                .WithDefaultSql(sql);

            CommonMother.AssertSql(condition, expected);
        }

        [Test]
        public void InSubselectCondition()
        {
            var condition = ParseCondition("1 IN (SELECT 2)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("1 IN (SELECT 2)")
                .WithSql("1 IN (SELECT 2 FROM dual)", swt.Oracle);

            CommonMother.AssertSql(condition, expected);
        }

        [Test]
        public void ExistsCondition()
        {
            var condition = ParseCondition("EXISTS (SELECT 1)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("EXISTS (SELECT 1)")
                .WithSql("EXISTS (SELECT 1 FROM dual)", swt.Oracle);

            CommonMother.AssertSql(condition, expected);
        }
    }
}
