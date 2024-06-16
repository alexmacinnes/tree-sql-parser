using NUnit.Framework;
using TreeSqlParser.Parsing;
using TreeSqlParser.Parsing.Errors;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    public class ErrorGeneratorTests
    {
        [TestCase("select ()", "Line:0, Idx:7, Tok:(, Msg:Empty brackets found")]
        [TestCase("  select ()", "Line:0, Idx:9, Tok:(, Msg:Empty brackets found")]
        [TestCase("\n  select ()", "Line:1, Idx:9, Tok:(, Msg:Empty brackets found")]
        [TestCase("select\n()", "Line:1, Idx:0, Tok:(, Msg:Empty brackets found")]
        [TestCase("select\n\n\n()", "Line:3, Idx:0, Tok:(, Msg:Empty brackets found")]
        [TestCase("select \n  ()", "Line:1, Idx:2, Tok:(, Msg:Empty brackets found")]
        [TestCase("select \n a , ()", "Line:1, Idx:5, Tok:(, Msg:Empty brackets found")]
        [TestCase("select \n a\n, ()", "Line:2, Idx:2, Tok:(, Msg:Empty brackets found")]
        public void ThrowsParseError(string sql, string expected)
        {
            string result = null;
            try
            {
                new SelectParser().ParseSelectStatement(sql);
            }
            catch (TreeSqlParseException ex) { 
                result = $"Line:{ex.LineNumber}, Idx:{ex.Index}, Tok:{ex.TokenText}, Msg:{ex.Message}";
            }

            Assert.AreEqual(expected, result);
        }
    }
}
