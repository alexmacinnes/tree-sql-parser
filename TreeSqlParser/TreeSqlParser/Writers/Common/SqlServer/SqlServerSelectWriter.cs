using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Writers.Common.SqlServer
{
    internal class SqlServerSelectWriter : SelectWriter
    {
        public SqlServerSelectWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string TopSql(SelectTop top)
        {
            if (top == null)
                return string.Empty;

            var countSql = sqlWriter.ColumnSql(top.Top);
            var sb = new StringBuilder($"TOP ({countSql}) ");

            if (top.Percent)
                sb.Append("PERCENT ");

            if (top.WithTies)
                sb.Append("WITH TIES ");

            return sb.ToString();
        }
    }
}
