using System;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Writers.Safe.Columns;
using TreeSqlParser.Writers.Safe.Identifiers;

namespace TreeSqlParser.Writers.Safe.Selects
{
    public class SelectWriter : ISelectWriter
    {
        private readonly SafeSqlWriter sqlWriter;

        public SelectWriter(SafeSqlWriter sqlWriter) 
        {
            this.sqlWriter = sqlWriter;
        }

        public virtual string SelectSql(Select s)
        {
            var sb = new StringBuilder();
            
            string setMod = SetModifierSql(s.SetModifier);
            if (setMod != null)
                sb.Append(setMod).Append(" ");

            sb.Append("SELECT ");
            sb.Append(string.Join(", ", s.Columns.Select(sqlWriter.GenerateSql)));

            string from = FromSql(s);
            if (from != null)
                sb.Append(" ").Append(from);

            string where = WhereSql(s);
            if (where != null)
                sb.Append(" ").Append(where);

            string groupBy = GroupBySql(s);
            if (groupBy != null)
                sb.Append(" ").Append(groupBy);

            string having = HavingSql(s);
            if (having != null)
                sb.Append(" ").Append(having);

            return sb.ToString();
        }

        protected virtual string HavingSql(Select s)
        {
            if (s.HavingCondition == null)
                return null;

            return "HAVING " + sqlWriter.GenerateSql(s.HavingCondition);
        }

        protected virtual string GroupBySql(Select s)
        {
            if (s.GroupBy?.Any() != true)
                return null;

            if (s.GroupBy.Count > 1)
                throw new InvalidOperationException("Multiple grouping sets not supported");

            var gs = s.GroupBy[0];

            if (gs.SetType != GroupingSetType.Columns)
                throw new InvalidOperationException("Grouping set type not supported");

            string result = "GROUP BY " + string.Join(", ", gs.Columns.Select(sqlWriter.GenerateSql));
            return result;
        }

        protected virtual string WhereSql(Select s)
        {
            if (s.WhereCondition == null)
                return null;

            return "WHERE " + sqlWriter.GenerateSql(s.WhereCondition);
        }

        protected virtual string FromSql(Select s)
        {
            if (s.From?.Any() != true)
                return null;

            string relations = string.Join(", ", s.From.Select(sqlWriter.GenerateSql));
            return "FROM " + relations;
        }

        protected virtual string SetModifierSql(SetModifier s)
        {
            switch (s) 
            {
                case SetModifier.None: return null;
                case SetModifier.Union: return "UNION";
                case SetModifier.UnionAll: return "UNION ALL";
                case SetModifier.Intersect: return "INTERSECT";
                case SetModifier.Except: return "EXCEPT";
                default: throw new InvalidOperationException("Unkown Set Modifier");
            }
        }

        private string Delimit(string name) => sqlWriter.IdentifierSql(new SqlIdentifier { Name = name });


        public string SelectStatementSql(SelectStatement s)
        {
            var sb = new StringBuilder();
            if (s.WithSelects?.Any() == true)
            {
                sb.Append("WITH ");
                sb.Append(string.Join(", ", s.WithSelects.Select(WithSelectSql)));
                sb.Append(" ");
            }

            string selectsSql = string.Join(" ", s.Selects.Select(SelectSql));
            sb.Append(selectsSql);

            string orderBy = OrderBySql(s);
            if (orderBy != null)
                sb.Append(" ").Append(orderBy);

            string fetch = FetchSql(s);
            if (fetch != null)
                sb.Append(" ").Append(fetch);

            return sb.ToString();
        }

        protected virtual string FetchSql(SelectStatement s)
        {
            if (s.OrderBy == null)
                return null;

            var sb = new StringBuilder();
            if (s.OrderBy.Offset != null)
            {
                sb.Append($"OFFSET {sqlWriter.GenerateSql(s.OrderBy.Offset)} ROWS");
                if (s.OrderBy.Fetch != null)
                    sb.Append(" ");
            }
            if (s.OrderBy.Fetch != null)
                sb.Append($"FETCH NEXT {sqlWriter.GenerateSql(s.OrderBy.Fetch)} ROWS ONLY");

            return sb.Length > 0 ? sb.ToString() : null;
        }

        protected virtual string OrderBySql(SelectStatement s)
        {
            if (s.OrderBy?.Columns?.Any() != true)
                return null;

            var cols = s.OrderBy.Columns.Select(OrderByColumnSql);
            return "ORDER BY " + string.Join(", ", cols);
        }

        protected virtual string OrderByColumnSql(OrderByColumn x)
        {
            if (x.Collate != null)
                throw new InvalidOperationException("ORDER BY collation not supported");

            string sort;
            switch (x.SortOrder)
            {
                case ColumnSortOrder.Asc: sort = string.Empty; break;
                case ColumnSortOrder.Desc: sort = " DESC"; break;
                default: throw new InvalidOperationException("Unknown column sort order");
            }

            return sqlWriter.GenerateSql(x.Column) + sort;
        }

        protected virtual string WithSelectSql(AliasSelect s) => 
            $"{Delimit(s.Alias.Name)} AS ({string.Join(" ", s.Selects.Select(SelectSql))})";
    }
}
