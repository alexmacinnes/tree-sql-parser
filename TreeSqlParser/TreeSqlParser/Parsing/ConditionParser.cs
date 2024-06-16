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
    public class ConditionParser
    {
        private static readonly IReadOnlyDictionary<string, ColumnComparison> columnComparisonMap =
            EnumUtilities.ToDictionary<ColumnComparison, EnumParseTextAttribute>();

        //internal static Condition ParseCondition(SqlElement parent, List<TSQLToken> tokens)
        //{
        //    int nextIndex = 0;
        //    return ParseCondition(parent, tokens, ref nextIndex);
        //}

        public SelectParser SelectParser { get; set; }

        public virtual Condition ParseCondition(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            var condition = ParseNextCondition(parent, parseContext);

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
                operation.RightCondition = ParseNextCondition(operation, parseContext);
                conditionChain.OtherConditions.Add(operation);
            }

            return condition;
        }

        public virtual Condition ParseNextCondition(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            var token = tokenList.Peek();

            if (token.IsKeyword(TSQLKeywords.EXISTS))
                return ParseExistsCondition(parent, parseContext);

            if (token.IsKeyword(TSQLKeywords.NOT))
                return ParseNotCondition(parent, parseContext);

            if (!token.IsCharacter(TSQLCharacters.OpenParentheses))
                return ParseColumnComparison(parent, parseContext);

            // the condition starts with '('
            // this can either be a
            //   A) bracketed condition e.g. (x < 100)
            //   B) column condition with a bracketed column on the left hand side e.g. (x + y) < 100

            if (IsColumnConditionNext(parseContext))
                return ParseColumnComparison(parent, parseContext);

            return ParseBracketedCondition(parent, parseContext);
        }

        public virtual bool IsColumnConditionNext(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            ParseUtilities.AssertIsChar(tokenList.Peek(), TSQLCharacters.OpenParentheses, parseContext);

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

        public virtual Condition ParseBracketedCondition(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses, parseContext);

            var innerTokens = tokenList.TakeBracketedTokens();
            var subContext = parseContext.Subcontext(innerTokens);

            var result = new BracketCondition { Parent = parent };
            result.InnerCondition = ParseCondition(result, subContext);

            return result;
        }

        public virtual Condition ParseExistsCondition(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            ParseUtilities.AssertIsKeyword(tokenList.Take(), parseContext, TSQLKeywords.EXISTS);
            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses, parseContext);

            var innerTokens = tokenList.TakeBracketedTokens();
            var subContext = parseContext.Subcontext(innerTokens);

            var result = new ExistsCondition { Parent = parent };
            result.Subselect = SelectParser.ParseSelectStatement(result, subContext);

            return result;
        }

        public virtual Condition ParseNotCondition(SqlElement parent, ParseContext parseContext)
        {
            ParseUtilities.AssertIsKeyword(parseContext.TokenList.Take(), parseContext, TSQLKeywords.NOT);

            var result = new NotCondition { Parent = parent };
            result.InnerCondition = ParseCondition(result, parseContext);

            return result;
        }

        public virtual Condition ParseColumnComparison(SqlElement parent, ParseContext parseContext)
        {
            var leftColumn = SelectParser.ColumnParser.ParseNextColumn(null, parseContext);
            var tokenList = parseContext.TokenList;

            if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.IN))
            {
                return ParseInCondition(parent, leftColumn, parseContext);
            }
            else if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.BETWEEN))
            {
                return ParseBetweenCondition(parent, leftColumn, parseContext);
            }
            else if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.IS))
            {
                return ParseIsCondition(parent, leftColumn, parseContext);
            }
            else
            {
                return ParseTwoColumnComparison(parent, leftColumn, parseContext);
            }
        }

        public virtual Condition ParseTwoColumnComparison(SqlElement parent, Column leftColumn, ParseContext parseContext)
        {
            var comparison = ParseColumnComparison(parseContext);
            var setConditionType = TryParseSetConditionType(parseContext);

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
                result.SubselectColumn = SelectParser.ColumnParser.ParseNextColumn(result, parseContext);

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
                result.RightColumn = SelectParser.ColumnParser.ParseNextColumn(result, parseContext);

                return result;
            }
        }

        public virtual SetConditionType? TryParseSetConditionType(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.SOME) || tokenList.TryTakeKeywords(parseContext, TSQLKeywords.ANY))
                return SetConditionType.Some;           // ANY is synonym for SOME

            if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.ALL))
                return SetConditionType.All;

            return null;
        }

        public virtual ColumnComparison ParseColumnComparison(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            if (columnComparisonMap.TryGetValue(tokenList.Peek().Text, out ColumnComparison result))
            {
                tokenList.Advance();
                return result;
            }

            throw new InvalidOperationException($"Expected column comparison, found {tokenList.Peek().Text}");
        }

        public virtual Condition ParseIsCondition(SqlElement parent, Column leftColumn, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            bool not = tokenList.TryTakeKeywords(parseContext, TSQLKeywords.NOT);

            ParseUtilities.AssertIsKeyword(tokenList.Take(), parseContext, TSQLKeywords.NULL);

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

        public virtual Condition ParseBetweenCondition(SqlElement parent, Column leftColumn, ParseContext parseContext)
        {
            var result = new BetweenCondition { Parent = parent, LeftColumn = leftColumn };
            leftColumn.Parent = result;
            
            result.LowerBound = SelectParser.ColumnParser.ParseNextColumn(result, parseContext);

            ParseUtilities.AssertIsKeyword(parseContext.TokenList.Take(), parseContext, TSQLKeywords.AND);
            
            result.UpperBound = SelectParser.ColumnParser.ParseNextColumn(result, parseContext);

            return result;
        }

        public virtual Condition ParseInCondition(SqlElement parent, Column leftColumn, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses, parseContext);

            var innerTokens = tokenList.TakeBracketedTokens();
            var subContext = parseContext.Subcontext(innerTokens);

            if (innerTokens.Peek().IsKeyword(TSQLKeywords.SELECT))
            {
                var result = new InSubselectCondition { Parent = parent, LeftColumn = leftColumn };
                leftColumn.Parent = result;
                result.Subselect = SelectParser.ParseSelectStatement(result, subContext);
                return result;
            } 
            else
            {
                var result = new InListCondition { Parent = parent, LeftColumn = leftColumn };
                leftColumn.Parent = result;
                result.RightColumns = SelectParser.ColumnParser.ParseColumns(result, subContext);
                return result;
            }
        }
    }
}
