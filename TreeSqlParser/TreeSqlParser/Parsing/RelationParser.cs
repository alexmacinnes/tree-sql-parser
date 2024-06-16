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
    public class RelationParser
    {
        public SelectParser SelectParser { get; set; }

        public virtual List<Relation> ParseRelations(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            if (!tokenList.TryTakeKeywords(parseContext, TSQLKeywords.FROM))
                return null;

            var result = new List<Relation>();

            while (tokenList.HasMore)
            {
                var relation = ParseNextRelation(parent, parseContext);
                result.Add(relation);

                if (!tokenList.TryTakeCharacter(TSQLCharacters.Comma))
                    break;
            }

            return result;
        }

        public virtual Relation ParseNextRelation(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            Relation relation;
            if (tokenList.Peek()?.IsCharacter(TSQLCharacters.OpenParentheses) == true)
            {
                if (tokenList.Peek(1)?.IsKeyword(TSQLKeywords.SELECT) == true)
                    relation = ParseSubselect(parent, parseContext);
                else
                    relation = ParseBracketedRelation(parent, parseContext);
            }
            else
            {
                relation = ParseTable(parent, parseContext);
            }

            while (true)
            {
                var joinType = TryParseJoinType(parseContext);
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
                join.RightRelation = ParseNextRelation(join, parseContext);
                if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.ON))
                    join.Condition = SelectParser.ConditionParser.ParseCondition(join, parseContext);

                joinChain.Joins.Add(join);
            }

            return relation;
        }

        public virtual JoinType? TryParseJoinType(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            void consumeOuterJoin()
            {
                tokenList.Advance();
                tokenList.TryTakeKeywords(parseContext, TSQLKeywords.OUTER);
                ParseUtilities.AssertIsKeyword(tokenList.Take(), parseContext, TSQLKeywords.JOIN);
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
                ParseUtilities.AssertIsKeyword(tokenList.Take(), parseContext, TSQLKeywords.JOIN);
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
                if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.JOIN))
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

        public virtual Table ParseTable(SqlElement parent, ParseContext parseContext)
        {
            var table = new Table
            {
                Parent = parent,
                Name = SelectParser.ParseMultiPartIndentifier(parseContext).Select(x => new SqlIdentifier(x)).ToArray()
            };

            table.Alias = ParseRelationAlias(parseContext);

            return table;
        }

        public virtual SqlIdentifier ParseRelationAlias(ParseContext parseContext)
        {
            string alias = ParseUtilities.TryTakeAlias(parseContext);
            if (alias != null)
                return new SqlIdentifier(alias);

            return null;
        }

        public virtual Relation ParseBracketedRelation(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses, parseContext);

            var innerTokens = tokenList.TakeBracketedTokens();
            var subContext = parseContext.Subcontext(innerTokens);

            var result = new BracketedRelation { Parent = parent };
            result.InnerRelation = ParseNextRelation(result, subContext);

            return result;
        }

        public virtual Relation ParseSubselect(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses, parseContext);

            var innerTokens = tokenList.TakeBracketedTokens();
            var subContext = parseContext.Subcontext(innerTokens);

            var result = new SubselectRelation { Parent = parent };
            result.InnerSelect = SelectParser.ParseSelectStatement(result, subContext);
            result.Alias = ParseRelationAlias(parseContext);

            return result;
        }
    }
}
