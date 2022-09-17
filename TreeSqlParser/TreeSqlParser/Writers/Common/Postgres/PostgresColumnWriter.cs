namespace TreeSqlParser.Writers.Common.Postgres
{
    public class PostgresColumnWriter : ColumnWriter
    {
        public PostgresColumnWriter(CommonSqlWriter sqlWriter)
        {
            this.literalWriter = new PostgresLiteralWriter();
            this.aggregationWriter = new AggregationWriter(sqlWriter);
            this.arithmeticWriter = new ArithmeticWriter(sqlWriter);
            this.functionWriter = new PostgresFunctionWriter(sqlWriter);
            this.castWriter = new PostgresCastWriter(sqlWriter);
            this.miscellaneousWriter = new MiscellaneousColumnWriter(sqlWriter);
        }
    }
}
