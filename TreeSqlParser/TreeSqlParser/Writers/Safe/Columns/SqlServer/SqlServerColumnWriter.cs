using TreeSqlParser.Writers.Safe.Columns.Components;
using TreeSqlParser.Writers.Safe.Identifiers;

namespace TreeSqlParser.Writers.Safe.Columns.SqlServer
{
    public class SqlServerColumnWriter : ColumnWriter
    {
        public SqlServerColumnWriter(ISqlWriter sqlWriter, IIdentifierWriter identifierWriter)
        {
            this.literalWriter = new SqlServerLiteralWriter();
            this.aggregationWriter = new AggregationWriter(this);
            this.arithmeticWriter = new SqlServerArithmeticWriter(this);
            this.functionWriter = new SqlServerFunctionWriter(this);
            this.castWriter = new SqlServerCastWriter(this);
            this.miscellaneousWriter = new MiscellaneousColumnWriter(sqlWriter, identifierWriter, "IIF");
        }
    }
}
