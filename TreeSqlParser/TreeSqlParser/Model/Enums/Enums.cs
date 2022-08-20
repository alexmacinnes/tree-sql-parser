using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Parsing.Enums;

namespace TreeSqlParser.Model.Enums
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum SetModifier
    {
        None,
        Union,
        UnionAll,
        Intersect,
        Except
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum Aggregation
    {
        Sum,
        Min,
        Max,
        Avg,
        Count,
        Approx_Count_Distinct,
        Checksum_Agg,
        Count_Big,
        Grouping,
        Grouping_Id,
        Stdev,
        StdevP,
        Var,
        VarP
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ColumnComparison
    {
        [EnumParseText("=")]
        Equals,
        [EnumParseText("<")]
        LessThan,
        [EnumParseText("<=")]
        LessThanOrEqual,
        [EnumParseText(">")]
        GreaterThan,
        [EnumParseText(">=")]
        GreaterThanOrEqual,
        [EnumParseText("<>")]
        NotEquals,
        [EnumParseText("LIKE")]
        Like
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LogicCondition
    {
        And,
        Or
    }

    public enum ArithmeticOperator
    {
        [EnumParseText("+")]
        Plus,
        [EnumParseText("-")]
        Minus,
        [EnumParseText("*")]
        Multiply,
        [EnumParseText("/")]
        Divide,
        [EnumParseText("%")]
        Modulo,
        [EnumParseText("||")]
        Concat
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum JoinType
    {
        InnerJoin,
        LeftJoin,
        RightJoin,
        FullJoin,
        CrossJoin,
        CrossApply,
        OuterApply
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ColumnSortOrder
    {
        Asc,
        Desc
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum ExtentType
    {
        [EnumParseText("ROWS")]
        Rows,

        [EnumParseText("RANGE")]
        Range
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GroupingSetType
    {
        Columns,

        [EnumParseText("ROLLUP")]
        Rollup,

        [EnumParseText("CUBE")]
        Cube
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum SetConditionType
    {
        Some,
        All
    }
}
