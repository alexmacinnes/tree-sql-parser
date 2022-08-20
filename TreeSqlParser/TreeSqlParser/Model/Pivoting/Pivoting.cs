using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Model.Pivoting
{
    public class Pivot : SqlElement
    {
        public Column AggregatedColumn { get; set; }
        public Column PivotColumn { get; set; }
        public List<Column> PivotValues { get; set; }
        public SqlIdentifier Alias { get; set; }
    }
}
