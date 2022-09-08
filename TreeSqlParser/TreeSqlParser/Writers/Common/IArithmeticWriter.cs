using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common
{
    public interface IArithmeticWriter
    {
        string ArithmeticSql(ArithmeticChain a);
    }
}
