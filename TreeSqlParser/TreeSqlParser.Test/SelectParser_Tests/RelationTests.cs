using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    [TestFixture]
    public class RelationTests
    {
        [Test]
        public void ParseSimpleTable() => SelectParserMother.TestParse("SELECT * FROM x");

        [Test]
        public void ParseMultipartTable() => SelectParserMother.TestParse("SELECT * FROM x.[y y].z");

        [Test]
        public void ParseMultipartTableWithAlias() => SelectParserMother.TestParse("SELECT * FROM x.[y y].z AS [foo bar]");

        [Test]
        public void ParseMultipartTableWithAliasWithoutAs() => SelectParserMother.TestParse("SELECT * FROM x.[y y].z [foo bar]", "SELECT * FROM x.[y y].z AS [foo bar]");

        [Test]
        public void ParseBracketedTable() => SelectParserMother.TestParse("SELECT * FROM ((x))");

        [Test]
        public void ParseMutiTableLists() => SelectParserMother.TestParse("SELECT * FROM x, y, z");

        [Test]
        public void ParseSubselectTable() => SelectParserMother.TestParse("SELECT * FROM (SELECT 1) AS foo");

        [Test]
        public void ParseInnerJoin() => SelectParserMother.TestParse("SELECT * FROM a INNER JOIN b ON 1=2");

        [Test]
        public void ParseDefaultInnerJoin() => SelectParserMother.TestParse("SELECT * FROM a JOIN b ON 1=2", "SELECT * FROM a INNER JOIN b ON 1=2");

        [Test]
        public void ParseLeftJoin() => SelectParserMother.TestParse("SELECT * FROM a LEFT JOIN b ON 1=2");

        [Test]
        public void ParseLeftOuterJoin() => SelectParserMother.TestParse("SELECT * FROM a LEFT OUTER JOIN b ON 1=2", "SELECT * FROM a LEFT JOIN b ON 1=2");

        [Test]
        public void ParseRightJoin() => SelectParserMother.TestParse("SELECT * FROM a RIGHT JOIN b ON 1=2");

        [Test]
        public void ParseRightOuterJoin() => SelectParserMother.TestParse("SELECT * FROM a RIGHT OUTER JOIN b ON 1=2", "SELECT * FROM a RIGHT JOIN b ON 1=2");

        [Test]
        public void ParseFullJoin() => SelectParserMother.TestParse("SELECT * FROM a FULL JOIN b ON 1=2");

        [Test]
        public void ParseFullOuterJoin() => SelectParserMother.TestParse("SELECT * FROM a FULL OUTER JOIN b ON 1=2", "SELECT * FROM a FULL JOIN b ON 1=2");

        [Test]
        public void ParseCrossJoin() => SelectParserMother.TestParse("SELECT * FROM a CROSS JOIN b");

        [Test]
        public void ParseCrossApply() => SelectParserMother.TestParse("SELECT * FROM a CROSS APPLY b");

        [Test]
        public void ParseOuterApply() => SelectParserMother.TestParse("SELECT * FROM a OUTER APPLY b");

        [Test]
        public void ParseThreeTableJoin() => SelectParserMother.TestParse("SELECT * FROM a INNER JOIN b ON 1=2 LEFT JOIN c ON 3=4");

        [Test]
        public void ParseNestedJoin() => SelectParserMother.TestParse("SELECT * FROM a LEFT JOIN b INNER JOIN c ON b.x=c.x ON a.y=b.y");

        [Test]
        public void ParseBracketedNestedJoin() => SelectParserMother.TestParse("SELECT * FROM a LEFT JOIN (b INNER JOIN c ON b.x=c.x) ON a.y=b.y");
    }
}
