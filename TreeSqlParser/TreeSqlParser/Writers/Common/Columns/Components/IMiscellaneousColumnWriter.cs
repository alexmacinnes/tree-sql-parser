using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Columns.Components
{
    public interface IMiscellaneousColumnWriter
    {
        string ColumnSql(Column column);
    }
}
