using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common
{
    public interface IAggregationWriter
    {
        string AggregationSql(AggregatedColumn column);
    }
}
