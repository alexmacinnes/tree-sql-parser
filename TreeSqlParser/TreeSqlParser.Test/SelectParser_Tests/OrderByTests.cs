using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Test.SelectParser_Tests
{   
    [TestFixture]
    public class OrderByTests
    {
        [Test]
        public void OrderByImplicitAsc() => SelectParserMother.TestParse("SELECT x ORDER BY 1", "SELECT x ORDER BY 1 ASC");

        [Test]
        public void OrderByExplicitAsc() => SelectParserMother.TestParse("SELECT x ORDER BY 1 ASC");

        [Test]
        public void OrderByExplicitDesc() => SelectParserMother.TestParse("SELECT x ORDER BY 1 DESC");

        [Test]
        public void OrderByCollateImplicitAsc() => SelectParserMother.TestParse("SELECT x ORDER BY 1 COLLATE foo", "SELECT x ORDER BY 1 COLLATE foo ASC");

        [Test]
        public void OrderByCollateExplicitDesc() => SelectParserMother.TestParse("SELECT x ORDER BY 1 COLLATE foo DESC");
        
        [Test]
        public void OrderByMultiColumn() => SelectParserMother.TestParse("SELECT x, y, z ORDER BY x ASC, 2 COLLATE foo DESC, 3 DESC");

        [Test]
        public void OrderByOffset() => SelectParserMother.TestParse("SELECT x ORDER BY 1 ASC OFFSET 2 ROWS");

        [Test]
        public void OrderByOffsetFetchNext() => SelectParserMother.TestParse("SELECT x ORDER BY 1 ASC OFFSET 2 ROWS FETCH NEXT 3 ROWS ONLY");

        [Test]
        public void OrderByOffsetFetchFirst() => SelectParserMother.TestParse("SELECT x ORDER BY 1 ASC OFFSET 2 ROWS FETCH FIRST 3 ROWS ONLY", "SELECT x ORDER BY 1 ASC OFFSET 2 ROWS FETCH NEXT 3 ROWS ONLY");

        [Test]
        public void OrderByOffsetFetchRow() => SelectParserMother.TestParse("SELECT x ORDER BY 1 ASC OFFSET 2 ROW FETCH NEXT 3 ROW ONLY", "SELECT x ORDER BY 1 ASC OFFSET 2 ROWS FETCH NEXT 3 ROWS ONLY");
    }
}
