using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Columns
{
    public interface IColumnWriter
    {
        string ColumnSql(Column c);
    }
}
