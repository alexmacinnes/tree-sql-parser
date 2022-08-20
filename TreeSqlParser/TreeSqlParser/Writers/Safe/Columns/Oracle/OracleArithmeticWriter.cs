using TreeSqlParser.Writers.Safe.Columns.Components;

namespace TreeSqlParser.Writers.Safe.Columns.Oracle
{
    public class OracleArithmeticWriter : ArithmeticWriter
    {
        public OracleArithmeticWriter(IColumnWriter columnWriter) : base(columnWriter)
        { }

        protected override string ModuloReplacementFunction => "MOD";
    }
}
