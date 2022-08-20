using TreeSqlParser.Writers.Safe.Columns.Components;

namespace TreeSqlParser.Writers.Safe.Columns.SqlServer
{
    public class SqlServerArithmeticWriter : ArithmeticWriter
    {
        public SqlServerArithmeticWriter(IColumnWriter columnWriter) : base(columnWriter)
        { }

        protected override string ModuloReplacementFunction => null;
    }
}
