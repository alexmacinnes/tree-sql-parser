using NUnit.Framework;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Conditions
{
    public class ConditionWriterTests
    {
        private string Sql(Condition c, SqlWriterType db) => CommonMother.Sql(c, db);

        private Condition ParseCondition(string sql) =>
            SelectParser.ParseCondition(sql);

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

            foreach (var x in CommonMother.AllCommonWriters)
            {
                string generatedSql = x.GenerateSql(condition);
                Assert.AreEqual(sql, generatedSql);
            }
        }

        [Test]
        public void InSubselectCondition()
        {
            var condition = ParseCondition("1 IN (SELECT 2)");

            Assert.AreEqual("1 IN (SELECT 2)", Sql(condition, SqlWriterType.SqlServer));
            Assert.AreEqual("1 IN (SELECT 2 FROM dual)", Sql(condition, SqlWriterType.Oracle));
            Assert.AreEqual("1 IN (SELECT 2)", Sql(condition, SqlWriterType.MySql));
            Assert.AreEqual("1 IN (SELECT 2)", Sql(condition, SqlWriterType.Sqlite));
            Assert.AreEqual("1 IN (SELECT 2)", Sql(condition, SqlWriterType.Postgres));
        }

        [Test]
        public void ExistsCondition()
        {
            var condition = ParseCondition("EXISTS (SELECT 1)");

            Assert.AreEqual("EXISTS (SELECT 1)", Sql(condition, SqlWriterType.SqlServer));
            Assert.AreEqual("EXISTS (SELECT 1 FROM dual)", Sql(condition, SqlWriterType.Oracle));
            Assert.AreEqual("EXISTS (SELECT 1)", Sql(condition, SqlWriterType.MySql));
            Assert.AreEqual("EXISTS (SELECT 1)", Sql(condition, SqlWriterType.Sqlite));
            Assert.AreEqual("EXISTS (SELECT 1)", Sql(condition, SqlWriterType.Postgres));
        }
    }
}
