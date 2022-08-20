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
                Name = name.Substring(1, name.Length-2);
                IsBracketed = true;
            }
            else
                Name = name;
        }

        public object Clone() => MemberwiseClone();
    }
}
