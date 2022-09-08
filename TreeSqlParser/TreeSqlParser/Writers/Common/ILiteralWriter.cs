using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common
{
    public interface ILiteralWriter
    {
        string LiteralSql(LiteralColumnBase x);
    }
}
