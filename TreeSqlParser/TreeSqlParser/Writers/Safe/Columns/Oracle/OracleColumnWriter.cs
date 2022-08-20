using TreeSqlParser.Writers.Safe.Columns.Components;
using TreeSqlParser.Writers.Safe.Identifiers;

namespace TreeSqlParser.Writers.Safe.Columns.Oracle
{
    public class OracleColumnWriter : ColumnWriter
    {
        public OracleColumnWriter(ISqlWriter sqlWriter, IIdentifierWriter identifierWriter)
        {
            this.literalWriter = new OracleLiteralWriter();
            this.aggregationWriter = new AggregationWriter(this);
            this.arithmeticWriter = new OracleArithmeticWriter(this);
            this.functionWriter = new OracleFunctionWriter(this);
            this.castWriter = new OracleCastWriter(this);
            this.miscellaneousWriter = new MiscellaneousColumnWriter(sqlWriter, identifierWriter, "IIF");
        }
    }
}
