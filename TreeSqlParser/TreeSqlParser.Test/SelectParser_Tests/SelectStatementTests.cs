using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    [TestFixture]
    public class SelectStatementTests
    {
        [Test]
        public void ParseUnion() => SelectParserMother.TestParse("SELECT 1 UNION SELECT 2");

        [Test]
        public void ParseUnionAll() => SelectParserMother.TestParse("SELECT 1 UNION ALL SELECT 2");

        [Test]
        public void ParseIntersect() => SelectParserMother.TestParse("SELECT 1 INTERSECT SELECT 2");

        [Test]
        public void ParseExcept() => SelectParserMother.TestParse("SELECT 1 EXCEPT SELECT 2");

        [Test]
        public void ParseMultiSet() => SelectParserMother.TestParse("SELECT 1 UNION SELECT 2 EXCEPT SELECT 1");

        [Test]
        public void ParseWith() => SelectParserMother.TestParse("WITH foo AS (SELECT 1) SELECT 2");

        [Test]
        public void ParseMultiWith() => SelectParserMother.TestParse("WITH foo AS (SELECT 1), bar AS (SELECT 2), baz AS (SELECT 3) SELECT 4");

        [Test]
        public void ParseUnionNestedInWith() => SelectParserMother.TestParse("WITH foo AS (SELECT 1 UNION SELECT 2) SELECT 3");

        [Test]
        public void ParseGroupBy() => SelectParserMother.TestParse("SELECT a, b, SUM(c) GROUP BY a, b");

        [Test]
        public void ParseHaving() => SelectParserMother.TestParse("SELECT 1 HAVING COUNT(*)>0");

        [Test]
        public void ParseGroupByAndHaving() => SelectParserMother.TestParse("SELECT a, SUM(b) GROUP BY a HAVING SUM(b)>100");
    }
}
