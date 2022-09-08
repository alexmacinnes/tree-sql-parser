using TreeSqlParser.Model;

namespace TreeSqlParser.Writers.Common
{
    public interface IIdentifierWriter
    {
        string Delimit(SqlIdentifier i);
    }
}
