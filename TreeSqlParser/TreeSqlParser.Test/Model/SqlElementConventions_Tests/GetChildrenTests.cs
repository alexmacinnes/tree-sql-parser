using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;

namespace TreeSqlParser.Test.Model.SqlElementConventions_Tests
{
    [TestFixture]
    public class GetChildrenTests
    {
        [Test]
        public void GetAllChildren()
        {
            var x = new FooComplexSqlElement
            {
                Element1 = new FooSimpleSqlElement("a"),
                Element2 = new FooSimpleSqlElement("b"),
                List1 = new List<SqlElement> { new FooSimpleSqlElement("c") },
                List2 = new List<SqlElement> { new FooSimpleSqlElement("d"), new FooSimpleSqlElement("e") },
            };

            var children = SqlElementConventions.GetChildren(x).ToList();

            Assert.AreEqual(5, children.Count);
            Assert.AreEqual("abcde", string.Join(string.Empty, children));
        }

        [Test]
        public void GetChildrenIgnoresNullListItems()
        {
            var x = new FooComplexSqlElement
            {
                List1 = new List<SqlElement> { new FooSimpleSqlElement("a"), null, new FooSimpleSqlElement("c") }
            };

            var children = SqlElementConventions.GetChildren(x).ToList();

            Assert.AreEqual(2, children.Count);
            Assert.AreEqual("ac", string.Join(string.Empty, children));
        }

        [Test]
        public void GetAlDerivedChildren()
        {
            var x = new FooComplexSqlElement
            {
                Element1 = new DerivedSimpleSqlElement("a"),
                Element2 = new DerivedSimpleSqlElement("b"),
                List1 = new List<SqlElement> { new DerivedSimpleSqlElement("c") },
                List2 = new List<SqlElement> { new DerivedSimpleSqlElement("d"), new DerivedSimpleSqlElement("e") },
            };

            var children = SqlElementConventions.GetChildren(x).ToList();

            Assert.AreEqual(5, children.Count);
            Assert.AreEqual("abcde", string.Join(string.Empty, children));
        }

        [Test]
        public void IgnoreNullChildren()
        {
            var x = new FooComplexSqlElement();

            var children = SqlElementConventions.GetChildren(x).ToList();

            Assert.IsFalse(children.Any());
        }

        [Test]
        public void GetAllChildrenFromDerivedType()
        {
            var x = new DerivedComplexSqlElement
            {
                Element1 = new FooSimpleSqlElement("a"),
                List1 = new List<SqlElement> { new FooSimpleSqlElement("c") }
            };

            var children = SqlElementConventions.GetChildren(x).ToList();

            Assert.AreEqual(2, children.Count);
            Assert.AreEqual("ac", string.Join(string.Empty, children));
        }

        [Test]
        public void GetAllChildrenFromDerivedPropertyType()
        {
            var x = new DerivedElementContainer
            {
                DerivedElement1 = new DerivedSimpleSqlElement("a"),
                DerivedList1 = new List<DerivedSimpleSqlElement> { new DerivedSimpleSqlElement("b"), new DerivedSimpleSqlElement("c") }
            };

            var children = SqlElementConventions.GetChildren(x).ToList();

            Assert.AreEqual(3, children.Count);
            Assert.AreEqual("abc", string.Join(string.Empty, children));
        }

        [Test]
        public void GetChildrenIgnoresNonPublicProperties()
        {
            var x = new PrivateMembersSqlElement();
            var children = SqlElementConventions.GetChildren(x);
            Assert.IsFalse(children.Any());
        }

        [Test]
        public void GetChildrenIgnoresNonSqlElementProperties()
        {
            var x = new StringMembersSqlElement();
            var children = SqlElementConventions.GetChildren(x);
            Assert.IsFalse(children.Any());
        }
    }
}
