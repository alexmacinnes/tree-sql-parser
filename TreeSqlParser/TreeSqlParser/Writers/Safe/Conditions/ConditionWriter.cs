using System;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Enums;

namespace TreeSqlParser.Writers.Safe.Conditions
{
    public class ConditionWriter : IConditionWriter
    {
        private readonly ISqlWriter sqlWriter;

        private string Sql(SqlElement x) => sqlWriter.GenerateSql(x);

        public ConditionWriter(ISqlWriter sqlWriter)
        {
            this.sqlWriter = sqlWriter;
        }

        public string ConditionSql(Condition c) => c switch 
        {
            ComparisonCondition x => ComparisonSql(x),
            ConditionChain x => ConditionChainSql(x),
            NotCondition x => NotSql(x),
            BracketCondition x => BracketSql(x),
            InListCondition x => InListSql(x),
            InSubselectCondition x => InSubselectSql(x),
            ExistsCondition x => ExistsSql(x),
            BetweenCondition x => BetweenSql(x),
            IsNullCondition x => IsNullSql(x),
            NotNullCondition x => NotNullSql(x),
            _ => throw new InvalidCastException("Unsupported condition type")
        };

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

        private string LogicSql(LogicCondition c) => c switch
        {
            LogicCondition.And => "AND",
            LogicCondition.Or => "OR",
            _ => throw new InvalidOperationException("Unkown Logic Condition")
        };

        private string ComparisonSql(ComparisonCondition x) =>
            $"{Sql(x.LeftColumn)} {ComparisonOperatorSql(x.Comparison)} {Sql(x.RightColumn)}";

        private string ComparisonOperatorSql(ColumnComparison c) => c switch
        {
            ColumnComparison.Equals => "=",
            ColumnComparison.LessThan => "<",
            ColumnComparison.LessThanOrEqual => "<=",
            ColumnComparison.GreaterThan => ">",
            ColumnComparison.GreaterThanOrEqual => ">=",
            ColumnComparison.NotEquals => "<>",
            ColumnComparison.Like => "LIKE",
            _ => throw new InvalidOperationException("Unknown column comparison")
        };
        
    }
}
