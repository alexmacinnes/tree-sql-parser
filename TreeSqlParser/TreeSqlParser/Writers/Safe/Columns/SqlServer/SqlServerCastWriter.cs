using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Writers.Safe.Columns.Components;

namespace TreeSqlParser.Writers.Safe.Columns.SqlServer
{
    public class SqlServerCastWriter : CastWriter
    {
        public SqlServerCastWriter(IColumnWriter columnWriter) : base(columnWriter)
        {
        }

        protected override string Int() => "int";

        protected override string NVarChar() => "nvarchar";

        protected override string Real() => "real";

        protected override string Timestamp() => "timestamp";

        protected override string VarChar() => "varchar";
    }
}
