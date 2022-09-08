using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common
{
    public interface IMiscellaneousColumnWriter
    {
        string ColumnSql(Column column);
    }
}
