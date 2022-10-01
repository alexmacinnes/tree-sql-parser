using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    public class SingleConditionTests
    {
        [Test]
        public void ParseEquals() => SelectParserMother.TestParse("SELECT 1 WHERE 1=2");

        [Test]
        public void ParseLessThan() => SelectParserMother.TestParse("SELECT 1 WHERE 1<2");

        [Test]
        public void ParseLessThanOrEqual() => SelectParserMother.TestParse("SELECT 1 WHERE 1<=2");

        [Test]
        public void ParseGreaterThan() => SelectParserMother.TestParse("SELECT 1 WHERE 1>2");

        [Test]
        public void ParseGreaterThanOrEqual() => SelectParserMother.TestParse("SELECT 1 WHERE 1>=2");

        [Test]
        public void ParseNotEquals() => SelectParserMother.TestParse("SELECT 1 WHERE 1<>2");

        [Test]
        public void ParseNot() => SelectParserMother.TestParse("SELECT 1 WHERE NOT 1=2");

        [Test]
        public void ParseBracket() => SelectParserMother.TestParse("SELECT 1 WHERE (1=2)");

        [Test]
        public void ParseNestedBracket() => SelectParserMother.TestParse("SELECT 1 WHERE ((1=2))");

        [Test]
        public void ParseBracketColumnInCondition() => SelectParserMother.TestParse("SELECT 1 WHERE (1)>2");

        [Test]
        public void ParseInList() => SelectParserMother.TestParse("SELECT 1 WHERE 1 IN (2,3)");

        [Test]
        public void ParseInSubselect() => SelectParserMother.TestParse("SELECT 1 WHERE 1 IN (SELECT 2)");

        [Test]
        public void ParseExists() => SelectParserMother.TestParse("SELECT 1 WHERE EXISTS (SELECT 2)");

        [Test]
        public void ParseBetween() => SelectParserMother.TestParse("SELECT 1 WHERE 1 BETWEEN 2 AND 3");

        [Test]
        public void ParseIsNull() => SelectParserMother.TestParse("SELECT 1 WHERE 1 IS NULL");

        [Test]
        public void ParseNotNull() => SelectParserMother.TestParse("SELECT 1 WHERE 1 IS NOT NULL");

        [Test]
        public void ParseSome() => SelectParserMother.TestParse("SELECT 1 WHERE a > SOME (SELECT x)");

        [Test]
        public void ParseAny() => SelectParserMother.TestParse("SELECT 1 WHERE a > ANY (SELECT x)", "SELECT 1 WHERE a > SOME (SELECT x)");

        [Test]
        public void ParseAll() => SelectParserMother.TestParse("SELECT 1 WHERE a > ALL (SELECT x)");

        [Test]
        public void ParseLike() => SelectParserMother.TestParse("SELECT 1 WHERE a LIKE '%foo%'");
    }
}
