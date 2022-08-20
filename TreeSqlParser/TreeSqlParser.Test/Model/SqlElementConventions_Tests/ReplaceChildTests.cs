using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;

namespace TreeSqlParser.Test.Model.SqlElementConventions_Tests
{
    [TestFixture]
    public class ReplaceChildTests
    {
        [Test]
        public void ReplaceSingleElement()
        {
            var x = new FooComplexSqlElement
            {
                Element1 = new FooSimpleSqlElement("a"),
                Element2 = new FooSimpleSqlElement("b"),
                List1 = new List<SqlElement> { new FooSimpleSqlElement("c") },
                List2 = new List<SqlElement> { new FooSimpleSqlElement("d"), new FooSimpleSqlElement("e"), new FooSimpleSqlElement("f") }
            };

            var childToReplace = x.Element2;
            var replacement = new FooSimpleSqlElement("Z");

            bool result = SqlElementConventions.TryReplaceChild(x, childToReplace, replacement);

            Assert.IsTrue(result);
            Assert.AreSame(x.Element2, replacement);
            Assert.AreEqual("aZcdef", string.Join(string.Empty, SqlElementConventions.GetChildren(x)));
        }

        [Test]
        public void ReplaceListHead()
        {
            var x = new FooComplexSqlElement
            {
                Element1 = new FooSimpleSqlElement("a"),
                Element2 = new FooSimpleSqlElement("b"),
                List1 = new List<SqlElement> { new FooSimpleSqlElement("c") },
                List2 = new List<SqlElement> { new FooSimpleSqlElement("d"), new FooSimpleSqlElement("e"), new FooSimpleSqlElement("f") }
            };

            var childToReplace = x.List2[0];
            var replacement = new FooSimpleSqlElement("Z");

            bool result = SqlElementConventions.TryReplaceChild(x, childToReplace, replacement);

            Assert.IsTrue(result);
            Assert.AreSame(x.List2[0], replacement);
            Assert.AreEqual("abcZef", string.Join(string.Empty, SqlElementConventions.GetChildren(x)));
        }

        [Test]
        public void ReplaceListTail()
        {
            var x = new FooComplexSqlElement
            {
                Element1 = new FooSimpleSqlElement("a"),
                Element2 = new FooSimpleSqlElement("b"),
                List1 = new List<SqlElement> { new FooSimpleSqlElement("c") },
                List2 = new List<SqlElement> { new FooSimpleSqlElement("d"), new FooSimpleSqlElement("e"), new FooSimpleSqlElement("f") }
            };

            var childToReplace = x.List2[2];
            var replacement = new FooSimpleSqlElement("Z");

            bool result = SqlElementConventions.TryReplaceChild(x, childToReplace, replacement);

            Assert.IsTrue(result);
            Assert.AreSame(x.List2[2], replacement);
            Assert.AreEqual("abcdeZ", string.Join(string.Empty, SqlElementConventions.GetChildren(x)));
        }

        [Test]
        public void ReplaceChildSkipsOverNullElements()
        {
            var x = new FooComplexSqlElement
            {
                Element1 = null,
                Element2 = null,
                List1 = null,
                List2 = new List<SqlElement> {null, new FooSimpleSqlElement("a")}
            };

            var childToReplace = x.List2[1];
            var replacement = new FooSimpleSqlElement("Z");

            bool result = SqlElementConventions.TryReplaceChild(x, childToReplace, replacement);

            Assert.IsTrue(result);
            Assert.AreSame(x.List2[1], replacement);
            Assert.AreEqual("Z", string.Join(string.Empty, SqlElementConventions.GetChildren(x)));
        }

        [Test]
        public void ReplaceSingleElementWithBadBaseTypeThrows()
        {
            var x = new DerivedElementContainer
            {
                DerivedElement1 = new DerivedSimpleSqlElement("a")
            };

            DerivedSimpleSqlElement childToReplace = x.DerivedElement1;
            SqlElement replacement = new FooSimpleSqlElement("Z");

            Assert.Throws<ArgumentException>(() => SqlElementConventions.TryReplaceChild(x, childToReplace, replacement));
        }

        [Test]
        public void ReplaceListElementWithBadBaseTypeThrows()
        {
            var x = new DerivedElementContainer
            {
                DerivedList1 = new List<DerivedSimpleSqlElement> { new DerivedSimpleSqlElement("a") }
            };

            DerivedSimpleSqlElement childToReplace = x.DerivedList1[0];
            SqlElement replacement = new FooSimpleSqlElement("Z");

            Assert.Throws<ArgumentException>(() => SqlElementConventions.TryReplaceChild(x, childToReplace, replacement));
        }

        [Test]
        public void ReplaceChildReturnsFalseIfChildNotFound()
        {
            var x = new FooComplexSqlElement
            {
                Element1 = new FooSimpleSqlElement("a"),
                Element2 = new FooSimpleSqlElement("b"),
                List1 = new List<SqlElement> { new FooSimpleSqlElement("c") },
                List2 = new List<SqlElement> { new FooSimpleSqlElement("d"), new FooSimpleSqlElement("e"), new FooSimpleSqlElement("f") }
            };

            var childToReplace = new FooSimpleSqlElement("Y");              // not contained in x above
            var replacement = new FooSimpleSqlElement("Z");

            bool result = SqlElementConventions.TryReplaceChild(x, childToReplace, replacement);

            Assert.IsFalse(result);
            Assert.AreEqual("abcdef", string.Join(string.Empty, SqlElementConventions.GetChildren(x)));
        }
    }
}
