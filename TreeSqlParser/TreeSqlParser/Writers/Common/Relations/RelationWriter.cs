using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Relations;
using TreeSqlParser.Writers.Common.Identifiers;

namespace TreeSqlParser.Writers.Common.Relations
{
    public class RelationWriter : IRelationWriter
    {
        private readonly CommonSqlWriter sqlWriter;
        private readonly bool supportsFullJoin;
        private readonly bool supportsRightJoin;

        public RelationWriter(CommonSqlWriter sqlWriter, bool supportsRightJoin = true, bool supportsFullJoin = true)
        {
            this.sqlWriter = sqlWriter;
            this.supportsFullJoin = supportsFullJoin;
            this.supportsRightJoin = supportsRightJoin;
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
            (s.Alias == null ? string.Empty : $" AS {sqlWriter.IdentifierSql(s.Alias)}");

        private string BracketedSql(BracketedRelation b) =>
            $"({RelationSql(b.InnerRelation)})";

        private string JoinChainSql(JoinChain c)
        {
            if (!supportsRightJoin)
                c = RearrangeRightJoins(c);

            var sb = new StringBuilder(RelationSql(c.LeftRelation));
            foreach (var j in c.Joins)
            {
                sb.Append(" ");
                sb.Append(JoinSql(j));
            }

            return sb.ToString();
        }

        private JoinChain RearrangeRightJoins(JoinChain c)
        {
            bool hasRightJoin = c.Joins?.Any(x => x.JoinType == JoinType.RightJoin) == true;
            if (!hasRightJoin)
                return c;

            var modified = (JoinChain)c.Clone();
            while (true)
            {
                int rightIndex = modified.Joins.FindIndex(x => x.JoinType == JoinType.RightJoin);
                if (rightIndex == -1)
                    break;

                if (rightIndex == 0)
                {
                    // swap the order of the terms
                    var temp = modified.LeftRelation;
                    modified.LeftRelation = modified.Joins[0].RightRelation;
                    modified.Joins[0].RightRelation = temp;

                    // and switch the join direction
                    modified.Joins[0].JoinType = JoinType.LeftJoin;
                    
                    continue;           //skip rest of loop
                }

                // everything after the right joined item
                var followingJoins = modified.Joins.Skip(rightIndex + 1).ToList();

                // everything before the right joined item
                var precedingJoinChain = new BracketedRelation
                {
                    InnerRelation = new JoinChain
                    {
                        LeftRelation = modified.LeftRelation,
                        Joins = modified.Joins.Take(rightIndex).ToList()
                    }
                };

                // rearrange, move right joined relation to head
                var newJoinChain = new JoinChain
                {
                    LeftRelation = modified.Joins[rightIndex].RightRelation,
                    Joins = new List<Join>
                    {
                        new Join
                        {
                            JoinType = JoinType.LeftJoin,
                            RightRelation = precedingJoinChain,
                            Condition = modified.Joins[rightIndex].Condition,
                        }
                    }
                };

                newJoinChain.Joins.AddRange(followingJoins);
                modified = newJoinChain;
            }

            return modified;
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
                case JoinType.FullJoin: 
                    return supportsFullJoin 
                        ? "FULL JOIN" 
                        : throw new NotSupportedException($"Full Join is not supported on {sqlWriter.Name}");
                case JoinType.CrossJoin: return "CROSS JOIN";
                default: throw new InvalidOperationException("Join Type not supported: " + j);
            }
        }

        private string TableSql(Table t)
        {
            var delimited = t.Name.Select(sqlWriter.IdentifierSql);
            string result = string.Join(".", delimited);

            if (t.Alias?.Name != null)
                result += " AS " + sqlWriter.IdentifierSql(t.Alias);

            return result;
        }
    }
}
