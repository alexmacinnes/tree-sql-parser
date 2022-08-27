using TreeSqlParser.Model.Columns;
using TreeSqlParser.Writers.Common.Columns.Components;

namespace TreeSqlParser.Writers.Common.Columns
{
    public class ColumnWriter : IColumnWriter
    {
        protected ILiteralWriter literalWriter;

        protected IArithmeticWriter arithmeticWriter;

        protected IFunctionWriter functionWriter;

        protected IAggregationWriter aggregationWriter;

        protected IMiscellaneousColumnWriter miscellaneousWriter;

        protected ICastWriter castWriter;

        public string ColumnSql(Column c) 
        {
            if (c is LiteralColumnBase lit)
                return literalWriter.LiteralSql(lit);
            if (c is ArithmeticChain arith)
                return arithmeticWriter.ArithmeticSql(arith);
            if (c is FunctionColumn func)
                return functionWriter.FunctionSql(func);
            if (c is AggregatedColumn agg)
                return aggregationWriter.AggregationSql(agg);
            if (c is CastColumn cst)
                return castWriter.CastSql(cst);
            return miscellaneousWriter.ColumnSql(c);
        }
    }
}
