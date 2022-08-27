using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Columns.Components
{
    public interface IArithmeticWriter
    {
        string ArithmeticSql(ArithmeticChain a);
    }
}
