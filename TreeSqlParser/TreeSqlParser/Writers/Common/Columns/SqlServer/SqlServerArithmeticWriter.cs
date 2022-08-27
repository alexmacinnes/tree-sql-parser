using TreeSqlParser.Writers.Safe.Columns.Components;

namespace TreeSqlParser.Writers.Safe.Columns.SqlServer
{
    public class SqlServerArithmeticWriter : ArithmeticWriter
    {
        public SqlServerArithmeticWriter(SafeSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string ModuloReplacementFunction => null;
    }
}
