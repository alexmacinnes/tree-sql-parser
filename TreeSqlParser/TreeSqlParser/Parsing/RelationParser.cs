using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Relations;
using TSQL;
using TSQL.Tokens;
using Table = TreeSqlParser.Model.Relations.Table;

namespace TreeSqlParser.Parsing
{
    internal class RelationParser
    {
        internal static List<Relation> ParseRelations(SqlElement parent, TokenList tokenList)
        {
            if (!tokenList.TryTakeKeywords(TSQLKeywords.FROM))
                return null;

            var result = new List<Relation>();

            while (tokenList.HasMore)
            {
                var relation = ParseNextRelation(parent, tokenList);
                result.Add(relation);

                if (!tokenList.TryTakeCharacter(TSQLCharacters.Comma))
                    break;
            }

            return result;
        }

        private static Relation ParseNextRelation(SqlElement parent, TokenList tokenList)
        {
            Relation relation;
            if (tokenList.Peek()?.IsCharacter(TSQLCharacters.OpenParentheses) == true)
            {
                if (tokenList.Peek(1)?.IsKeyword(TSQLKeywords.SELECT) == true)
                    relation = ParseSubselect(parent, tokenList);
                else
                    relation = ParseBracketedRelation(parent, tokenList);
            }
            else
            {
                relation = ParseTable(parent, tokenList);
            }

            while (true)
            {
                var joinType = TryParseJoinType(tokenList);
                if (!joinType.HasValue)
                    break;

                if (!(relation is JoinChain joinChain))
                {
                    joinChain = new JoinChain
                    {
                        Parent = parent,
                        LeftRelation = relation,
                        Joins = new List<Join>()
                    };
                    relation.Parent = joinChain;
                    relation = joinChain;
                }
                var join = new Join
                {
                    Parent = joinChain,
                    JoinType = joinType.Value
                };
                join.RightRelation = ParseNextRelation(join, tokenList);
                if (tokenList.TryTakeKeywords(TSQLKeywords.ON))
                    join.Condition = ConditionParser.ParseCondition(join, tokenList);

                joinChain.Joins.Add(join);
            }

            return relation;
        }

        private static JoinType? TryParseJoinType(TokenList tokenList)
        {
            void consumeOuterJoin()
            {
                tokenList.Advance();
                tokenList.TryTakeKeywords(TSQLKeywords.OUTER);
                ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.JOIN);
            }

            void consumeApply()
            {
                if (tokenList.Take()?.Text?.ToUpperInvariant() != "APPLY")
                    throw new InvalidOperationException("Expected keyword JOIN or APPLY");
            }

            if (!tokenList.HasMore) return null;

            var nextToken = tokenList.Peek();
            if (!(nextToken is TSQLKeyword)) return null;

            var keyword = ((TSQLKeyword)nextToken).Keyword;

            if (keyword == TSQLKeywords.JOIN)
            {
                tokenList.Advance();
                return JoinType.InnerJoin;
            }
            else if (keyword == TSQLKeywords.INNER)
            {
                tokenList.Advance();
                ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.JOIN);
                return JoinType.InnerJoin;
            }
            else if (keyword == TSQLKeywords.LEFT)
            {
                consumeOuterJoin();
                return JoinType.LeftJoin;
            }
            else if (keyword == TSQLKeywords.RIGHT)
            {
                consumeOuterJoin();
                return JoinType.RightJoin;
            }
            else if (keyword == TSQLKeywords.FULL)
            {
                consumeOuterJoin();
                return JoinType.FullJoin;
            }
            else if (keyword == TSQLKeywords.CROSS)
            {
                tokenList.Advance();
                if (tokenList.TryTakeKeywords(TSQLKeywords.JOIN))
                    return JoinType.CrossJoin;

                consumeApply();
                return JoinType.CrossApply;
            }
            else if (keyword == TSQLKeywords.OUTER)
            {
                tokenList.Advance();
                consumeApply();
                return JoinType.OuterApply;
            }

            // keyword is not a join
            return null;
        }

        private static Table ParseTable(SqlElement parent, TokenList tokenList)
        {
            var table = new Table
            {
                Parent = parent,
                Name = SelectParser.ParseMultiPartIndentifier(tokenList).Select(x => new SqlIdentifier(x)).ToArray()
            };

            table.Alias = ParseRelationAlias(tokenList);

            return table;
        }

        private static SqlIdentifier ParseRelationAlias(TokenList tokenList)
        {
            string alias = ParseUtilities.TryTakeAlias(tokenList);
            if (alias != null)
                return new SqlIdentifier(alias);

            return null;
        }

        private static Relation ParseBracketedRelation(SqlElement parent, TokenList tokenList)
        {
            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses);

            var innerTokens = tokenList.TakeBracketedTokens();

            var result = new BracketedRelation { Parent = parent };
            result.InnerRelation = ParseNextRelation(result, innerTokens);

            return result;
        }

        private static Relation ParseSubselect(SqlElement parent, TokenList tokenList)
        {
            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses);

            var innerTokens = tokenList.TakeBracketedTokens();

            var result = new SubselectRelation { Parent = parent };
            result.InnerSelect = SelectParser.ParseSelectStatement(result, innerTokens);
            result.Alias = ParseRelationAlias(tokenList);

            return result;
        }
    }
}
