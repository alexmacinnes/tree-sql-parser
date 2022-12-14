using System;
using System.Collections.Generic;
using System.Linq;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Parsing.Enums;
using TSQL;
using TSQL.Tokens;

namespace TreeSqlParser.Parsing
{
    internal class ConditionParser
    {
        private static readonly IReadOnlyDictionary<string, ColumnComparison> columnComparisonMap =
            EnumUtilities.ToDictionary<ColumnComparison, EnumParseTextAttribute>();

        //internal static Condition ParseCondition(SqlElement parent, List<TSQLToken> tokens)
        //{
        //    int nextIndex = 0;
        //    return ParseCondition(parent, tokens, ref nextIndex);
        //}

        internal static Condition ParseCondition(SqlElement parent, TokenList tokenList)
        {
            var condition = ParseNextCondition(parent, tokenList);

            while (tokenList.HasMore)
            {
                var nextToken = tokenList.Peek();
                LogicCondition logicCondition;
                if (nextToken.IsKeyword(TSQLKeywords.AND))
                    logicCondition = LogicCondition.And;
                else if (nextToken.IsKeyword(TSQLKeywords.OR))
                    logicCondition = LogicCondition.Or;
                else
                    break;
                tokenList.Advance();

                if (!(condition is ConditionChain conditionChain))
                {
                    conditionChain = new ConditionChain
                    {
                        Parent = parent,
                        LeftCondition = condition,
                        OtherConditions = new List<LogicOperation>()
                    };
                    condition.Parent = conditionChain;
                    condition = conditionChain;
                }

                var operation = new LogicOperation
                {
                    Parent = conditionChain,
                    LogicCondition = logicCondition,
                };
                operation.RightCondition = ParseNextCondition(operation, tokenList);
                conditionChain.OtherConditions.Add(operation);
            }

            return condition;
        }

        internal static Condition ParseNextCondition(SqlElement parent, TokenList tokenList)
        {
            var token = tokenList.Peek();

            if (token.IsKeyword(TSQLKeywords.EXISTS))
                return ParseExistsCondition(parent, tokenList);

            if (token.IsKeyword(TSQLKeywords.NOT))
                return ParseNotCondition(parent, tokenList);

            if (!token.IsCharacter(TSQLCharacters.OpenParentheses))
                return ParseColumnComparison(parent, tokenList);

            // the condition starts with '('
            // this can either be a
            //   A) bracketed condition e.g. (x < 100)
            //   B) column condition with a bracketed column on the left hand side e.g. (x + y) < 100

            if (IsColumnConditionNext(tokenList))
                return ParseColumnComparison(parent, tokenList);

            return ParseBracketedCondition(parent, tokenList);
        }

        private static bool IsColumnConditionNext(TokenList tokenList)
        {
            ParseUtilities.AssertIsChar(tokenList.Peek(), TSQLCharacters.OpenParentheses);

            var clonedTokenList = tokenList.CloneFromCurrentPosition();

            int level = 0;
            while(clonedTokenList.HasMore)
            {
                var nextToken = clonedTokenList.Take();
                if (nextToken.IsCharacter(TSQLCharacters.OpenParentheses))
                    level++;
                else if (nextToken.IsCharacter(TSQLCharacters.CloseParentheses))
                    level--;

                if (level == 0)
                    break;
            }

            // this must be a bracketed condition at the end of the token list
            if (!clonedTokenList.HasMore)
                return false;

            var tokenAfterBrackets = clonedTokenList.Take();
            
            var keyword = tokenAfterBrackets as TSQLKeyword;
            if (keyword != null)
            {
                //if true the condition is something like '(x + y) BETWEEN 1 AND 10'
                var keywords = new[] { TSQLKeywords.BETWEEN, TSQLKeywords.IN, TSQLKeywords.IS };
                return keywords.Contains(keyword.Keyword);          
            }

            //if true the condition is something like '(x + y) > 10'
            return columnComparisonMap.ContainsKey(tokenAfterBrackets.Text);

        }

        private static Condition ParseBracketedCondition(SqlElement parent, TokenList tokenList)
        {
            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses);

            var innerTokens = tokenList.TakeBracketedTokens();

            var result = new BracketCondition { Parent = parent };
            result.InnerCondition = ParseCondition(result, innerTokens);

            return result;
        }

        private static Condition ParseExistsCondition(SqlElement parent, TokenList tokenList)
        {
            ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.EXISTS);
            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses);

            var innerTokens = tokenList.TakeBracketedTokens();

            var result = new ExistsCondition { Parent = parent };
            result.Subselect = SelectParser.ParseSelectStatement(result, innerTokens);

            return result;
        }

        private static Condition ParseNotCondition(SqlElement parent, TokenList tokenList)
        {
            ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.NOT);

            var result = new NotCondition { Parent = parent };
            result.InnerCondition = ParseCondition(result, tokenList);

            return result;
        }

        private static Condition ParseColumnComparison(SqlElement parent, TokenList tokenList)
        {
            var leftColumn = ColumnParser.ParseNextColumn(null, tokenList);

            if (tokenList.TryTakeKeywords(TSQLKeywords.IN))
            {
                return ParseInCondition(parent, leftColumn, tokenList);
            }
            else if (tokenList.TryTakeKeywords(TSQLKeywords.BETWEEN))
            {
                return ParseBetweenCondition(parent, leftColumn, tokenList);
            }
            else if (tokenList.TryTakeKeywords(TSQLKeywords.IS))
            {
                return ParseIsCondition(parent, leftColumn, tokenList);
            }
            else
            {
                return ParseTwoColumnComparison(parent, leftColumn, tokenList);
            }
        }

        private static Condition ParseTwoColumnComparison(SqlElement parent, Column leftColumn, TokenList tokenList)
        {
            var comparison = ParseColumnComparison(tokenList);
            var setConditionType = TryParseSetConditionType(tokenList);

            if (setConditionType.HasValue)
            {
                var result = new SetCondition
                {
                    Parent = parent,
                    LeftColumn = leftColumn,
                    Comparison = comparison,
                    SetConditionType = setConditionType.Value
                };
                leftColumn.Parent = result;
                result.SubselectColumn = ColumnParser.ParseNextColumn(result, tokenList);

                return result;
            }
            else
            {
                var result = new ComparisonCondition 
                { 
                    Parent = parent, 
                    LeftColumn = leftColumn,
                    Comparison = comparison
                };
                leftColumn.Parent = result;
                result.RightColumn = ColumnParser.ParseNextColumn(result, tokenList);

                return result;
            }
        }

        private static SetConditionType? TryParseSetConditionType(TokenList tokenList)
        {
            if (tokenList.TryTakeKeywords(TSQLKeywords.SOME) || tokenList.TryTakeKeywords(TSQLKeywords.ANY))
                return SetConditionType.Some;           // ANY is synonym for SOME

            if (tokenList.TryTakeKeywords(TSQLKeywords.ALL))
                return SetConditionType.All;

            return null;
        }

        private static ColumnComparison ParseColumnComparison(TokenList tokenList)
        {
            if (columnComparisonMap.TryGetValue(tokenList.Peek().Text, out ColumnComparison result))
            {
                tokenList.Advance();
                return result;
            }

            throw new InvalidOperationException($"Expected column comparison, found {tokenList.Peek().Text}");
        }

        private static Condition ParseIsCondition(SqlElement parent, Column leftColumn, TokenList tokenList)
        {
            bool not = tokenList.TryTakeKeywords(TSQLKeywords.NOT);

            ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.NULL);

            if (not)
            {
                var result = new NotNullCondition { Parent = parent, LeftColumn = leftColumn };
                leftColumn.Parent = result;
                return result;
            } 
            else
            {
                var result = new IsNullCondition { Parent = parent, LeftColumn = leftColumn };
                leftColumn.Parent = result;
                return result;
            }
        }

        private static Condition ParseBetweenCondition(SqlElement parent, Column leftColumn, TokenList tokenList)
        {
            var result = new BetweenCondition { Parent = parent, LeftColumn = leftColumn };
            leftColumn.Parent = result;
            
            result.LowerBound = ColumnParser.ParseNextColumn(result, tokenList);

            ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.AND);
            
            result.UpperBound = ColumnParser.ParseNextColumn(result, tokenList);

            return result;
        }

        private static Condition ParseInCondition(SqlElement parent, Column leftColumn, TokenList tokenList)
        {
            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses);

            var innerTokens = tokenList.TakeBracketedTokens();
            if (innerTokens.Peek().IsKeyword(TSQLKeywords.SELECT))
            {
                var result = new InSubselectCondition { Parent = parent, LeftColumn = leftColumn };
                leftColumn.Parent = result;
                result.Subselect = SelectParser.ParseSelectStatement(result, innerTokens);
                return result;
            } 
            else
            {
                var result = new InListCondition { Parent = parent, LeftColumn = leftColumn };
                leftColumn.Parent = result;
                result.RightColumns = ColumnParser.ParseColumns(result, innerTokens);
                return result;
            }
        }
    }
}
