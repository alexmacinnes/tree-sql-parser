using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;
using NUnit.Framework;
using System.Linq;
using TreeSqlParser.Parsing;
using TreeSqlParser.Writers.Full;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    internal class SelectParserMother
    {
        public static void TestParse(string sql, string expected = null)
        {
            var root = SelectParser.ParseSelectStatement(sql);

            AssertIntegrity(root);
            
            string actual = new FullSqlServerWriter().GenerateSql(root);
            expected ??= sql;

            Assert.AreEqual(expected, actual);

            var clone = (SqlRootElement) root.Clone();
            Assert.IsNull(clone.Parent);
            AssertIntegrity(clone);

            var elementsInCommon = root.Flatten().Intersect(clone.Flatten());
            Assert.IsEmpty(elementsInCommon);
        }


        public static void TestParseThrows(string sql, string expectedMsg)
        {
            var ex = Assert.Throws<InvalidOperationException>(() => TestParse(sql));
            Assert.AreEqual(expectedMsg, ex.Message);
        }

        public static void AssertIntegrity(SqlElement s)
        {
            var elements = s.Flatten().ToArray();
            
            Assert.Null(elements[0].Parent);

            foreach (var x in elements.Skip(1))
                Assert.True(x.Parent.Children.Contains(x));

            var allParents1 = new HashSet<SqlElement> (elements.Select(x => x.Parent).Where(x => x != null));
            var allParents2 = new HashSet<SqlElement>(elements.Where(x => x.Children.Any()));
            CollectionAssert.AreEquivalent(allParents1, allParents2);
        }
            
    }
}
