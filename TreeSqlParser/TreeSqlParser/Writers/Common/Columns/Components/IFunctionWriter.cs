using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Columns.Components
{
    public interface IFunctionWriter
    {
        string FunctionSql(FunctionColumn column);
    }
}
