using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Writers.Common.MySql
{
    public class MySqlSelectWriter : SelectWriter
    {
        public MySqlSelectWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string SetModifierSql(SetModifier s) =>
            s == SetModifier.Except 
            ? throw new NotSupportedException($"EXCEPT is not supported on {sqlWriter.Name}") 
            : base.SetModifierSql(s);

        protected override string FetchSql(SelectStatement s)
        {
            if (s.OrderBy == null)
                return null;

            if (s.OrderBy.Offset == null && s.OrderBy.Fetch == null)
                return null;

            var sb = new StringBuilder();
            sb.Append("LIMIT ");
            if (s.OrderBy.Offset != null)
            {
                sb.Append($"{sqlWriter.GenerateSql(s.OrderBy.Offset)}, ");
            }
            if (s.OrderBy.Fetch == null)
                sb.Append(int.MaxValue.ToString());
            else
                sb.Append($"{sqlWriter.GenerateSql(s.OrderBy.Fetch)}");

            return sb.ToString();
        }
    }
}
