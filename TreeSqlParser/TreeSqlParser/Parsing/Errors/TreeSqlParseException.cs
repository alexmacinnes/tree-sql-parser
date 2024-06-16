using System;
using System.Collections.Generic;
using System.Text;
using TSQL.Tokens;

namespace TreeSqlParser.Parsing.Errors
{
    public class TreeSqlParseException : Exception
    {
        public TreeSqlParseException() { }

        public TreeSqlParseException(string message, int? lineNumber, int? index, string tokenText) 
            : base(message)
        {
            LineNumber = lineNumber;
            Index = index;
            TokenText = tokenText;
        }

        public int? LineNumber { get; }
        public int? Index { get; }
        public string TokenText { get; }
    }
}
