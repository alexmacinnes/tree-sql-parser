using TreeSqlParser.Writers.Common.Columns.Components;

namespace TreeSqlParser.Writers.Common.Columns.Oracle
{
    public class OracleArithmeticWriter : ArithmeticWriter
    {
        public OracleArithmeticWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string ModuloReplacementFunction => "MOD";
    }
}
