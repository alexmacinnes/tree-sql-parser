using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TreeSqlParser.Model.Analytics;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Model.Columns
{
    public abstract class Column : SqlElement 
    {
        public abstract string DebuggerDisplay { get; }
    }

    public abstract class LiteralColumnBase : Column { }

    public abstract class LiteralColumn<T> : LiteralColumnBase
    {
        public T Value { get; set; }
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class IntegerColumn : LiteralColumn<long> 
    {
        public override string DebuggerDisplay => Value.ToString();
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class DecimalColumn : LiteralColumn<decimal>
    {
        public override string DebuggerDisplay => Value.ToString();
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class DateColumn : LiteralColumn<DateTime>
    {
        public override string DebuggerDisplay => Value.ToString("yyyy-MM-dd");
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class DateTimeColumn : LiteralColumn<DateTime>
    {
        public override string DebuggerDisplay => Value.ToString("yyyy-MM-dd HH:mm:ss.FFFFFF");
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class TimeColumn : LiteralColumn<TimeSpan>
    {
        public override string DebuggerDisplay => Value.ToString();
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class StringColumn : LiteralColumn<string>
    {
        public override string DebuggerDisplay => Value;
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class StarColumn : Column
    {
        public override string DebuggerDisplay => "*";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class AliasColumn : Column
    {
        public SqlIdentifier Alias { get; set; }
        public Column InnerColumn { get; set; }

        public override string DebuggerDisplay => $"{InnerColumn.DebuggerDisplay} AS {Alias.Name}";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class FunctionColumn : Column
    {
        public SqlIdentifier[] Name { get; set; }
        public List<Column> Parameters { get; set; }

        public override string DebuggerDisplay => 
            $"{string.Join(".", Name.Select(x=>x.Name))}({string.Join(", ", Parameters.Select(x=>x.DebuggerDisplay))})";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class AggregatedColumn : Column
    {
        public bool Distinct { get; set; }
        public Aggregation Aggregation { get; set; }
        public List<Column> InnerColumns { get; set; }

        public override string DebuggerDisplay =>
            $"{Aggregation}({string.Join(", ", InnerColumns.Select(x => x.DebuggerDisplay))})";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class PrimitiveColumn : Column
    {
        public SqlIdentifier TableAlias { get; set; }
        public SqlIdentifier Name { get; set; }

        public override string DebuggerDisplay =>
            TableAlias == null ? Name.Name : $"{TableAlias.Name}.{Name.Name}"; 
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class BracketedColumn : Column
    {
        public Column InnerColumn { get; set; }

        public override string DebuggerDisplay => $"({InnerColumn.DebuggerDisplay})";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class SubselectColumn : Column
    {
        public SelectStatement InnerSelect { get; set; }

        public override string DebuggerDisplay => $"(SELECT ...)";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class CaseBranch : SqlElement
    {
        public Condition Condition { get; set; }
        public Column Column { get; set; }

        public string DebuggerDisplay => $"WHEN {Condition.DebuggerDisplay} THEN {Column.DebuggerDisplay}";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class CaseColumn : Column
    {
        public List<CaseBranch> Branches { get; set; }
        public Column DefaultColumn { get; set; }

        public override string DebuggerDisplay => 
            $"CASE {string.Join(" ", Branches.Select(x => x.DebuggerDisplay))}" + 
            $"{(DefaultColumn == null ? "" : " ELSE " + DefaultColumn.DebuggerDisplay)} END";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class IifColumn : Column
    {
        public Condition Condition { get; set; }
        public Column TrueColumn { get; set; }
        public Column FalseColumn { get; set; }

        public override string DebuggerDisplay =>
            $"IIF({Condition.DebuggerDisplay}, {TrueColumn.DebuggerDisplay}, {FalseColumn.DebuggerDisplay})"; 
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ArithmeticChain : Column
    {
        public Column LeftColumn { get; set; }
        public List<ArithmeticOperation> Operations { get; set; }

        public override string DebuggerDisplay => 
            $"{LeftColumn.DebuggerDisplay} {string.Join(" ", Operations.Select(x => x.DebuggerDisplay))}";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ArithmeticOperation : SqlElement
    {
        public ArithmeticOperator Operator { get; set; }
        public Column RightColumn { get; set; }

        public string DebuggerDisplay => $"{Operator} {RightColumn.DebuggerDisplay}";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class NullColumn : Column
    {
        public override string DebuggerDisplay => "NULL";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class CastColumn : Column
    {
        public bool TryCast { get; set; }
        public Column Column { get; set; }
        public ColumnDataType DataType { get; set; }

        public override string DebuggerDisplay => 
            $"{(TryCast ? "TRY_" : "")}CAST({Column.DebuggerDisplay} AS {DataType.Value})";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ConvertColumn : Column
    {
        public bool TryConvert { get; set; }
        public ColumnDataType DataType { get; set; }
        public Column Column { get; set; }
        public Column Style { get; set; }

        public override string DebuggerDisplay =>
            $"{(TryConvert ? "TRY_" : "")}CONVERT({DataType.Value}, {Column.DebuggerDisplay}, {Style.DebuggerDisplay})";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ParseColumn : Column
    {
        public bool TryParse { get; set; }
        public Column Column { get; set; }
        public ColumnDataType DataType { get; set; }
        public string Culture { get; set; }

        public override string DebuggerDisplay =>
            $"{(TryParse ? "TRY_" : "")}PARSE({Column.DebuggerDisplay} AS {DataType.Value}" + 
            $"{(Culture == null ? "" : " USING " + Culture)})";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class VariableColumn : Column
    {
        public string VariableName { get; set; }

        public override string DebuggerDisplay => "%" + VariableName;
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class ColumnDataType : SqlElement
    {
        public string Value { get; set; }

        public string DebuggerDisplay => Value;
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class OrderByColumn : SqlElement
    {
        public Column Column { get; set; }
        public string Collate { get; set; }
        public ColumnSortOrder SortOrder { get; set; }

        public string DebuggerDisplay =>
            $"{Column.DebuggerDisplay} {(Collate == null ? "" : "COLLATE " + Collate)}{SortOrder}";
    }

    [DebuggerDisplay("{DebuggerDisplay}")]
    public class OverColumn : Column
    {
        public Column Column { get; set; }
        public Over Over { get; set; }

        public override string DebuggerDisplay => $"{Column.DebuggerDisplay} OVER ...";
    }
}
