using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public interface IMiscellaneousColumnWriter
    {
        string ColumnSql(Column column);
    }
}
