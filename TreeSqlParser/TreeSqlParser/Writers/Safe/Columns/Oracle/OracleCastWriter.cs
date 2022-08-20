using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Writers.Safe.Columns.Components;

namespace TreeSqlParser.Writers.Safe.Columns.Oracle
{
    public class OracleCastWriter : CastWriter
    {
        public OracleCastWriter(IColumnWriter columnWriter) : base(columnWriter)
        {
        }

        protected override string Int() => "int";

        protected override string NVarChar() => "nvarchar2(255)";

        protected override string Real() => "number";

        protected override string Timestamp() => "datetime";

        protected override string VarChar() => "varchar2(255)";
    }
}
