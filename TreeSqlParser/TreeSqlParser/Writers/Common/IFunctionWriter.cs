using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common
{
    public interface IFunctionWriter
    {
        string FunctionSql(FunctionColumn column);
    }
}
