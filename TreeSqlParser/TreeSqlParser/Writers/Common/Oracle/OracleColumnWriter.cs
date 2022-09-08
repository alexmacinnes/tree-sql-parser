namespace TreeSqlParser.Writers.Common.Oracle
{
    public class OracleColumnWriter : ColumnWriter
    {
        public OracleColumnWriter(CommonSqlWriter sqlWriter)
        {
            this.literalWriter = new OracleLiteralWriter();
            this.aggregationWriter = new AggregationWriter(sqlWriter);
            this.arithmeticWriter = new OracleArithmeticWriter(sqlWriter);
            this.functionWriter = new OracleFunctionWriter(sqlWriter);
            this.castWriter = new OracleCastWriter(sqlWriter);
            this.miscellaneousWriter = new MiscellaneousColumnWriter(sqlWriter);
        }
    }
}
