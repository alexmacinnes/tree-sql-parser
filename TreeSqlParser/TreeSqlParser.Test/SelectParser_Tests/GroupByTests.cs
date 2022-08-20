using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    [TestFixture]
    public class GroupByTests
    {
        [Test]
        public void ParseGroupBySingleColumn() => SelectParserMother.TestParse("SELECT 1 GROUP BY x");

        [Test]
        public void ParseGroupByColumns() => SelectParserMother.TestParse("SELECT 1 GROUP BY x, y");

        [Test]
        public void ParseGroupByRollup() => SelectParserMother.TestParse("SELECT 1 GROUP BY ROLLUP (x, y)");

        [Test]
        public void ParseGroupByCube() => SelectParserMother.TestParse("SELECT 1 GROUP BY CUBE (x, y)");

        [Test]
        public void ParseGroupByGroupingSets() => SelectParserMother.TestParse("SELECT 1 GROUP BY GROUPING SETS (CUBE (x, y), ROLLUP (a), (i, j, k))");

    }
}
