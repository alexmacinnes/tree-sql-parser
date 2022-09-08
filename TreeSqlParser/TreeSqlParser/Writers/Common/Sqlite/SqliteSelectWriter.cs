using System.Text;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Writers.Common.Sqlite
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
