using TreeSqlParser.Writers.Common.Columns.Components;
using TreeSqlParser.Writers.Common.Identifiers;

namespace TreeSqlParser.Writers.Common.Columns.MySql
{
    public class MySqlColumnWriter : ColumnWriter
    {
        public MySqlColumnWriter(CommonSqlWriter sqlWriter)
        {
            this.literalWriter = new MySqlLiteralWriter();
            this.aggregationWriter = new AggregationWriter(sqlWriter);
            this.arithmeticWriter = new ArithmeticWriter(sqlWriter);
            this.functionWriter = new MySqlFunctionWriter(sqlWriter);
            this.castWriter = new MySqlCastWriter(sqlWriter);
            this.miscellaneousWriter = new MiscellaneousColumnWriter(sqlWriter, "IF");
        }
    }
}
