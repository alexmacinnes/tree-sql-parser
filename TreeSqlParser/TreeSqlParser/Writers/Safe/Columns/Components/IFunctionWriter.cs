using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public interface IFunctionWriter
    {
        string FunctionSql(FunctionColumn column);
    }
}
