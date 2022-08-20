using TreeSqlParser.Model.Columns;
using TreeSqlParser.Writers.Safe.Columns.Components;

namespace TreeSqlParser.Writers.Safe.Columns
{
    public class ColumnWriter : IColumnWriter
    {
        protected ILiteralWriter literalWriter;

        protected IArithmeticWriter arithmeticWriter;

        protected IFunctionWriter functionWriter;

        protected IAggregationWriter aggregationWriter;

        protected IMiscellaneousColumnWriter miscellaneousWriter;

        protected ICastWriter castWriter;

        public string ColumnSql(Column c) => c switch
        {
            LiteralColumnBase x => literalWriter.LiteralSql(x),
            ArithmeticChain x => arithmeticWriter.ArithmeticSql(x),
            FunctionColumn x => functionWriter.FunctionSql(x),
            AggregatedColumn x => aggregationWriter.AggregationSql(x),
            CastColumn x => castWriter.CastSql(x),
            _ => miscellaneousWriter.ColumnSql(c)
        };
    }
}
