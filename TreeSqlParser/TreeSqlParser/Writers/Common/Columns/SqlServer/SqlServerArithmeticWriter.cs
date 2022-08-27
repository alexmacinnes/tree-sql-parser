using TreeSqlParser.Writers.Common.Columns.Components;

namespace TreeSqlParser.Writers.Common.Columns.SqlServer
{
    public class SqlServerArithmeticWriter : ArithmeticWriter
    {
        public SqlServerArithmeticWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string ModuloReplacementFunction => null;
    }
}
