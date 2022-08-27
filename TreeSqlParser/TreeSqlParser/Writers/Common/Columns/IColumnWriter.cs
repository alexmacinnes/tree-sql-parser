using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns
{
    public interface IColumnWriter
    {
        string ColumnSql(Column c);
    }
}
