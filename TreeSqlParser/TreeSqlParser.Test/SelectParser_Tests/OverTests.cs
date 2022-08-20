using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    [TestFixture]
    public class OverTests
    {
        [Test]
        public void ParseEmptyOver() => SelectParserMother.TestParse("SELECT 1 OVER()");

        [Test]
        public void ParseOverPartitionBy() => SelectParserMother.TestParse("SELECT 1 OVER(PARTITION BY a, b)");

        [Test]
        public void ParseOverOrderBy() => SelectParserMother.TestParse("SELECT 1 OVER(ORDER BY a COLLATE foo ASC, b DESC)");

        [Test]
        public void ParseOverUnboundedPreceding() => SelectParserMother.TestParse("SELECT 1 OVER(ROWS UNBOUNDED PRECEDING)", "SELECT 1 OVER(ROWS null TO 0)");

        [Test]
        public void ParseOverCurrentRow() => SelectParserMother.TestParse("SELECT 1 OVER(ROWS CURRENT ROW)", "SELECT 1 OVER(ROWS 0 TO 0)");

        [Test]
        public void ParseOverNRowsPreceding() => SelectParserMother.TestParse("SELECT 1 OVER(ROWS 99 PRECEDING)", "SELECT 1 OVER(ROWS -99 TO 0)");

        [Test]
        public void ParseOverBetweenCurrentRowAndUnbounded() => SelectParserMother.TestParse("SELECT 1 OVER(ROWS BETWEEN CURRENT ROW AND UNBOUNDED FOLLOWING)", "SELECT 1 OVER(ROWS 0 TO null)");

        [Test]
        public void ParseOverBetweenCurrentRowAndCurrentRow() => SelectParserMother.TestParse("SELECT 1 OVER(ROWS BETWEEN CURRENT ROW AND CURRENT ROW)", "SELECT 1 OVER(ROWS 0 TO 0)");

        [Test]
        public void ParseOverBetweenCurrentRowAndNFollowing() => SelectParserMother.TestParse("SELECT 1 OVER(ROWS BETWEEN CURRENT ROW AND 99 FOLLOWING)", "SELECT 1 OVER(ROWS 0 TO 99)");

        [Test]
        public void ParseOverBetweenNPrecedingAndMFollowing() => SelectParserMother.TestParse("SELECT 1 OVER(ROWS BETWEEN 3 PRECEDING AND 4 FOLLOWING)", "SELECT 1 OVER(ROWS -3 TO 4)");

        [Test]
        public void ParseOverBetweenNPrecedingAndMPreceding() => SelectParserMother.TestParse("SELECT 1 OVER(ROWS BETWEEN 3 PRECEDING AND 1 PRECEDING)", "SELECT 1 OVER(ROWS -3 TO -1)");

        [Test]
        public void ParseOverBetweenNFollowingAndMFollowing() => SelectParserMother.TestParse("SELECT 1 OVER(ROWS BETWEEN 1 FOLLOWING AND 3 FOLLOWING)", "SELECT 1 OVER(ROWS 1 TO 3)");

        [Test]
        public void ParseOverRange() => SelectParserMother.TestParse("SELECT 1 OVER(RANGE BETWEEN CURRENT ROW AND 99 FOLLOWING)", "SELECT 1 OVER(RANGE 0 TO 99)");

        [Test]
        public void ParseCompleteOver() => SelectParserMother.TestParse("SELECT 1 OVER(PARTITION BY x, y ORDER BY z ASC ROWS UNBOUNDED PRECEDING)", "SELECT 1 OVER(PARTITION BY x, y ORDER BY z ASC ROWS null TO 0)");
    }
}
