using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Parsing
{
    public class ParseContext
    {
        public TokenList TokenList { get; set; }

        public ErrorGenerator ErrorGenerator { get; set; }

        public ParseContext() { }

        public ParseContext Subcontext(TokenList innerTokenList)
            => new ParseContext { TokenList = innerTokenList, ErrorGenerator = ErrorGenerator };
    }
}
