using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Analytics;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Parsing.Enums;
using TSQL;
using TSQL.Tokens;

namespace TreeSqlParser.Parsing
{
    public class OverParser
    {
        public SelectParser SelectParser { get; set; }

        private static readonly IReadOnlyDictionary<string, ExtentType> ExtentTypeMap = EnumUtilities.ToDictionary<ExtentType, EnumParseTextAttribute>();

        private const string PARTITION = "PARTITION";
        private const string PRECEDING = "PRECEDING";
        private const string FOLLOWING = "FOLLOWING";
        private const string UNBOUNDED = "UNBOUNDED";
        private const string ROW = "ROW";

        public virtual Over ParseOver(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            if (!tokenList.TryTakeKeywords(parseContext, TSQLKeywords.OVER))
                return null;

            if (!tokenList.TryTakeCharacter(TSQLCharacters.OpenParentheses))
                throw parseContext.ErrorGenerator.ParseException("Expected open parentheses after OVER", tokenList.Peek());

            var innerTokens = tokenList.TakeBracketedTokens(parseContext.ErrorGenerator);
            var subContext = parseContext.Subcontext(innerTokens);

            var result = new Over { Parent = parent };

            if (innerTokens.Peek()?.Text.ToUpper() == PARTITION)
            {
                innerTokens.Advance();
                ParseUtilities.AssertIsKeyword(innerTokens.Take(), subContext, TSQLKeywords.BY);
                result.PartitionBy = SelectParser.ColumnParser.ParseColumns(result, subContext);
            }

            if (innerTokens.TryTakeKeywords(subContext, TSQLKeywords.ORDER, TSQLKeywords.BY))
                result.OrderBy = SelectParser.OrderByParser.ParseOrderByColumns(result, subContext);

            var extentType = TryParseExtentType(subContext);
            if (extentType.HasValue)
            {  
                result.Extent = ParseExtent(result, extentType.Value, subContext);
            }

            return result;
        }

        public virtual OverExtent ParseExtent(Over parent, ExtentType extentType, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            var result = new OverExtent { Parent = parent, ExtentType = extentType };

            if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.BETWEEN))
            {
                result.LowerBound = ParseBound(parseContext);
                ParseUtilities.AssertIsKeyword(tokenList.Take(), parseContext, TSQLKeywords.AND);
                result.UpperBound = ParseBound(parseContext);
            }
            else
            {
                int? bound = ParseBound(parseContext);
                if (bound.HasValue && bound.Value > 0)
                {
                    result.LowerBound = 0;
                    result.UpperBound = bound;
                }
                else
                {
                    result.LowerBound = bound;
                    result.UpperBound = 0;
                }
            }

            return result;
        }

        public virtual int? ParseBound(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.CURRENT))
            {
                TakeKeyword(ROW, parseContext);
                return 0;
            }

            if (tokenList.Peek()?.Text?.ToUpperInvariant() == UNBOUNDED)
            {
                return null;
            }

            int n = (int) ParseUint(parseContext);

            var nextToken = tokenList.Take();
            string boundText = nextToken?.Text?.ToUpperInvariant();
            if (boundText == PRECEDING)
                n = 0 - n;
            else if (boundText != FOLLOWING)
                throw parseContext.ErrorGenerator.ParseException("Expected keyword PRECEDING or FOLLOWING", nextToken);

            return n;
        }

        public virtual uint ParseUint(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            var nextToken = tokenList.Take();
            string text = nextToken?.Text;

            if (uint.TryParse(text, out uint result))
                return result;

            throw parseContext.ErrorGenerator.ParseException($"Expected unsigned int in window extent, found {text ?? ""}", nextToken);
        }

        public virtual ExtentType? TryParseExtentType(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            var keyword = tokenList.Peek()?.Text?.ToUpperInvariant();
            if (keyword != null)
            {
                if (ExtentTypeMap.TryGetValue(keyword, out ExtentType result))
                {
                    tokenList.Advance();
                    return result;
                }
            }

            return null;
        }

        private static void TakeKeyword(string keyword, ParseContext parseContext)
        {
            var token = parseContext.TokenList.Take();
            string text = token.Text?.ToUpperInvariant();
            if (text != keyword)
                throw parseContext.ErrorGenerator.ParseException($"Expected {keyword} keyword, found {text}", token);
        }
    }
}
