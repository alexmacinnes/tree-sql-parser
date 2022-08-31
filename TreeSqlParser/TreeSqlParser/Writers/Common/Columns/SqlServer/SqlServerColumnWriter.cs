using TreeSqlParser.Writers.Common.Columns.Components;
using TreeSqlParser.Writers.Common.Identifiers;

namespace TreeSqlParser.Writers.Common.Columns.SqlServer
{
    public class SqlServerColumnWriter : ColumnWriter
    {
        public SqlServerColumnWriter(CommonSqlWriter sqlWriter)
        {
            this.literalWriter = new SqlServerLiteralWriter();
            this.aggregationWriter = new AggregationWriter(sqlWriter);
            this.arithmeticWriter = new ArithmeticWriter(sqlWriter);
            this.functionWriter = new SqlServerFunctionWriter(sqlWriter);
            this.castWriter = new SqlServerCastWriter(sqlWriter);
            this.miscellaneousWriter = new MiscellaneousColumnWriter(sqlWriter, "IIF");
        }
    }
}
