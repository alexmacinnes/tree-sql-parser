using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Model
{
    public class SqlIdentifier : ICloneable
    {
        public string Name { get; set; }

        public bool IsBracketed { get; set; }

        public SqlIdentifier() { }

        public SqlIdentifier(string name)
        {
            if (name.StartsWith("[") && name.EndsWith("]"))
            {
                Name = name[1..^1];
                IsBracketed = true;
            }
            else
                Name = name;
        }

        public object Clone() => MemberwiseClone();
    }
}
