using TreeSqlParser.Writers.Common.Columns.Components;
using TreeSqlParser.Writers.Common.Identifiers;

namespace TreeSqlParser.Writers.Common.Columns.Sqlite
{
    public class SqliteColumnWriter : ColumnWriter
    {
        public SqliteColumnWriter(CommonSqlWriter sqlWriter)
        {
            this.literalWriter = new SqliteLiteralWriter();
            this.aggregationWriter = new AggregationWriter(sqlWriter);
            this.arithmeticWriter = new ArithmeticWriter(sqlWriter);
            this.functionWriter = null; //TODO
            this.castWriter = new SqliteCastWriter(sqlWriter);
            this.miscellaneousWriter = new SqliteMiscellaneousColumnWriter(sqlWriter);
        }
    }

    /*
     *         protected override string Int() => "int";

        protected override string NVarChar() => "nchar";

        protected override string Real() => "real";

        protected override string Timestamp() => throw new NotSupportedException("Use Us");

        protected override string VarChar() => "nvarchar";
    */
}
