using System;
using System.Collections.Generic;
using System.Text;

namespace TreeSqlParser.Model
{
    public class SqlRootElement : SqlElement
    {
        public override SqlElement Parent 
        { 
            get => null; 
            set => throw new InvalidOperationException("Root element may not have a parent"); 
        }

        public override void ReplaceSelf(SqlElement replacement) =>
            throw new InvalidOperationException("Root element may not be replaced");

        public SqlElement Child { get; set; }
    }
}
