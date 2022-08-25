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

namespace TreeSqlParser.Writers.Full
{
    public class FullSqlServerWriter : ISqlWriter
    {
        public string GenerateSql(SqlElement e)
        {
            if (e is SqlRootElement root) return GenerateSql(root.Child);

            //Analytics.cs
            if (e is Over over) return OverSql(over);
            if (e is OverExtent overExtent) return OverExtentSql(overExtent);

            //Columns.cs
            if (e is Column column) return ColumnSql(column);
            if (e is ArithmeticOperation arithmeticOperation) return ArithmeticOperationSql(arithmeticOperation);
            if (e is CaseBranch caseBranch) return CaseBranchSql(caseBranch);

            //Conditions.cs
            if (e is Condition condition) return ConditionSql(condition);
            if (e is LogicOperation logicOperation) return LogicOperationSql(logicOperation);

            //Grouping.cs
            if (e is GroupingSet groupingSet) return GroupingSetSql(groupingSet);

            //Hints.cs
            if (e is SelectOptions selectOptions) return SelectOptionsSql(selectOptions);

            //Pivoting.cs
            if (e is Pivot pivot) return PivotSql(pivot);

            //Relations.cs
            if (e is Relation relation) return RelationSql(relation);
            if (e is Join join) return JoinSql(join);

            //Selects.cs
            if (e is SelectStatement selectStatement) return SelectStatementSql(selectStatement);
            if (e is Select select) return SelectSql(select);
            if (e is CteSelect cteSelect) return CteSql(cteSelect);
            if (e is OrderBy orderBy) return OrderBySql(orderBy);
            if (e is SelectTop selectTop) return SelectTopSql(selectTop);

            throw new NotSupportedException("Unknown SqlElement type: " + e.GetType().Name);
        }

        private string RelationSql(Relation x)
        {
            if (x is BracketedRelation bracketedRelation) return $"({RelationSql(bracketedRelation.InnerRelation)})";
            if (x is SubselectRelation subselectRelation) return $"({SelectStatementSql(subselectRelation.InnerSelect)}) AS {SqlIdentifierSql(subselectRelation.Alias)}";
            if (x is Table table) return $"{SqlIdentifierSql(table.Name)}" + (table.Alias == null ? string.Empty : $" AS {SqlIdentifierSql(table.Alias)}");
            if (x is JoinChain joinChain) return JoinChainSql(joinChain);

            throw new NotSupportedException("Unknown Relation typoe: " + x.GetType().Name);
        }

        private string JoinChainSql(JoinChain x)
        {
            var sb = new StringBuilder();
            sb.Append(RelationSql(x.LeftRelation));

            if (x.Joins != null)
            {
                foreach (var j in x.Joins)
                {
                    sb.Append($" {JoinSql(j)}");
                }
            }

            return sb.ToString();
        }

        private string JoinSql(Join j) =>
            $"{JoinTypeSql(j.JoinType)} {RelationSql(j.RightRelation)}" +
            (j.Condition == null ? String.Empty : $" ON {ConditionSql(j.Condition)}");

        private string JoinTypeSql(JoinType j)
        {
            switch (j)
            {
                case JoinType.InnerJoin: return "INNER JOIN";
                case JoinType.LeftJoin: return "LEFT JOIN";
                case JoinType.RightJoin: return "RIGHT JOIN";
                case JoinType.FullJoin: return "FULL JOIN";
                case JoinType.CrossJoin: return "CROSS JOIN";
                case JoinType.CrossApply: return "CROSS APPLY";
                case JoinType.OuterApply: return "OUTER APPLY";
                default: throw new NotSupportedException("Unknown JoinType: " + j.ToString());
            }
        }

        private string PivotSql(Pivot p)
        {
            if (p == null) return string.Empty;
            return $" PIVOT ({ColumnSql(p.AggregatedColumn)} FOR {ColumnSql(p.PivotColumn)} IN ({ColumnsSql(p.PivotValues)})) AS {SqlIdentifierSql(p.Alias)}";
        }

        private string GroupingSetSql(GroupingSet g, bool forceBrackets = false)
        {
            string columns = ColumnsSql(g.Columns);
            string typeText = g.SetType == GroupingSetType.Columns ? null : g.SetType.ToString().ToUpper() + " ";
            bool brackets = forceBrackets || typeText != null;

            if (brackets)
                return $"{typeText}({columns})";
            else
                return columns;
        }

        private string ConditionSql(Condition c)
        {
            if (c is ComparisonCondition comparisonCondition)
                return $"{ColumnSql(comparisonCondition.LeftColumn)}{ComparisonSql(comparisonCondition.Comparison)}{ColumnSql(comparisonCondition.RightColumn)}";
            if (c is NotCondition notCondition)
                return $"NOT {ConditionSql(notCondition.InnerCondition)}";
            if (c is BracketCondition bracketCondition)
                return $"({ConditionSql(bracketCondition.InnerCondition)})";
            if (c is InListCondition inListCondition)
                return $"{ColumnSql(inListCondition.LeftColumn)} IN ({string.Join(",", inListCondition.RightColumns.Select(ColumnSql))})";
            if (c is InSubselectCondition inSubselectCondition)
                return $"{ColumnSql(inSubselectCondition.LeftColumn)} IN ({SelectStatementSql(inSubselectCondition.Subselect)})";
            if (c is ExistsCondition existsCondition)
                return $"EXISTS ({SelectStatementSql(existsCondition.Subselect)})";
            if (c is BetweenCondition betweenCondition)
                return $"{ColumnSql(betweenCondition.LeftColumn)} BETWEEN {ColumnSql(betweenCondition.LowerBound)} AND {ColumnSql(betweenCondition.UpperBound)}";
            if (c is IsNullCondition isNullCondition)
                return $"{ColumnSql(isNullCondition.LeftColumn)} IS NULL";
            if (c is NotNullCondition notNullCondition)
                return $"{ColumnSql(notNullCondition.LeftColumn)} IS NOT NULL";
            if (c is ConditionChain conditionChain)
                return ConditionChainSql(conditionChain);
            if (c is SetCondition setCondition)
                return $"{ColumnSql(setCondition.LeftColumn)} {ComparisonSql(setCondition.Comparison)} {setCondition.SetConditionType.ToString().ToUpperInvariant()} {ColumnSql(setCondition.SubselectColumn)}";

            throw new NotSupportedException("Unknown condition type: " + c.GetType().Name);
        }

        private string ConditionChainSql(ConditionChain x)
        {
            var sb = new StringBuilder();
            sb.Append(ConditionSql(x.LeftCondition));

            if (x.OtherConditions != null)
                foreach (var c in x.OtherConditions)
                    sb.Append($" {LogicOperationSql(c)}");

            return sb.ToString();
        }

        private string LogicOperationSql(LogicOperation o) =>
            $"{o.LogicCondition.ToString().ToUpperInvariant()} {ConditionSql(o.RightCondition)}";

        private string ComparisonSql(ColumnComparison c)
        {
            switch (c)
            {
                case ColumnComparison.Equals: return "=";
                case ColumnComparison.LessThan: return "<";
                case ColumnComparison.LessThanOrEqual: return "<=";
                case ColumnComparison.GreaterThan: return ">";
                case ColumnComparison.GreaterThanOrEqual: return ">=";
                case ColumnComparison.NotEquals: return "<>";
                case ColumnComparison.Like: return " LIKE ";
                default: throw new NotSupportedException("Unknown column comparison: " + c.ToString());
            }
        }

        private string ColumnsSql(IEnumerable<Column> c) => string.Join(", ", c.Select(ColumnSql));

        private string ColumnSql(Column x)
        {
            if (x is AliasColumn aliasColumn) 
                return $"{ColumnSql(aliasColumn.InnerColumn)} AS {SqlIdentifierSql(aliasColumn.Alias)}";
            if (x is LiteralColumnBase literalColumn)
                return LiteralColumnSql(literalColumn);
            if (x is StarColumn)
                return "*";
            if (x is PrimitiveColumn primitiveColumn)
                return primitiveColumn.TableAlias == null 
                    ? SqlIdentifierSql(primitiveColumn.Name) 
                    : $"{SqlIdentifierSql(primitiveColumn.TableAlias)}.{SqlIdentifierSql(primitiveColumn.Name)}";
            if (x is FunctionColumn functionColumn)
                return $"{SqlIdentifierSql(functionColumn.Name)}({ColumnsSql(functionColumn.Parameters)})";
            if (x is AggregatedColumn aggregatedColumn)
                return $"{aggregatedColumn.Aggregation.ToString().ToUpper()}({(aggregatedColumn.Distinct ? "DISTINCT " : "")}{ColumnsSql(aggregatedColumn.InnerColumns)})";
            if (x is BracketedColumn bracketedColumn)
                return $"({ColumnSql(bracketedColumn.InnerColumn)})";
            if (x is SubselectColumn subselectColumn)
                return $"({SelectStatementSql(subselectColumn.InnerSelect)})";
            if (x is CaseColumn caseColumn)
                return CaseColumnSql(caseColumn);
            if (x is ArithmeticChain arithmeticChain)
                return ArithmeticChainSql(arithmeticChain);
            if (x is NullColumn)
                return "NULL";
            if (x is CastColumn castColumn)
                return $"{(castColumn.TryCast ? "TRY_" : string.Empty)}CAST({ColumnSql(castColumn.Column)} AS {castColumn.DataType.Value})";
            if (x is ConvertColumn convertColumn)
                return $"{(convertColumn.TryConvert ? "TRY_" : string.Empty)}CONVERT({convertColumn.DataType.Value}, {ColumnSql(convertColumn.Column)}{(convertColumn.Style == null ? string.Empty : ", " + ColumnSql(convertColumn.Style))})";
            if (x is ParseColumn parseColumn)
                return $"{(parseColumn.TryParse ? "TRY_" : string.Empty)}PARSE({ColumnSql(parseColumn.Column)} AS {parseColumn.DataType.Value}{(parseColumn.Culture == null ? string.Empty : " USING '" + parseColumn.Culture + "'")})";
            if (x is VariableColumn variableColumn)
                return $"@{variableColumn.VariableName}";
            if (x is OverColumn overColumn)
                return $"{ColumnSql(overColumn.Column)}{OverSql(overColumn.Over)}";
            if (x is IifColumn iifColumn)
                return $"IIF({ConditionSql(iifColumn.Condition)}, {ColumnSql(iifColumn.TrueColumn)}, {ColumnSql(iifColumn.FalseColumn)})";

            throw new NotSupportedException("Unknown column type: " + x.GetType().Name);
        }

        private string OverSql(Over over)
        {
            if (over == null)
                return string.Empty;

            var elements = new List<string>();
            if (over.PartitionBy?.Any() == true)
                elements.Add("PARTITION BY " + ColumnsSql(over.PartitionBy));

            if (over.OrderBy?.Any() == true)
            {
                var orderBys = over.OrderBy.Select(OrderByColumnSql);
                elements.Add("ORDER BY " + string.Join(", ", orderBys));
            }

            // [ROWS|RANGE] [null|uint] TO [null|uint]
            if (over.Extent != null)
                elements.Add(OverExtentSql(over.Extent));

            return $" OVER({string.Join(" ", elements)})";
        }

        private string OverExtentSql(OverExtent e) =>
            $"{e.ExtentType.ToString().ToUpperInvariant()} {(e.LowerBound?.ToString() ?? "null")} TO {(e.UpperBound?.ToString() ?? "null")}";

        private string ArithmeticChainSql(ArithmeticChain x)
        {
            var sb = new StringBuilder();
            sb.Append(ColumnSql(x.LeftColumn));
            if (x.Operations != null)
            {
                foreach (var o in x.Operations)
                    sb.Append($"{ArithmeticOperatorSql(o.Operator)}{ColumnSql(o.RightColumn)}");
            }

            return sb.ToString();
        }

        private string ArithmeticOperationSql(ArithmeticOperation o) =>
            $"{ArithmeticOperatorSql(o.Operator)}{ColumnSql(o.RightColumn)}";

        private string ArithmeticOperatorSql(ArithmeticOperator o) 
        {
            switch (o)
            {
                case ArithmeticOperator.Plus: return "+";
                case ArithmeticOperator.Minus: return "-";
                case ArithmeticOperator.Multiply: return "*";
                case ArithmeticOperator.Divide: return "/";
                case ArithmeticOperator.Modulo: return "%";
                case ArithmeticOperator.Concat: return "||";
                default: throw new NotSupportedException("Unkown arithmetic operator: " + o.ToString());
            }
        }

        private string CaseColumnSql(CaseColumn c)
        {
            var sb = new StringBuilder();
            sb.Append("CASE");

            foreach (var b in c.Branches)
            {
                sb.Append(" WHEN ");
                sb.Append(ConditionSql(b.Condition));
                sb.Append(" THEN ");
                sb.Append(ColumnSql(b.Column));
            }

            if (c.DefaultColumn != null)
            {
                sb.Append(" ELSE ");
                sb.Append(ColumnSql(c.DefaultColumn));
            }

            sb.Append(" END");

            return sb.ToString();
        }

        private string CaseBranchSql(CaseBranch b) =>
            $"WHEN {ConditionSql(b.Condition)} THEN {ColumnSql(b.Column)}";

        private string SqlIdentifierSql(IEnumerable<SqlIdentifier> i) =>
            string.Join(".", i.Select(SqlIdentifierSql));

        private string SqlIdentifierSql(SqlIdentifier i) =>
            i.IsBracketed ? $"[{i.Name}]" : i.Name;

        private string LiteralColumnSql(LiteralColumnBase c)
        {
            if (c is IntegerColumn i)
                return i.Value.ToString();
            if (c is DecimalColumn m)
                return m.Value.ToString();
            if (c is StringColumn s)
                return $"'{s.Value}'";
            if (c is DateColumn d)
                return $"{{d '{d.Value:yyyy-MM-dd}'}}";
            if (c is DateTimeColumn dt)
                return $"{{ts '{dt.Value:yyyy-MM-dd HH:mm:ss.FFFFFFF}'}}";
            if (c is TimeColumn t)
                return $"{{t '{t.Value}'}}";

            throw new NotSupportedException("Unknown literal column type: " + c.GetType().Name);
        }
    

        private string SelectStatementSql(SelectStatement s)
        {
            var sb = new StringBuilder();

            if (s.WithSelects?.Any() == true)
            {
                sb.Append("WITH ");
                var withs = s.WithSelects.Select(CteSql);
                sb.Append(string.Join(", ", withs));
                sb.Append(" ");
            }

            sb.Append(SelectsSql(s.Selects));

            sb.Append(OrderBySql(s.OrderBy));

            sb.Append(SelectOptionsSql(s.Options));

            return sb.ToString();
        }

        private string SelectsSql(List<Select> selects)
        {
            var strings = selects.Select(SelectSql);
            return string.Join(" ", strings);
        }

        private string SelectSql(Select s)
        {
            var sb = new StringBuilder();
            sb.Append(SetModifierSql(s.SetModifier));
            sb.Append("SELECT ");
            sb.Append(SelectTopSql(s.Top));
            if (s.Distinct)
                sb.Append("DISTINCT ");

            var cols = s.Columns.Select(ColumnSql);
            sb.Append(string.Join(", ", cols));

            if (s.From?.Any() == true)
            {
                sb.Append(" FROM ");
                var relations = s.From.Select(RelationSql);
                sb.Append(string.Join(", ", relations));
            }

            sb.Append(PivotSql(s.Pivot));

            if (s.WhereCondition != null)
                sb.Append(" WHERE " + ConditionSql(s.WhereCondition));

            sb.Append(GroupBySql(s.GroupBy));

            if (s.HavingCondition != null)
                sb.Append(" HAVING " + ConditionSql(s.HavingCondition));

            return sb.ToString();
        }

        private string GroupBySql(List<GroupingSet> g)
        {
            if (g?.Any() != true)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append(" GROUP BY ");
            if (g.Count == 1)
                sb.Append(GroupingSetSql(g[0], false));
            else
            {
                var setStrings = g.Select(x => GroupingSetSql(x, true));
                string fullText = $"GROUPING SETS ({string.Join(", ", setStrings)})";
                sb.Append(fullText);
            }

            return sb.ToString();
        }

        private string CteSql(CteSelect s)
        {
            var sb = new StringBuilder();
            sb.Append(s.Alias.Name);
            sb.Append(" AS (");
            sb.Append(SelectsSql(s.Selects));
            sb.Append(")");

            return sb.ToString();
        }

        private string OrderBySql (OrderBy o)
        {
            if (o == null) return string.Empty;

            var sb = new StringBuilder();
            sb.Append(" ORDER BY ");
            if (o.Columns.Any() == true)
            {
                var orders = o.Columns.Select(OrderByColumnSql);
                sb.Append(string.Join(", ", orders));
            }

            if (o.Offset != null)
                sb.Append($" OFFSET {ColumnSql(o.Offset)} ROWS");

            if (o.Fetch != null)
                sb.Append($" FETCH NEXT {ColumnSql(o.Fetch)} ROWS ONLY");

            return sb.ToString();
        }

        private string OrderByColumnSql(OrderByColumn o) =>
            $"{ColumnSql(o.Column)}{(o.Collate == null ? string.Empty : " COLLATE " + o.Collate)} {o.SortOrder.ToString().ToUpper()}";

        private string SelectOptionsSql(SelectOptions o)
        {
            if (o == null) return string.Empty;

            return $" OPTION ({string.Join(",", o.Options)})";
        }

        private string SetModifierSql(SetModifier s)
        {
            switch (s)
            {
                case SetModifier.None:
                    return String.Empty;
                case SetModifier.UnionAll:
                    return "UNION ALL ";
                case SetModifier.Union:
                case SetModifier.Intersect:
                case SetModifier.Except:
                    return s.ToString().ToUpperInvariant() + " ";
                default:
                    throw new NotSupportedException("Unknown SetModifier value: " + s.ToString());
            }
        }

        private string SelectTopSql(SelectTop t)
        {
            if (t == null)
                return string.Empty;

            string col = ColumnSql(t.Top);
            string percent = t.Percent ? " PERCENT" : string.Empty;
            string withTies = t.WithTies ? " WITH TIES" : string.Empty;

            return $"TOP {col}{percent}{withTies} ";
        }
    }
}
