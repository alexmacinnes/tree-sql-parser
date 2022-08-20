using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;

namespace TreeSqlParser.Test.Model.SqlElementConventions_Tests
{
    public class CloneTests
    {
        public void AssertIntegrity(SqlElement original, SqlElement cloned)
        {
            Assert.IsNull(cloned.Parent);

            var originalElements = original.Flatten();
            var clonedElements = cloned.Flatten();

            var common = originalElements.Intersect(clonedElements);
            Assert.IsEmpty(common);

            foreach (var x in clonedElements)
                foreach (var c in x.Children)
                    Assert.AreEqual(x, c.Parent);
        }

        [Test]
        public void CloneSimpleElement()
        {
            var original = new FooSimpleSqlElement("abc");
            var cloned = (FooSimpleSqlElement)original.Clone();

            AssertIntegrity(original, cloned);
            Assert.AreEqual("abc", cloned.Name);
        }

        [Test]
        public void CloneDerivedElement()
        {
            var original = new DerivedSimpleSqlElement("abc");
            var cloned = (DerivedSimpleSqlElement)original.Clone();

            AssertIntegrity(original, cloned);
            Assert.AreEqual("abc", cloned.Name);
        }

        [Test]
        public void CloneComplexElement()
        {
            var original = new FooComplexSqlElement
            {
                Element1 = null,
                Element2 = new FooSimpleSqlElement("a"),
                List1 = null,
                List2 = new List<SqlElement>
                {
                    null,
                    new FooSimpleSqlElement("b"),
                    new FooSimpleSqlElement("c")
                }
            };
            var cloned = (FooComplexSqlElement)original.Clone();

            AssertIntegrity(original, cloned);

            dynamic dyn = cloned;
            Assert.IsNull(dyn.Element1);
            Assert.AreEqual("a", dyn.Element2.Name);
            Assert.IsNull(dyn.List1);
            Assert.IsNull(dyn.List2[0]);
            Assert.AreEqual("b", dyn.List2[1].Name);
            Assert.AreEqual("c", dyn.List2[2].Name);
        }

        [Test]
        public void CloneDerivedComplexElement()
        {
            var original = new DerivedComplexSqlElement
            {
                Element1 = null,
                Element2 = new FooSimpleSqlElement("a"),
                List1 = null,
                List2 = new List<SqlElement>
                {
                    null,
                    new FooSimpleSqlElement("b"),
                    new FooSimpleSqlElement("c")
                }
            };
            var cloned = (DerivedComplexSqlElement)original.Clone();

            AssertIntegrity(original, cloned);

            dynamic dyn = cloned;
            Assert.IsNull(dyn.Element1);
            Assert.AreEqual("a", dyn.Element2.Name);
            Assert.IsNull(dyn.List1);
            Assert.IsNull(dyn.List2[0]);
            Assert.AreEqual("b", dyn.List2[1].Name);
            Assert.AreEqual("c", dyn.List2[2].Name);
        }

        [Test]
        public void CloneStringMembersElement()
        {
            var original = new StringMembersSqlElement
            {
                String1 = "a",
                List1 = new List<string>
                {
                    null,
                    "b"
                }
            };
            var cloned = (StringMembersSqlElement)original.Clone();

            AssertIntegrity(original, cloned);

            dynamic dyn = cloned;
            Assert.AreEqual("a", dyn.String1);
            Assert.IsNull(dyn.List1[0]);
            Assert.AreEqual("b", dyn.List1[1]);

            var original2 = new StringMembersSqlElement();
            var cloned2 = (StringMembersSqlElement)original2.Clone();

            Assert.IsNull(cloned2.String1);
            Assert.IsNull(cloned2.List1);
        }

        [Test]
        public void CloneCloneableMembersElement()
        {
            var original = new CloneableMembersContainer
            {
                Item1 = null,
                Item2 = new FooCloneable { Name = "a" },
                List1 = null,
                List2 = new List<FooCloneable>
                {
                    null,
                    new FooCloneable { Name = "b" },
                    new FooCloneable { Name = "c"}
                }
            };
            var cloned = (CloneableMembersContainer)original.Clone();

            AssertIntegrity(original, cloned);

            dynamic dyn = cloned;
            Assert.IsNull(dyn.Item1);
            Assert.AreEqual("a", dyn.Item2.Name);
            Assert.IsNull(dyn.List1);
            Assert.IsNull(dyn.List2[0]);
            Assert.AreEqual("b", dyn.List2[1].Name);
            Assert.AreEqual("c", dyn.List2[2].Name);
        }
    }
}
