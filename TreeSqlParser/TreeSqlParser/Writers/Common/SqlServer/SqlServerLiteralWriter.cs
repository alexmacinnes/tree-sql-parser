﻿using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.SqlServer
{
    public class SqlServerLiteralWriter : LiteralWriter
    {
        protected override string DateSql(DateColumn x) =>
            $"{{d '{x.Value:yyyy-MM-dd}'}}";

        protected override string DateTimeSql(DateTimeColumn x) =>
            $"{{ts '{x.Value:yyyy-MM-dd HH:mm:ss.FFFFFFF}'}}";

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
