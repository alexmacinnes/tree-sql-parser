using System;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Enums;

namespace TreeSqlParser.Writers.Common.Conditions
{
    public class ConditionWriter : IConditionWriter
    {
        private readonly CommonSqlWriter sqlWriter;

        private string Sql(SqlElement x) => sqlWriter.GenerateSql(x);

        public ConditionWriter(CommonSqlWriter sqlWriter)
        {
            this.sqlWriter = sqlWriter;
        }

        public string ConditionSql(Condition c) 
        {
            if (c is ComparisonCondition comp) return ComparisonSql(comp);
            if (c is ConditionChain cond) return ConditionChainSql(cond);
            if (c is NotCondition n) return NotSql(n);
            if (c is BracketCondition br) return BracketSql(br);
            if (c is InListCondition inl) return InListSql(inl);
            if (c is InSubselectCondition ins) return InSubselectSql(ins);
            if (c is ExistsCondition e) return ExistsSql(e);
            if (c is BetweenCondition be) return BetweenSql(be);
            if (c is IsNullCondition nl) return IsNullSql(nl);
            if (c is NotNullCondition nnl) return NotNullSql(nnl);

            throw new InvalidCastException("Unsupported condition type");
        }

        private string NotNullSql(NotNullCondition x) =>
            $"{Sql(x.LeftColumn)} IS NOT NULL";

        private string IsNullSql(IsNullCondition x) =>
            $"{Sql(x.LeftColumn)} IS NULL";

        private string BetweenSql(BetweenCondition x) =>
            $"{Sql(x.LeftColumn)} BETWEEN {Sql(x.LowerBound)} AND {Sql(x.UpperBound)}";

        private string ExistsSql(ExistsCondition x) =>
            $"EXISTS ({Sql(x.Subselect)})";

        private string InSubselectSql(InSubselectCondition x) =>
            $"{Sql(x.LeftColumn)} IN ({Sql(x.Subselect)})";

        private string InListSql(InListCondition x) =>
            $"{Sql(x.LeftColumn)} IN ({string.Join(", ", x.RightColumns.Select(Sql))})";

        private string BracketSql(BracketCondition x) =>
            "(" + ConditionSql(x.InnerCondition) + ")";

        private string NotSql(NotCondition x) =>
            "NOT " + ConditionSql(x.InnerCondition);

        private string ConditionChainSql(ConditionChain x)
        {
            var sb = new StringBuilder(ConditionSql(x.LeftCondition));

            foreach (var c in x.OtherConditions)
                sb.Append($" {LogicSql(c.LogicCondition)} {ConditionSql(c.RightCondition)}");

            return sb.ToString();
        }

        private string LogicSql(LogicCondition c) 
        {
            switch (c)
            {
                case LogicCondition.And: return "AND";
                case LogicCondition.Or: return "OR";
                default: throw new InvalidOperationException("Unkown Logic Condition");
            }
        }

        private string ComparisonSql(ComparisonCondition x) =>
            $"{Sql(x.LeftColumn)} {ComparisonOperatorSql(x.Comparison)} {Sql(x.RightColumn)}";

        private string ComparisonOperatorSql(ColumnComparison c)
        {
            switch (c)
            {
                case ColumnComparison.Equals: return "=";
                case ColumnComparison.LessThan: return "<";
                case ColumnComparison.LessThanOrEqual: return "<=";
                case ColumnComparison.GreaterThan: return ">";
                case ColumnComparison.GreaterThanOrEqual: return ">=";
                case ColumnComparison.NotEquals: return "<>";
                case ColumnComparison.Like: return "LIKE";
                default: throw new InvalidOperationException("Unknown column comparison");
            }
        }
        
    }
}
