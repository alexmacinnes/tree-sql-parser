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
        #region Init

        private Dictionary<Type, Func<SqlElement, string>> sqlFuncs;

        /// <summary>SQL Server does not have a concept of Boolean values.
        /// If this is set, any TRUE or FALSE columns in the tree will be emitted as 1 or 0.
        /// If not, throw an exception.
        /// Default is false.       
        /// </summary>
        public bool ConvertBoolColumnsToInteger { get; set; } = false;

        public FullSqlServerWriter()
        {
            sqlFuncs = new Dictionary<Type, Func<SqlElement, string>>
            {
                { typeof(SqlRootElement), (x) => RootElementSql((SqlRootElement)x) },

                //Analytics.cs
                { typeof(Over), (x) => OverSql((Over)x) },
                { typeof(OverExtent), (x) => OverExtentSql((OverExtent)x) },

                //Columns.cs
                { typeof(AggregatedColumn), (x) => AggregatedColumnSql((AggregatedColumn)x) },
                { typeof(ArithmeticChain), (x) => ArithmeticChainSql((ArithmeticChain)x) },
                { typeof(AliasColumn), (x) => AliasColumnSql((AliasColumn)x) },
                { typeof(ArithmeticOperation), (x) => ArithmeticOperationSql((ArithmeticOperation)x) },
                { typeof(BracketedColumn), (x) => BracketedColumnSql((BracketedColumn)x) },
                { typeof(CaseBranch), (x) => CaseBranchSql((CaseBranch)x) },
                { typeof(CaseColumn), (x) => CaseColumnSql((CaseColumn)x) },
                { typeof(CastColumn), (x) => CastColumnSql((CastColumn)x) },
                { typeof(ConvertColumn), (x) => ConvertColumnSql((ConvertColumn)x) },
                { typeof(DateColumn), (x) => DateColumnSql((DateColumn)x) },
                { typeof(DateTimeColumn), (x) => DateTimeColumnSql((DateTimeColumn)x) },
                { typeof(DecimalColumn), (x) => DecimalColumnSql((DecimalColumn)x) },
                { typeof(BoolColumn), (x) => BoolColumnSql((BoolColumn)x) },
                { typeof(FunctionColumn), (x) => FunctionColumnSql((FunctionColumn)x) },
                { typeof(IifColumn), (x) => IifColumnSql((IifColumn)x) },
                { typeof(IntegerColumn), (x) => IntegerColumnSql((IntegerColumn)x) },
                { typeof(NullColumn), (x) => NullColumnSql() },
                { typeof(OrderByColumn), (x) => OrderByColumnSql((OrderByColumn)x) },
                { typeof(OverColumn), (x) => OverColumnSql((OverColumn)x) },
                { typeof(ParseColumn), (x) => ParseColumnSql((ParseColumn)x) },
                { typeof(PrimitiveColumn), (x) => PrimitiveColumnSql((PrimitiveColumn)x) },
                { typeof(StarColumn), (x) => StarColumnSql((StarColumn)x) },
                { typeof(StringColumn), (x) => StringColumnSql((StringColumn)x) },
                { typeof(SubselectColumn), (x) => SubselectColumnSql((SubselectColumn)x) },
                { typeof(TimeColumn), (x) => TimeColumnSql((TimeColumn)x) },
                { typeof(VariableColumn), (x) => VariableColumnSql((VariableColumn)x) },
                
                //Conditions.cs
                { typeof(BetweenCondition), (x) => BetweenConditionSql((BetweenCondition)x) },
                { typeof(BracketCondition), (x) => BracketedConditionSql((BracketCondition)x) },
                { typeof(ComparisonCondition), (x) => ComparisonConditionSql((ComparisonCondition)x) },
                { typeof(ConditionChain), (x) => ConditionChainSql((ConditionChain)x) },
                { typeof(ExistsCondition), (x) => ExistsConditionSql((ExistsCondition)x) },
                { typeof(InListCondition), (x) => InListConditionSql((InListCondition)x) },
                { typeof(InSubselectCondition), (x) => InSubselectConditionSql((InSubselectCondition)x) },
                { typeof(IsNullCondition), (x) => IsNullConditionSql((IsNullCondition)x) },
                { typeof(LogicOperation), (x) => LogicOperationSql((LogicOperation)x) },
                { typeof(NotCondition), (x) => NotConditionSql((NotCondition)x) },
                { typeof(NotNullCondition), (x) => NotNullConditionSql((NotNullCondition)x) },
                { typeof(SetCondition), (x) => SetConditionSql((SetCondition)x) },

                //Grouping.cs
                { typeof(GroupingSet), (x) => GroupingSetSql((GroupingSet)x) },

                //Hints.cs
                { typeof(SelectOptions), (x) => SelectOptionsSql((SelectOptions)x) },

                //Pivoting.cs
                { typeof(Pivot), (x) => PivotSql((Pivot)x) },

                //Relations.cs
                { typeof(BracketedRelation), (x) => BracketedRelationSql((BracketedRelation)x) },
                { typeof(Join), (x) => JoinSql((Join)x) },
                { typeof(JoinChain), (x) => JoinChainSql((JoinChain)x) },
                { typeof(SubselectRelation), (x) => SubselectRelationSql((SubselectRelation)x) },
                { typeof(Table), (x) => TableSql((Table)x) },

                //Selects.cs
                { typeof(CteSelect), (x) => CteSql((CteSelect)x) },
                { typeof(OrderBy), (x) => OrderBySql((OrderBy)x) },
                { typeof(Select), (x) => SelectSql((Select)x) },
                { typeof(SelectStatement), (x) => SelectStatementSql((SelectStatement)x) },
                { typeof(SelectTop), (x) => SelectTopSql((SelectTop)x) },
            };
        }

        #endregion

        #region Public

        public string GenerateSql(SqlElement x)
        {
            if (sqlFuncs.TryGetValue(x.GetType(), out var sqlFunc))
                return sqlFunc(x);

            throw new NotSupportedException("Unknown SqlElement tyoe: " + x.GetType().Name);
        }

        #endregion

        #region Protected

        protected virtual string AliasColumnSql(AliasColumn x) =>
            $"{ColumnSql(x.InnerColumn)} AS {SqlIdentifierSql(x.Alias)}";

        protected virtual string AggregatedColumnSql(AggregatedColumn x) =>
            $"{x.Aggregation.ToString().ToUpper()}({(x.Distinct ? "DISTINCT " : "")}{ColumnsSql(x.InnerColumns)})";

        protected virtual string ArithmeticChainSql(ArithmeticChain x)
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

        protected virtual string ArithmeticOperationSql(ArithmeticOperation x) =>
            $"{ArithmeticOperatorSql(x.Operator)}{ColumnSql(x.RightColumn)}";

        protected virtual string ArithmeticOperatorSql(ArithmeticOperator x)
        {
            switch (x)
            {
                case ArithmeticOperator.Plus: return "+";
                case ArithmeticOperator.Minus: return "-";
                case ArithmeticOperator.Multiply: return "*";
                case ArithmeticOperator.Divide: return "/";
                case ArithmeticOperator.Modulo: return "%";
                case ArithmeticOperator.Concat: return "||";
                default: throw new NotSupportedException("Unkown arithmetic operator: " + x.ToString());
            }
        }

        protected virtual string BetweenConditionSql(BetweenCondition x) =>
            $"{ColumnSql(x.LeftColumn)} BETWEEN {ColumnSql(x.LowerBound)} AND {ColumnSql(x.UpperBound)}";

        protected virtual string BracketedColumnSql(BracketedColumn x) =>
            $"({ColumnSql(x.InnerColumn)})";

        protected virtual string BracketedConditionSql(BracketCondition x) =>
            $"({ConditionSql(x.InnerCondition)})";

        protected virtual string BracketedRelationSql(BracketedRelation x) =>
            $"({RelationSql(x.InnerRelation)})";

        protected virtual string CaseBranchSql(CaseBranch x) =>
            $"WHEN {ConditionSql(x.Condition)} THEN {ColumnSql(x.Column)}";

        protected virtual string CaseColumnSql(CaseColumn x)
        {
            var sb = new StringBuilder();
            sb.Append("CASE");

            foreach (var b in x.Branches)
            {
                sb.Append($" {CaseBranchSql(b)}");
            }

            if (x.DefaultColumn != null)
            {
                sb.Append(" ELSE ");
                sb.Append(ColumnSql(x.DefaultColumn));
            }

            sb.Append(" END");

            return sb.ToString();
        }

        protected virtual string CastColumnSql(CastColumn x) =>
            $"{(x.TryCast ? "TRY_" : string.Empty)}CAST({ColumnSql(x.Column)} AS {x.DataType.Value})";

        protected virtual string ColumnSql(Column x) => 
            GenerateSql(x);

        protected virtual string ColumnsSql(IEnumerable<Column> x) => 
            string.Join(", ", x.Select(ColumnSql));

        protected virtual string ComparisonConditionSql(ComparisonCondition x) =>
            $"{ColumnSql(x.LeftColumn)}{ComparisonSql(x.Comparison)}{ColumnSql(x.RightColumn)}";

        protected virtual string ComparisonSql(ColumnComparison c)
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

        protected virtual string ConditionChainSql(ConditionChain x)
        {
            var sb = new StringBuilder();
            sb.Append(ConditionSql(x.LeftCondition));

            if (x.OtherConditions != null)
                foreach (var c in x.OtherConditions)
                    sb.Append($" {LogicOperationSql(c)}");

            return sb.ToString();
        }

        private string ConditionSql(Condition x) => 
            GenerateSql(x);

        protected virtual string ConvertColumnSql(ConvertColumn x) =>
            $"{(x.TryConvert ? "TRY_" : string.Empty)}CONVERT({x.DataType.Value}, {ColumnSql(x.Column)}{(x.Style == null ? string.Empty : ", " + ColumnSql(x.Style))})";

        protected virtual string CteSql(CteSelect s)
        {
            var sb = new StringBuilder();
            sb.Append(s.Alias.Name);
            sb.Append(" AS (");
            sb.Append(SelectsSql(s.Selects));
            sb.Append(")");

            return sb.ToString();
        }

        protected virtual string DateColumnSql(DateColumn x) =>
            $"{{d '{x.Value:yyyy-MM-dd}'}}";

        protected virtual string DateTimeColumnSql(DateTimeColumn x) =>
            $"{{ts '{x.Value:yyyy-MM-dd HH:mm:ss.FFFFFFF}'}}";

        protected virtual string DecimalColumnSql(DecimalColumn x) =>
            x.Value.ToString();

        protected virtual string BoolColumnSql(BoolColumn x) =>
            ConvertBoolColumnsToInteger
            ? (x.Value ? "1" : "0")
            : throw new NotSupportedException("Boolean columns are not directly supported on SQL Server.");

        protected virtual string ExistsConditionSql(ExistsCondition x) =>
            $"EXISTS ({SelectStatementSql(x.Subselect)})";

        protected virtual string FunctionColumnSql(FunctionColumn x) =>
            $"{SqlIdentifierSql(x.Name)}({ColumnsSql(x.Parameters)})";

        protected virtual string GroupBySql(List<GroupingSet> x)
        {
            if (x?.Any() != true)
                return string.Empty;

            var sb = new StringBuilder();
            sb.Append(" GROUP BY ");
            if (x.Count == 1)
                sb.Append(GroupingSetSql(x[0], false));
            else
            {
                var setStrings = x.Select(s => GroupingSetSql(s, forceBrackets: true));
                string fullText = $"GROUPING SETS ({string.Join(", ", setStrings)})";
                sb.Append(fullText);
            }

            return sb.ToString();
        }

        protected virtual string GroupingSetSql(GroupingSet g, bool forceBrackets = false)
        {
            string columns = ColumnsSql(g.Columns);
            string typeText = g.SetType == GroupingSetType.Columns ? null : g.SetType.ToString().ToUpper() + " ";
            bool brackets = forceBrackets || typeText != null;

            if (brackets)
                return $"{typeText}({columns})";
            else
                return columns;
        }

        protected virtual string IifColumnSql(IifColumn x) =>
            $"IIF({ConditionSql(x.Condition)}, {ColumnSql(x.TrueColumn)}, {ColumnSql(x.FalseColumn)})";

        protected virtual string InListConditionSql(InListCondition x) =>
            $"{ColumnSql(x.LeftColumn)} IN ({string.Join(",", x.RightColumns.Select(ColumnSql))})";

        protected virtual string InSubselectConditionSql(InSubselectCondition x) =>
            $"{ColumnSql(x.LeftColumn)} IN ({SelectStatementSql(x.Subselect)})";

        protected virtual string IntegerColumnSql(IntegerColumn x) =>
            x.Value.ToString();

        protected virtual string IsNullConditionSql(IsNullCondition x) =>
            $"{ColumnSql(x.LeftColumn)} IS NULL";

        protected virtual string JoinChainSql(JoinChain x)
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

        protected virtual string JoinSql(Join x) =>
            $"{JoinTypeSql(x.JoinType)} {RelationSql(x.RightRelation)}" +
            (x.Condition == null ? String.Empty : $" ON {ConditionSql(x.Condition)}");

        protected virtual string JoinTypeSql(JoinType x)
        {
            switch (x)
            {
                case JoinType.InnerJoin: return "INNER JOIN";
                case JoinType.LeftJoin: return "LEFT JOIN";
                case JoinType.RightJoin: return "RIGHT JOIN";
                case JoinType.FullJoin: return "FULL JOIN";
                case JoinType.CrossJoin: return "CROSS JOIN";
                case JoinType.CrossApply: return "CROSS APPLY";
                case JoinType.OuterApply: return "OUTER APPLY";
                default: throw new NotSupportedException("Unknown JoinType: " + x.ToString());
            }
        }

        protected virtual string LogicOperationSql(LogicOperation o) =>
            $"{o.LogicCondition.ToString().ToUpperInvariant()} {ConditionSql(o.RightCondition)}";

        protected virtual string NotConditionSql(NotCondition x) =>
            $"NOT {ConditionSql(x.InnerCondition)}";

        protected virtual string NotNullConditionSql(NotNullCondition x) =>
            $"{ColumnSql(x.LeftColumn)} IS NOT NULL";

        protected virtual string NullColumnSql() => 
            "NULL";

        protected virtual string OrderByColumnSql(OrderByColumn x) =>
            $"{ColumnSql(x.Column)}{(x.Collate == null ? string.Empty : " COLLATE " + x.Collate)} {x.SortOrder.ToString().ToUpper()}";

        protected virtual string OrderBySql(OrderBy o)
        {
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

        protected virtual string OverColumnSql(OverColumn x) =>
            $"{ColumnSql(x.Column)}{OverSql(x.Over)}";

        protected virtual string OverExtentSql(OverExtent e) =>
           $"{e.ExtentType.ToString().ToUpperInvariant()} {(e.LowerBound?.ToString() ?? "null")} TO {(e.UpperBound?.ToString() ?? "null")}";

        protected virtual string OverSql(Over over)
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

        protected virtual string ParseColumnSql(ParseColumn x) =>
            $"{(x.TryParse ? "TRY_" : string.Empty)}PARSE({ColumnSql(x.Column)} AS {x.DataType.Value}{(x.Culture == null ? string.Empty : " USING '" + x.Culture + "'")})";

        protected virtual string PivotSql(Pivot p)
        {
            return $" PIVOT ({ColumnSql(p.AggregatedColumn)} FOR {ColumnSql(p.PivotColumn)} IN ({ColumnsSql(p.PivotValues)})) AS {SqlIdentifierSql(p.Alias)}";
        }

        protected virtual string PrimitiveColumnSql(PrimitiveColumn x) =>
            x.TableAlias == null
            ? SqlIdentifierSql(x.Name)
            : $"{SqlIdentifierSql(x.TableAlias)}.{SqlIdentifierSql(x.Name)}";

        private string RelationSql(Relation x) => 
            GenerateSql(x);

        protected virtual string RootElementSql(SqlRootElement x) => 
            GenerateSql(x.Child);

        protected virtual string SelectOptionsSql(SelectOptions o) =>
            $" OPTION ({string.Join(",", o.Options)})";

        protected virtual string SelectSql(Select s)
        {
            var sb = new StringBuilder();
            sb.Append(SetModifierSql(s.SetModifier));
            sb.Append("SELECT ");
            if (s.Top != null)
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

            if (s.Pivot != null)
                sb.Append(PivotSql(s.Pivot));

            if (s.WhereCondition != null)
                sb.Append(" WHERE " + ConditionSql(s.WhereCondition));

            sb.Append(GroupBySql(s.GroupBy));

            if (s.HavingCondition != null)
                sb.Append(" HAVING " + ConditionSql(s.HavingCondition));

            return sb.ToString();
        }

        protected virtual string SelectsSql(List<Select> selects)
        {
            var strings = selects.Select(SelectSql);
            return string.Join(" ", strings);
        }

        protected virtual string SelectStatementSql(SelectStatement s)
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

            if (s.OrderBy != null)
                sb.Append(OrderBySql(s.OrderBy));

            if (s.Options != null)
                sb.Append(SelectOptionsSql(s.Options));

            return sb.ToString();
        }

        protected virtual string SelectTopSql(SelectTop t)
        {
            string col = ColumnSql(t.Top);
            string percent = t.Percent ? " PERCENT" : string.Empty;
            string withTies = t.WithTies ? " WITH TIES" : string.Empty;

            return $"TOP {col}{percent}{withTies} ";
        }

        protected virtual string SetConditionSql(SetCondition x) =>
           $"{ColumnSql(x.LeftColumn)} {ComparisonSql(x.Comparison)} {x.SetConditionType.ToString().ToUpperInvariant()} {ColumnSql(x.SubselectColumn)}";

        protected virtual string SetModifierSql(SetModifier s)
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

        protected virtual string SqlIdentifierSql(IEnumerable<SqlIdentifier> x) =>
            string.Join(".", x.Select(SqlIdentifierSql));

        protected virtual string SqlIdentifierSql(SqlIdentifier x) =>
            x.IsBracketed ? $"[{x.Name}]" : x.Name;

        protected virtual string StarColumnSql(StarColumn x) => 
            x.TableAlias == null ? "*" : $"{SqlIdentifierSql(x.TableAlias)}.*";

        protected virtual string StringColumnSql(StringColumn x) =>
            $"'{x.Value}'";

        protected virtual string SubselectColumnSql(SubselectColumn x) =>
            $"({SelectStatementSql(x.InnerSelect)})";

        protected virtual string SubselectRelationSql(SubselectRelation x) =>
            $"({SelectStatementSql(x.InnerSelect)}) AS {SqlIdentifierSql(x.Alias)}";

        protected virtual string TableSql(Table x) =>
           $"{SqlIdentifierSql(x.Name)}" + (x.Alias == null ? string.Empty : $" AS {SqlIdentifierSql(x.Alias)}");

        protected virtual string TimeColumnSql(TimeColumn x) =>
            $"{{t '{x.Value}'}}";

        protected virtual string VariableColumnSql(VariableColumn x) =>
            $"@{x.VariableName}";

        #endregion
    }
}
