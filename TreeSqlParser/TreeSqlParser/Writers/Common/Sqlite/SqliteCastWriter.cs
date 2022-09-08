using System;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Sqlite
{
    public class SqliteCastWriter : CastWriter
    {
        public SqliteCastWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        public override string CastSql(CastColumn c)
        {
            if (c.DataType.Value == "timestamp" && !c.TryCast)
                return $"DATETIME({ColumnSql(c.Column)})";

            return base.CastSql(c);
        }

        protected override string Int() => "int";

        protected override string NVarChar() => "nvarchar";

        protected override string Real() => "real";

        protected override string Timestamp() => throw new NotSupportedException("USE DATETIME(date) instead");

        protected override string VarChar() => "varchar";
    }
}
