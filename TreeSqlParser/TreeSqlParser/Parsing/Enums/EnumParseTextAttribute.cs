using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Parsing.Enums
{
    public class EnumParseTextAttribute : Attribute
    {
        public string ParseText { get; }

        public EnumParseTextAttribute(string parseText)
        {
            ParseText = parseText;
        }

        public override string ToString() => ParseText;
    }
}
