using NUnit.Framework;
using System;
using System.Collections.Generic;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class AggregationWriterTests
    {
        private Column ParseColumn(string sql) =>
            SelectParser.ParseColumn(sql);

        private void AssertSql(string inputSql)
        {
            var column = ParseColumn(inputSql);
            foreach (var x in CommonMother.AllCommonWriters)
            {
                string outputSql = x.GenerateSql(column);
                Assert.AreEqual(inputSql, outputSql);
            }
        }

        private void AssertThrows(string inputSql)
        {
            var column = ParseColumn(inputSql);
            foreach (var x in CommonMother.AllCommonWriters)
            {
                Assert.Throws<InvalidOperationException>(
                    () => x.GenerateSql(column),
                    "");
            }
        }

        [TestCase("SUM(1)")]
        [TestCase("MAX(1)")]
        [TestCase("MIN(1)")]
        [TestCase("AVG(1)")]
        [TestCase("COUNT(1)")]
        [TestCase("COUNT(DISTINCT 1)")]
        [TestCase("COUNT(*)")]
        public void ValidAggregation(string sql)
        {
            AssertSql(sql);
        }

        [TestCase("STDEV(1)")]
        public void InvalidAggregationThrows(string sql)
        {
            AssertThrows(sql);
        }

    }
}
