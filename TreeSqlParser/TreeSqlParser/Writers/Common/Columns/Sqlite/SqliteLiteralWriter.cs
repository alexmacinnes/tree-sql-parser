using TreeSqlParser.Model.Columns;
using TreeSqlParser.Writers.Common.Columns.Components;

namespace TreeSqlParser.Writers.Common.Columns.Sqlite
{
    public class SqliteLiteralWriter : LiteralWriter
    {
        protected override string DateSql(DateColumn x) =>
            $"DATE('{x.Value:yyyy-MM-dd}')";

        protected override string DateTimeSql(DateTimeColumn x) =>
            $"DATETIME('{x.Value:yyyy-MM-dd HH:mm:ss}')";

        protected override string DecimalSql(DecimalColumn x) =>
            x.Value.ToString();

        protected override string IntegerSql(IntegerColumn x) =>
            x.Value.ToString();

        protected override string StringSql(StringColumn x) =>
            $"'{x.Value.Replace("'", "''")}'";
    }
}
