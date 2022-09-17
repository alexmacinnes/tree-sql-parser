using System;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Postgres
{
    public class PostgresCastWriter : CastWriter
    {
        public PostgresCastWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Int() => "integer";

        protected override string NVarChar() => "varchar";

        protected override string Real() => "decimal";

        protected override string Timestamp() => "timestamp";

        protected override string VarChar() => "varchar";

        public override string CastSql(CastColumn c)
        {
            if (c.TryCast)
                throw new InvalidOperationException("TRY_CAST not supported");

            return $"({ColumnSql(c.Column)})::{MapDataType(c.DataType)}";
        }
    }
}
