using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    public class SelectOptionsTests
    {
        [Test]
        public void ParseSingleOptionHint() => SelectParserMother.TestParse("SELECT 1 OPTION (RECOMPILE)");

        [Test]
        public void ParseTwoOptionHints() => SelectParserMother.TestParse("SELECT 1 OPTION (RECOMPILE,HASH GROUP)");
    }
}
