using TreeSqlParser.Writers.Safe.Columns.Components;

namespace TreeSqlParser.Writers.Safe.Columns.Oracle
{
    public class OracleArithmeticWriter : ArithmeticWriter
    {
        public OracleArithmeticWriter(SafeSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string ModuloReplacementFunction => "MOD";
    }
}
