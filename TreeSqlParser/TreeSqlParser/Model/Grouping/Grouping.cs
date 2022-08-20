using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Enums;

namespace TreeSqlParser.Model.Grouping
{
    public class GroupingSet : SqlElement
    {
        public GroupingSetType SetType { get; set; }
        public List<Column> Columns { get; set; }
    }
}
