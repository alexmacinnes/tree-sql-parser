using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common
{
    public interface IColumnWriter
    {
        string ColumnSql(Column c);
    }
}
