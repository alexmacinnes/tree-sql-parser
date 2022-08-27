using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Columns.Components
{
    public interface IAggregationWriter
    {
        string AggregationSql(AggregatedColumn column);
    }
}
