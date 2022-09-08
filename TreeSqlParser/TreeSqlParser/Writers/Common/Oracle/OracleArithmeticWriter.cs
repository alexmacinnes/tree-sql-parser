namespace TreeSqlParser.Writers.Common.Oracle
{
    public class OracleArithmeticWriter : ArithmeticWriter
    {
        public OracleArithmeticWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string ModuloReplacementFunction => "MOD";
    }
}
