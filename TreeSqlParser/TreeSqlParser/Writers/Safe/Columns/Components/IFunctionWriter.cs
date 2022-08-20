using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public interface IFunctionWriter
    {
        public string FunctionSql(FunctionColumn column);
    }
}
