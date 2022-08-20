using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Analytics;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Grouping;
using TreeSqlParser.Model.Hints;
using TreeSqlParser.Model.Pivoting;
using TreeSqlParser.Model.Relations;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    internal class SqlStringifier
    {
        public static string DebugString(SqlRootElement r) => DebugString((SelectStatement)r.Child);

        private static string DebugString(SelectStatement s)
        {
            var sb = new StringBuilder();

            if (s.WithSelects?.Any() == true)
            {
                sb.Append("WITH ");
                var withs = s.WithSelects.Select(WithDebugString);
                sb.Append(string.Join(", ", withs));
                sb.Append(" ");
            }

            sb.Append(SelectsString(s.Selects));

            sb.Append(OrderByString(s.OrderBy));

            sb.Append(OptionsString(s.Options));

            return sb.ToString();
        }

        private static string OptionsString(SelectOptions o)
        {
            if (o == null) return string.Empty;

            return $" OPTION ({string.Join(",", o.Options)})";
        }

        private static string OrderByString(OrderBy o)
        {
            if (o == null) return string.Empty;

            var sb = new StringBuilder();
            sb.Append(" ORDER BY ");
            if (o.Columns.Any() == true)
            {
                var orders = o.Columns.Select(OrderByString);
                sb.Append(string.Join(", ", orders));
            }

            if (o.Offset != null)
                sb.Append($" OFFSET {ColumnString(o.Offset)} ROWS");

            if (o.Fetch != null)
                sb.Append($" FETCH NEXT {ColumnString(o.Fetch)} ROWS ONLY");

            return sb.ToString();
        }

        private static string OrderByString(OrderByColumn o) => 
            $"{ColumnString(o.Column)}{(o.Collate == null ? string.Empty : " COLLATE " + o.Collate)} {o.SortOrder.ToString().ToUpper()}";

        private static string WithDebugString(AliasSelect s)
        {
            var sb = new StringBuilder();
            sb.Append(s.Alias.Name);
            sb.Append(" AS (");
            sb.Append(SelectsString(s.Selects));
            sb.Append(")");

            return sb.ToString();
        }

        private static string SelectsString(List<Select> selects)
        {
            var strings = selects.Select(SelectString);
            return string.Join(" ", strings);
        }

        private static string SelectString(Select s)
        {
            var sb = new StringBuilder();
            sb.Append(SetModifierString(s.SetModifier));
            sb.Append("SELECT ");
            sb.Append(TopString(s.Top));
            if (s.Distinct)
                sb.Append("DISTINCT ");

            var cols = s.Columns.Select(ColumnString);
            sb.Append(string.Join(", ", cols));

            if (s.From?.Any() == true)
            {
                sb.Append(" FROM ");
                var relations = s.From.Select(RelationString);
                sb.Append(string.Join(", ", relations));
            }

            sb.Append(PivotString(s.Pivot));

            if (s.WhereCondition != null)
                sb.Append(" WHERE " + ConditionString(s.WhereCondition));

            sb.Append(GroupByString(s.GroupBy));

            if (s.HavingCondition != null)
                sb.Append(" HAVING " + ConditionString(s.HavingCondition));

            return sb.ToString();
        }

        private static string PivotString(Pivot p)
        {
            if (p == null) return string.Empty;
            return $" PIVOT ({ColumnString(p.AggregatedColumn)} FOR {ColumnString(p.PivotColumn)} IN ({ColumnsString(p.PivotValues)})) AS {SqlIdentifierString(p.Alias)}";
        }

        private static string GroupByString(List<GroupingSet> g)
        {
            if (g?.Any() != true)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append(" GROUP BY ");
            if (g.Count == 1)
                sb.Append(GroupingSetString(g[0], false));
            else
            {
                var setStrings = g.Select(x => GroupingSetString(x, true));
                string fullText = $"GROUPING SETS ({string.Join(", ", setStrings)})";
                sb.Append(fullText);
            }

            return sb.ToString();
        }

        private static string GroupingSetString(GroupingSet g, bool forceBrackets)
        {
            string columns = ColumnsString(g.Columns);
            string typeText = g.SetType == GroupingSetType.Columns ? null : g.SetType.ToString().ToUpper() + " ";
            bool brackets = forceBrackets || typeText != null;

            if (brackets)
                return $"{typeText}({columns})";
            else
                return columns;
        }

        private static string TopString(SelectTop t)
        {
            if (t == null)
                return string.Empty;

            string col = ColumnString(t.Top);
            string percent = t.Percent ? " PERCENT" : string.Empty;
            string withTies = t.WithTies ? " WITH TIES" : string.Empty;

            return $"TOP {col}{percent}{withTies} ";
        }

        private static string RelationString(Relation r) => r switch
        {
            BracketedRelation x => $"({RelationString(x.InnerRelation)})",
            SubselectRelation x => $"({DebugString(x.InnerSelect)}) AS {SqlIdentifierString(x.Alias)}",
            Table x => $"{SqlIdentifiersString(x.Name)}"
                    + (x.Alias == null ? string.Empty : $" AS {SqlIdentifierString(x.Alias)}"),
            JoinChain x => JoinChainString(x),
            _ => "UNKOWN RELATION TYPE",
        };

        private static string JoinChainString(JoinChain x)
        {
            var sb = new StringBuilder();
            sb.Append(RelationString(x.LeftRelation));

            if (x.Joins != null)
            {
                foreach (var j in x.Joins)
                {
                    sb.Append($" {JoinString(j.JoinType)} {RelationString(j.RightRelation)}");
                    if (j.Condition != null)
                        sb.Append($" ON {ConditionString(j.Condition)}");
                }
            }

            return sb.ToString();
        }

        private static string JoinString(JoinType j) => j switch
        {
            JoinType.InnerJoin => "INNER JOIN",
            JoinType.LeftJoin => "LEFT JOIN",
            JoinType.RightJoin => "RIGHT JOIN",
            JoinType.FullJoin => "FULL JOIN",
            JoinType.CrossJoin => "CROSS JOIN",
            JoinType.CrossApply => "CROSS APPLY",
            JoinType.OuterApply => "OUTER APPLY",
            _ => "UNKOWN JOIN TYPE",
        };

        private static string ColumnString(Column c) => c switch
        {
            AliasColumn x => $"{ColumnString(x.InnerColumn)} AS {SqlIdentifierString(x.Alias)}",
            IntegerColumn x => x.Value.ToString(),
            DecimalColumn x => x.Value.ToString(),
            StringColumn x => $"'{x.Value}'",
            DateColumn x => $"{{d '{x.Value:yyyy-MM-dd}'}}",
            DateTimeColumn x => $"{{ts '{x.Value:yyyy-MM-dd HH:mm:ss.FFFFFFF}'}}",
            TimeColumn x => $"{{t '{x.Value}'}}",
            StarColumn _ => "*",
            PrimitiveColumn x => x.TableAlias == null ? SqlIdentifierString(x.Name) : $"{SqlIdentifierString(x.TableAlias)}.{SqlIdentifierString(x.Name)}",
            FunctionColumn x => $"{SqlIdentifiersString(x.Name)}({ColumnsString(x.Parameters)})",
            AggregatedColumn x => $"{x.Aggregation.ToString().ToUpper()}({(x.Distinct ? "DISTINCT " : "")}{ColumnsString(x.InnerColumns)})",
            BracketedColumn x => $"({ColumnString(x.InnerColumn)})",
            SubselectColumn x => $"({DebugString(x.InnerSelect)})",
            CaseColumn x => CaseColumnString(x),
            ArithmeticChain x => ArithmeticChainString(x),
            NullColumn _ => "NULL",
            CastColumn x => $"{(x.TryCast ? "TRY_" : string.Empty)}CAST({ColumnString(x.Column)} AS {x.DataType.Value})",
            ConvertColumn x => $"{(x.TryConvert ? "TRY_" : string.Empty)}CONVERT({x.DataType.Value}, {ColumnString(x.Column)}{(x.Style == null ? string.Empty : ", " + ColumnString(x.Style))})",
            ParseColumn x => $"{(x.TryParse ? "TRY_" : string.Empty)}PARSE({ColumnString(x.Column)} AS {x.DataType.Value}{(x.Culture == null ? string.Empty : " USING '" + x.Culture + "'")})",
            VariableColumn x => $"@{x.VariableName}",
            OverColumn x => $"{ColumnString(x.Column)}{OverString(x.Over)}",
            IifColumn x => $"IIF({ConditionString(x.Condition)}, {ColumnString(x.TrueColumn)}, {ColumnString(x.FalseColumn)})",
            _ => "UNKOWN COLUMN",
        };

        private static string OverString(Over over)
        {
            if (over == null)
                return string.Empty;
            
            var elements = new List<string>();
            if (over.PartitionBy?.Any() == true)
                elements.Add("PARTITION BY " + ColumnsString(over.PartitionBy));

            if (over.OrderBy?.Any() == true)
                elements.Add("ORDER BY " + string.Join(", ", over.OrderBy.Select(OrderByString)));

            // [ROWS|RANGE] [null|uint] TO [null|uint]
            if (over.Extent != null)
                elements.Add($"{over.Extent.ExtentType.ToString().ToUpperInvariant()} {(over.Extent.LowerBound?.ToString() ?? "null")} TO {(over.Extent.UpperBound?.ToString() ?? "null")}");

            return $" OVER({string.Join(" ", elements)})";
        }

        private static string ArithmeticChainString(ArithmeticChain x)
        {
            var sb = new StringBuilder();
            sb.Append(ColumnString(x.LeftColumn));
            if (x.Operations != null)
            {
                foreach (var o in x.Operations)
                    sb.Append($"{ArithmeticOperatorString(o.Operator)}{ColumnString(o.RightColumn)}");
            }

            return sb.ToString();
        }

        private static string ColumnsString(IEnumerable<Column> c) => string.Join(", ", c.Select(ColumnString));
        
        private static string CaseColumnString(CaseColumn c)
        {
            var sb = new StringBuilder();
            sb.Append("CASE");

            foreach (var b in c.Branches)
            {
                sb.Append(" WHEN ");
                sb.Append(ConditionString(b.Condition));
                sb.Append(" THEN ");
                sb.Append(ColumnString(b.Column));
            }

            if (c.DefaultColumn != null)
            {
                sb.Append(" ELSE ");
                sb.Append(ColumnString(c.DefaultColumn));
            }

            sb.Append(" END");

            return sb.ToString();
        }

        private static string ConditionString(Condition c)
        {
            return c switch
            {
                ComparisonCondition x => $"{ColumnString(x.LeftColumn)}{ComparisonString(x.Comparison)}{ColumnString(x.RightColumn)}",
                NotCondition x => $"NOT {ConditionString(x.InnerCondition)}",
                BracketCondition x => $"({ConditionString(x.InnerCondition)})",
                InListCondition x => $"{ColumnString(x.LeftColumn)} IN ({string.Join(",", x.RightColumns.Select(ColumnString))})",
                InSubselectCondition x => $"{ColumnString(x.LeftColumn)} IN ({DebugString(x.Subselect)})",
                ExistsCondition x => $"EXISTS ({DebugString(x.Subselect)})",
                BetweenCondition x => $"{ColumnString(x.LeftColumn)} BETWEEN {ColumnString(x.LowerBound)} AND {ColumnString(x.UpperBound)}",
                IsNullCondition x => $"{ColumnString(x.LeftColumn)} IS NULL",
                NotNullCondition x => $"{ColumnString(x.LeftColumn)} IS NOT NULL",
                ConditionChain x => ConditionChainString(x),
                SetCondition x => $"{ColumnString(x.LeftColumn)} {ComparisonString(x.Comparison)} {x.SetConditionType.ToString().ToUpperInvariant()} {ColumnString(x.SubselectColumn)}",
                _ => "UNKOWN CONDITION",
            };
        }

        private static string ConditionChainString(ConditionChain x)
        {
            var sb = new StringBuilder();
            sb.Append(ConditionString(x.LeftCondition));

            if (x.OtherConditions != null)
                foreach (var c in x.OtherConditions)
                    sb.Append($" {c.LogicCondition.ToString().ToUpperInvariant()} {ConditionString(c.RightCondition)}");

            return sb.ToString();
        }

        private static string ComparisonString(ColumnComparison comparison) => comparison switch
        {
            ColumnComparison.Equals => "=",
            ColumnComparison.LessThan => "<",
            ColumnComparison.LessThanOrEqual => "<=",
            ColumnComparison.GreaterThan => ">",
            ColumnComparison.GreaterThanOrEqual => ">=",
            ColumnComparison.NotEquals => "<>",
            ColumnComparison.Like => " LIKE ",
            _ => "UNKOWN COMPARISON",
        };

        private static string ArithmeticOperatorString(ArithmeticOperator o) => o switch
        {
            ArithmeticOperator.Plus => "+",
            ArithmeticOperator.Minus => "-",
            ArithmeticOperator.Multiply => "*",
            ArithmeticOperator.Divide => "/",
            ArithmeticOperator.Modulo => "%",
            ArithmeticOperator.Concat => "||",
            _ => "UNKOWN ARITHMETIC OPERATOR",
        };

        private static string SqlIdentifiersString(IEnumerable<SqlIdentifier> i) =>
            string.Join(".", i.Select(SqlIdentifierString));

        private static string SqlIdentifierString(SqlIdentifier i) =>
            i.IsBracketed ? $"[{i.Name}]" : i.Name;

        private static string SetModifierString(SetModifier s) => s switch
        {
            SetModifier.None => string.Empty,
            SetModifier.UnionAll => "UNION ALL ",
            _ => s.ToString().ToUpperInvariant() + " ",
        };
    }
}
