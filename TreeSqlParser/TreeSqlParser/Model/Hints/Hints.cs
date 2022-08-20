using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Model.Hints
{
    public class SelectOptions : SqlElement
    {
        public List<string> Options { get; set; }
    }
}
