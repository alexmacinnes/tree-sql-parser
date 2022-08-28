using NUnit.Framework;
using System;
using System.Collections.Generic;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class AggregationWriterTests
    {
        private readonly IReadOnlyDictionary<DbFamily, ISqlWriter> writers = new Dictionary<DbFamily, ISqlWriter>
        {
            { DbFamily.SqlServer, new CommonSqlServerSqlWriter() },
            { DbFamily.Oracle, new CommonOracleSqlWriter() }
        };

        private Column ParseColumn(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select " + sql).Child).Selects[0].Columns[0];

        private void AssertSql(string inputSql)
        {
            var column = ParseColumn(inputSql);
            foreach (var x in writers.Values)
            {
                string outputSql = x.GenerateSql(column);
                Assert.AreEqual(inputSql, outputSql);
            }
        }

        private void AssertThrows(string inputSql)
        {
            var column = ParseColumn(inputSql);
            foreach (var x in writers.Values)
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
