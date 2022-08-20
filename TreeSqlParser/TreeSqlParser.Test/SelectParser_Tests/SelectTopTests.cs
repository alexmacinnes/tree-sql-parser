using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    [TestFixture]
    public class SelectTopTests
    {
        [Test]
        public void SelectTop() => SelectParserMother.TestParse("SELECT TOP 5 *");

        [Test]
        public void SelectTopBrackets() => SelectParserMother.TestParse("SELECT TOP (1+2) *");

        [Test]
        public void SelectTopPercent() => SelectParserMother.TestParse("SELECT TOP 5 PERCENT *");

        [Test]
        public void SelectTopWithTies() => SelectParserMother.TestParse("SELECT TOP 5 WITH TIES *");

        [Test]
        public void SelectTopPercentWithTies() => SelectParserMother.TestParse("SELECT TOP 5 PERCENT WITH TIES *");

        [Test]
        public void SelectTopVariableColumn() => SelectParserMother.TestParse("SELECT TOP @rowCount *");
    }
}
