using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    public class MultiConditionTests
    {
        [Test]
        public void ParseSingleAnd() => SelectParserMother.TestParse("SELECT * WHERE 1=2 AND 3=4");

        [Test]
        public void ParseSingleOr() => SelectParserMother.TestParse("SELECT * WHERE 1=2 OR 3=4");

        [Test]
        public void ParseMultiAnd() => SelectParserMother.TestParse("SELECT * WHERE 1=2 AND 3=4 AND 5=6");

        [Test]
        public void ParseMultiOr() => SelectParserMother.TestParse("SELECT * WHERE 1=2 OR 3=4 OR 5=6");

        [Test]
        public void ParseMultiAndOr() => SelectParserMother.TestParse("SELECT * WHERE 1=1 AND 2=2 AND 3=3 OR 4=4 OR 5=5 OR 6=6 AND 7=7");

        [Test]
        public void ParseMultiWithNestedNot() => SelectParserMother.TestParse("SELECT * WHERE 1=1 OR NOT 2=2 AND 3=3");

        [Test]
        public void ParseMultiWithNestedBracket() => SelectParserMother.TestParse("SELECT * WHERE 1=1 AND (2=2 OR 3=3) AND 4=4");
    }
}
