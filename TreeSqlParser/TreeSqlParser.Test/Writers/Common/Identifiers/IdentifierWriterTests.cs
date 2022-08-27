using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Writers.Safe.Identifiers;

namespace TreeSqlParser.Writers.Test.Safe.Identifiers
{
    public class IdentifierWriterTests
    {
        IIdentifierWriter identifierWriter = new IdentifierWriter('[', ']');

        private string Delimit(string name) =>
            identifierWriter.Delimit(new SqlIdentifier { Name = name });
        
        [Test]
        public void DelimitSimpleName()
        {
            string result = Delimit("a bc");
            Assert.AreEqual("[a bc]", result);
        }

        [TestCase('[')]
        [TestCase(']')]
        [TestCase(';')]
        public void DelimitThrowsOnIllegalCharacter(char c)
        {
            Assert.Throws<InvalidOperationException>(() => Delimit("x" + c + "x"));
        }
    }
}
