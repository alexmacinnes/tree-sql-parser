using TreeSqlParser.Writers.Safe.Columns.Components;
using TreeSqlParser.Writers.Safe.Identifiers;

namespace TreeSqlParser.Writers.Safe.Columns.Oracle
{
    public class OracleColumnWriter : ColumnWriter
    {
        public OracleColumnWriter(SafeSqlWriter sqlWriter)
        {
            this.literalWriter = new OracleLiteralWriter();
            this.aggregationWriter = new AggregationWriter(sqlWriter);
            this.arithmeticWriter = new OracleArithmeticWriter(sqlWriter);
            this.functionWriter = new OracleFunctionWriter(sqlWriter);
            this.castWriter = new OracleCastWriter(sqlWriter);
            this.miscellaneousWriter = new MiscellaneousColumnWriter(sqlWriter, "IIF");
        }
    }
}
