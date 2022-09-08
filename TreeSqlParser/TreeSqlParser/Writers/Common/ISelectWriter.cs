using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Writers.Common
{
    public interface ISelectWriter
    {
        string SelectStatementSql(SelectStatement s);

        string SelectSql(Select s);
    }
}
