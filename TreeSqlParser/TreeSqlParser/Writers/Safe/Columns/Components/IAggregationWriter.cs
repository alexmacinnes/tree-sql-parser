using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public interface IAggregationWriter
    {
        public string AggregationSql(AggregatedColumn column);
    }
}
