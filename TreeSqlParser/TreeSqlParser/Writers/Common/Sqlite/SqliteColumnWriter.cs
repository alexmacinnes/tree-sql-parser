namespace TreeSqlParser.Writers.Common.Sqlite
{
    public class SqliteColumnWriter : ColumnWriter
    {
        public SqliteColumnWriter(CommonSqlWriter sqlWriter)
        {
            this.literalWriter = new SqliteLiteralWriter();
            this.aggregationWriter = new AggregationWriter(sqlWriter);
            this.arithmeticWriter = new ArithmeticWriter(sqlWriter);
            this.functionWriter = new SqliteFunctionWriter(sqlWriter);
            this.castWriter = new SqliteCastWriter(sqlWriter);
            this.miscellaneousWriter = new SqliteMiscellaneousColumnWriter(sqlWriter);
        }
    }
}
