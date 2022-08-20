using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Relations;
using TreeSqlParser.Writers.Safe.Identifiers;

namespace TreeSqlParser.Writers.Safe.Relations
{
    public class RelationWriter : IRelationWriter
    {
        private readonly ISqlWriter sqlWriter;
        private readonly IIdentifierWriter identifierWriter;
        private readonly bool supportsFullJoin;

        public RelationWriter(ISqlWriter sqlWriter, IIdentifierWriter identifierWriter, bool supportsFullJoin)
        {
            this.sqlWriter = sqlWriter;
            this.identifierWriter = identifierWriter;
            this.supportsFullJoin = supportsFullJoin;
        }

        public string RelationSql(Relation r) 
        {
            if (r is Table t) return TableSql(t);
            if (r is JoinChain j) return JoinChainSql(j);
            if (r is BracketedRelation b) return BracketedSql(b);
            if (r is SubselectRelation s) return SubselectSql(s);

            throw new InvalidOperationException("Unknown Relation type");
        }

        private string SubselectSql(SubselectRelation s) =>
            $"({sqlWriter.GenerateSql(s.InnerSelect)})" +
            (s.Alias == null ? string.Empty : $" AS {identifierWriter.Delimit(s.Alias)}");

        private string BracketedSql(BracketedRelation b) =>
            $"({RelationSql(b.InnerRelation)})";

        private string JoinChainSql(JoinChain c)
        {
            var sb = new StringBuilder(RelationSql(c.LeftRelation));
            foreach (var j in c.Joins)
            {
                sb.Append(" ");
                sb.Append(JoinSql(j));
            }

            return sb.ToString();
        }

        private string JoinSql(Join j)
        {
            if (j.JoinType == JoinType.FullJoin && !supportsFullJoin)
                throw new InvalidOperationException("Full Join not supported");

            string result = $"{JoinTypeSql(j.JoinType)} {RelationSql(j.RightRelation)}";
            if (j.JoinType != JoinType.CrossJoin)
                result += " ON " + sqlWriter.GenerateSql(j.Condition);

            return result;
        }

        private string JoinTypeSql(JoinType j)
        {
            switch (j)
            {
                case JoinType.InnerJoin: return "INNER JOIN";
                case JoinType.LeftJoin: return "LEFT JOIN";
                case JoinType.RightJoin: return "RIGHT JOIN";
                case JoinType.FullJoin: return "FULL JOIN";
                case JoinType.CrossJoin: return "CROSS JOIN";
                default: throw new InvalidOperationException("Join Type not supported: " + j);
            }
        }

        private string TableSql(Table t)
        {
            var delimited = t.Name.Select(identifierWriter.Delimit);
            string result = string.Join(".", delimited);

            if (t.Alias?.Name != null)
                result += " AS " + identifierWriter.Delimit(t.Alias);

            return result;
        }
    }
}
