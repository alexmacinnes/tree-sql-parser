﻿using TreeSqlParser.Model.Columns;
using TreeSqlParser.Writers.Safe.Columns.Components;

namespace TreeSqlParser.Writers.Safe.Columns.Oracle
{
    public class OracleLiteralWriter : LiteralWriter
    {
        protected override string DateSql(DateColumn x) =>
            $"DATE '{x.Value:yyyy-MM-dd}'";

        protected override string DateTimeSql(DateTimeColumn x) =>
            $"TIMESTAMP '{x.Value:yyyy-MM-dd HH:mm:ss.FFFFFFF}'";

        protected override string DecimalSql(DecimalColumn x) =>
            x.Value.ToString();

        protected override string IntegerSql(IntegerColumn x) =>
            x.Value.ToString();

        protected override string StringSql(StringColumn x) =>
            $"'{x.Value.Replace("'", "''")}'";
    }
}
