using NUnit.Framework;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    public class ZFooTests
    {
        [Test]
        public void Foo() => SelectParserMother.TestParse("SELECT {d '2000-12-31'}");

        [Test]
        public void Bool() => SelectParserMother.TestParse("SELECT true", "SELECT 1");



    }
}
