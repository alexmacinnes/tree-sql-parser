using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Writers.Common.Identifiers;

namespace TreeSqlParser.Writers.Common.Selects
{
    public class OracleSelectWriter : SelectWriter
    {
        public OracleSelectWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string FromSql(Select s) => s.From?.Any() == true ? base.FromSql(s) : "FROM dual";

        protected override string SetModifierSql(SetModifier s) =>
            s == SetModifier.Except ? "MINUS" : base.SetModifierSql(s);
    }
}
