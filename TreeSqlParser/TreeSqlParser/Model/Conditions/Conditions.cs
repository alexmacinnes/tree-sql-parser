using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Model.Conditions
{
    public abstract class Condition : SqlElement 
    { 
        public abstract string DebuggerDisplay { get; }
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ComparisonCondition : Condition
    {
        public Column LeftColumn { get; set; }
        public Column RightColumn { get; set; }
        public ColumnComparison Comparison { get; set; }

        public override string DebuggerDisplay =>
            $"{LeftColumn.DebuggerDisplay} {Comparison} {RightColumn.DebuggerDisplay}";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ConditionChain : Condition
    {
        public Condition LeftCondition { get; set; }
        public List<LogicOperation> OtherConditions { get; set; }

        public override string DebuggerDisplay =>
            $"{LeftCondition.DebuggerDisplay} {string.Join(" ", OtherConditions.Select(x => x.DebuggerDisplay))}";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class LogicOperation : SqlElement
    {
        public LogicCondition LogicCondition { get; set; }
        public Condition RightCondition { get; set; }

        public string DebuggerDisplay =>
            $"{LogicCondition} {RightCondition.DebuggerDisplay}";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class NotCondition : Condition
    {
        public Condition InnerCondition { get; set; }

        public override string DebuggerDisplay => $"NOT {InnerCondition.DebuggerDisplay}";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class BracketCondition : Condition
    {
        public Condition InnerCondition { get; set; }

        public override string DebuggerDisplay => $"({InnerCondition.DebuggerDisplay})";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class InListCondition : Condition
    {
        public Column LeftColumn { get; set; }
        public List<Column> RightColumns { get; set; }

        public override string DebuggerDisplay => 
            $"{LeftColumn.DebuggerDisplay} IN ({string.Join(", ", RightColumns.Select(x => x.DebuggerDisplay))})";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class InSubselectCondition : Condition
    {
        public Column LeftColumn { get; set; }
        public SelectStatement Subselect { get; set; }

        public override string DebuggerDisplay =>
            $"{LeftColumn.DebuggerDisplay} IN (SELECT ...)";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ExistsCondition : Condition
    {
        public SelectStatement Subselect { get; set; }

        public override string DebuggerDisplay => $"EXISTS (SELECT ...)";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class BetweenCondition : Condition
    {
        public Column LeftColumn { get; set; }
        public Column LowerBound { get; set; }
        public Column UpperBound { get; set; }

        public override string DebuggerDisplay => 
            $"{LeftColumn.DebuggerDisplay} BETWEEN {LowerBound.DebuggerDisplay} AND {UpperBound.DebuggerDisplay}";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class IsNullCondition : Condition
    {
        public Column LeftColumn { get; set; }

        public override string DebuggerDisplay => $"{LeftColumn.DebuggerDisplay} IS NULL";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class NotNullCondition : Condition
    {
        public Column LeftColumn { get; set; }

        public override string DebuggerDisplay => $"{LeftColumn.DebuggerDisplay} IS NOT NULL";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class SetCondition : Condition
    {
        public Column LeftColumn { get; set; }
        public ColumnComparison Comparison { get; set; }          
        public SetConditionType SetConditionType { get; set; }
        public Column SubselectColumn { get; set; }

        public override string DebuggerDisplay => 
            $"{LeftColumn.DebuggerDisplay} {Comparison} {SetConditionType} {SubselectColumn.DebuggerDisplay}";
    }
}
