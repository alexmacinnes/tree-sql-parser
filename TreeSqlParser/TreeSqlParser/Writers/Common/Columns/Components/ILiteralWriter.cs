using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Columns.Components
{
    public interface ILiteralWriter
    {
        string LiteralSql(LiteralColumnBase x);
    }
}
