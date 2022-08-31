using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Writers.Common.Columns.Components;

namespace TreeSqlParser.Writers.Common.Columns.MySql
{
    public class MySqlCastWriter : CastWriter
    {
        public MySqlCastWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Int() => "signed";

        protected override string NVarChar() => "nchar";

        protected override string Real() => "decimal";

        protected override string Timestamp() => "datetime";

        protected override string VarChar() => "char";
    }
}
