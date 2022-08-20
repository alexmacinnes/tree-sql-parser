using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Grouping;
using TreeSqlParser.Model.Hints;
using TreeSqlParser.Model.Pivoting;
using TreeSqlParser.Model.Relations;

namespace TreeSqlParser.Model.Selects
{
    public class Select : SqlElement
    {
        public SetModifier SetModifier { get; set; }
        public bool Distinct { get; set; }
        public SelectTop Top { get; set; }
        public List<Column> Columns { get; set; }
        public List<Relation> From { get; set; }
        public Pivot Pivot { get; set; }
        public Condition WhereCondition { get; set; }
        public List<GroupingSet> GroupBy { get; set; }
        public Condition HavingCondition { get; set; }
    }

    public class AliasSelect : SqlElement
    {
        public SqlIdentifier Alias { get; set; }
        public List<Select> Selects { get; set; }
    }

    public class CteSelect : AliasSelect
    { } 

    public class SelectStatement : SqlElement
    {
        public List<CteSelect> WithSelects { get; set; }
        public List<Select> Selects { get; set; }
        public OrderBy OrderBy { get; set; }
        public SelectOptions Options { get; set; }
    }

    public class OrderBy : SqlElement
    {
        public List<OrderByColumn> Columns { get; set; }
        public Column Offset { get; set; }
        public Column Fetch { get; set; }
    }

    public class SelectTop : SqlElement
    {
        public Column Top { get; set; }
        public bool Percent { get; set; }
        public bool WithTies { get; set; }
    }
    
}
