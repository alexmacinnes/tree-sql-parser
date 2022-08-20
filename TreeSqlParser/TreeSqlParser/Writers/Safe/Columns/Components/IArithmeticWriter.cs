using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public interface IArithmeticWriter
    {
        string ArithmeticSql(ArithmeticChain a);
    }
}
