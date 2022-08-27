using TreeSqlParser.Writers.Safe.Columns.Components;
using TreeSqlParser.Writers.Safe.Identifiers;

namespace TreeSqlParser.Writers.Safe.Columns.SqlServer
{
    public class SqlServerColumnWriter : ColumnWriter
    {
        public SqlServerColumnWriter(SafeSqlWriter sqlWriter)
        {
            this.literalWriter = new SqlServerLiteralWriter();
            this.aggregationWriter = new AggregationWriter(sqlWriter);
            this.arithmeticWriter = new SqlServerArithmeticWriter(sqlWriter);
            this.functionWriter = new SqlServerFunctionWriter(sqlWriter);
            this.castWriter = new SqlServerCastWriter(sqlWriter);
            this.miscellaneousWriter = new MiscellaneousColumnWriter(sqlWriter, "IIF");
        }
    }
}
