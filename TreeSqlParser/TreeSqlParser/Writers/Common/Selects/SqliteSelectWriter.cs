using System;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Writers.Common.Columns;
using TreeSqlParser.Writers.Common.Identifiers;

namespace TreeSqlParser.Writers.Common.Selects
{
    public class SqliteSelectWriter : SelectWriter
    {
        public SqliteSelectWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string FetchSql(SelectStatement s)
        {
            if (s.OrderBy == null)
                return null;

            if (s.OrderBy.Fetch == null && s.OrderBy.Offset == null)
                return null;

            var sb = new StringBuilder();
            sb.Append("LIMIT ");

            if (s.OrderBy.Fetch == null)
                sb.Append(int.MaxValue);
            else 
                sb.Append(sqlWriter.GenerateSql(s.OrderBy.Fetch));
            
            if (s.OrderBy.Offset != null)
                sb.Append($" OFFSET {sqlWriter.GenerateSql(s.OrderBy.Offset)}");

            return sb.Length > 0 ? sb.ToString() : null;
        }

    }
}
