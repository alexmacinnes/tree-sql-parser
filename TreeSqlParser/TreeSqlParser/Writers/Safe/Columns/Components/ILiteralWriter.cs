using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public interface ILiteralWriter
    {
        string LiteralSql(LiteralColumnBase x);
    }
}
