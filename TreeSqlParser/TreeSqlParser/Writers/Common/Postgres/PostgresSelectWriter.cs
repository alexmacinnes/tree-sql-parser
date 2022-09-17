using System.Text;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Writers.Common.Postgres
{
    public class PostgresSelectWriter : SelectWriter
    {
        public PostgresSelectWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string FetchSql(SelectStatement s)
        {
            if (s.OrderBy == null)
                return null;

            if (s.OrderBy.Offset == null && s.OrderBy.Fetch == null)
                return null;

            var sb = new StringBuilder();

            if (s.OrderBy.Fetch != null)
            {
                sb.Append("LIMIT ");
                sb.Append(sqlWriter.GenerateSql(s.OrderBy.Fetch)); 
            }

            if (s.OrderBy.Offset != null)
            {
                if (s.OrderBy.Fetch != null)
                    sb.Append(" ");

                sb.Append("OFFSET ");
                sb.Append(sqlWriter.GenerateSql(s.OrderBy.Offset));
            }
            
            return sb.ToString();
        }


    }
}
