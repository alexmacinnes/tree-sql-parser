using System.Linq;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Writers.Common.Oracle
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
