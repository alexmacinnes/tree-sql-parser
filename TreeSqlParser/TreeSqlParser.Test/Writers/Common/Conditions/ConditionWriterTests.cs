using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Writers.Safe;

namespace TreeSqlParser.Writers.Test.Safe.Conditions
{
    public class ConditionWriterTests
    {
        private readonly IReadOnlyDictionary<DbFamily, ISqlWriter> writers = new Dictionary<DbFamily, ISqlWriter>
        {
            { DbFamily.SqlServer, new SafeSqlServerWriter() },
            { DbFamily.Oracle, new SafeOracleSqlWriter() }
        };

        private string Sql(Condition c, DbFamily db) => writers[db].GenerateSql(c);

        private Condition ParseCondition(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select 1 where " + sql).Child).Selects[0].WhereCondition;

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

            foreach (var x in writers.Values)
            {
                string generatedSql = x.GenerateSql(condition);
                Assert.AreEqual(sql, generatedSql);
            }
        }

        [Test]
        public void InSubselectCondition()
        {
            var condition = ParseCondition("1 IN (SELECT 2)");

            Assert.AreEqual("1 IN (SELECT 2)", Sql(condition, DbFamily.SqlServer));
            Assert.AreEqual("1 IN (SELECT 2 FROM dual)", Sql(condition, DbFamily.Oracle));
        }

        [Test]
        public void ExistsCondition()
        {
            var condition = ParseCondition("EXISTS (SELECT 1)");

            Assert.AreEqual("EXISTS (SELECT 1)", Sql(condition, DbFamily.SqlServer));
            Assert.AreEqual("EXISTS (SELECT 1 FROM dual)", Sql(condition, DbFamily.Oracle));
        }
    }
}
