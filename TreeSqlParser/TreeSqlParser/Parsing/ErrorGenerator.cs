using System.Collections.Generic;
using TSQL.Tokens;

namespace TreeSqlParser.Parsing
{
    public class ErrorGenerator
    {
        private IList<TSQLWhitespace> whitespaceTokens;

        public ErrorGenerator(IList<TSQLWhitespace> whitespaceTokens) 
        {
            this.whitespaceTokens = whitespaceTokens;
        }
    }
}