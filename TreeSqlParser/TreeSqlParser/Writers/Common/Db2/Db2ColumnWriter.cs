namespace TreeSqlParser.Writers.Common.Db2
{
    public class Db2ColumnWriter : ColumnWriter
    {
        public Db2ColumnWriter(CommonSqlWriter sqlWriter)
        {
            this.literalWriter = new Db2LiteralWriter();
            this.aggregationWriter = new AggregationWriter(sqlWriter);
            this.arithmeticWriter = new ArithmeticWriter(sqlWriter);
            this.functionWriter = new Db2FunctionWriter(sqlWriter);
            this.castWriter = new Db2CastWriter(sqlWriter);
            this.miscellaneousWriter = new Db2MiscellaneousColumnWriter(sqlWriter);
        }
    }
}
