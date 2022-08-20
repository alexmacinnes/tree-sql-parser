using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    [TestFixture]
    public class PivotTests
    {
        [Test]
        public void ParsePivot() => SelectParserMother.TestParse("SELECT 1 PIVOT (MAX(x) FOR y IN (abc, def, ghi)) AS fooPivot");
    }
}
