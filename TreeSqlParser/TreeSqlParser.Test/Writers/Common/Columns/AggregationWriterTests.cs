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
            (Column) SelectParser.ParseColumn(sql).Child;

        [TestCase("SUM(1)")]
        [TestCase("MAX(1)")]
        [TestCase("MIN(1)")]
        [TestCase("AVG(1)")]
        [TestCase("COUNT(1)")]
        [TestCase("COUNT(DISTINCT 1)")]
        [TestCase("COUNT(*)")]
        public void ValidAggregation(string sql)
        {
            var c = ParseColumn(sql);

            var expected = new ExpectedSqlResult()
                .WithDefaultSql(sql);

            CommonMother.AssertSql(c, expected);
        }

        [TestCase("STDEV(1)")]
        public void InvalidAggregationThrows(string sql)
        {
            var c = ParseColumn(sql);

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("EXCEPTION: Limited Aggregation Writer does not support Stdev");

            CommonMother.AssertSql(c, expected);
        }

    }
}
