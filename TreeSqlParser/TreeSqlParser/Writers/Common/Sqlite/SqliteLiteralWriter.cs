using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Sqlite
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

        protected override string BoolSql(BoolColumn x) =>
            x.Value ? "1" : "0";
    }
}
