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

        public virtual Over ParseOver(SqlElement parent, TokenList tokenList)
        {
            if (!tokenList.TryTakeKeywords(TSQLKeywords.OVER))
                return null;

            if (!tokenList.TryTakeCharacter(TSQLCharacters.OpenParentheses))
                throw new InvalidOperationException("Expected open parentheses after OVER");

            var innerTokens = tokenList.TakeBracketedTokens();

            var result = new Over { Parent = parent };

            if (innerTokens.Peek()?.Text.ToUpper() == PARTITION)
            {
                innerTokens.Advance();
                ParseUtilities.AssertIsKeyword(innerTokens.Take(), TSQLKeywords.BY);
                result.PartitionBy = SelectParser.ColumnParser.ParseColumns(result, innerTokens);
            }

            if (innerTokens.TryTakeKeywords(TSQLKeywords.ORDER, TSQLKeywords.BY))
                result.OrderBy = SelectParser.OrderByParser.ParseOrderByColumns(result, innerTokens);

            var extentType = TryParseExtentType(innerTokens);
            if (extentType.HasValue)
                result.Extent = ParseExtent(result, extentType.Value, innerTokens);

            return result;
        }

        public virtual OverExtent ParseExtent(Over parent, ExtentType extentType, TokenList tokenList)
        {
            var result = new OverExtent { Parent = parent, ExtentType = extentType };

            if (tokenList.TryTakeKeywords(TSQLKeywords.BETWEEN))
            {
                result.LowerBound = ParseBound(tokenList);
                ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.AND);
                result.UpperBound = ParseBound(tokenList);
            }
            else
            {
                int? bound = ParseBound(tokenList);
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

        public virtual int? ParseBound(TokenList tokenList)
        {
            if (tokenList.TryTakeKeywords(TSQLKeywords.CURRENT))
            {
                TakeKeyword(tokenList, ROW);
                return 0;
            }

            if (tokenList.Peek()?.Text?.ToUpperInvariant() == UNBOUNDED)
            {
                return null;
            }

            int n = (int) ParseUint(tokenList);

            string boundText = tokenList.Take()?.Text?.ToUpperInvariant();
            if (boundText == PRECEDING)
                n = 0 - n;
            else if (boundText != FOLLOWING)
                throw new InvalidOperationException("Expected keyword PRECEDING or FOLLOWING");

            return n;
        }

        public virtual uint ParseUint(TokenList tokenList)
        {
            string text = tokenList.Take().Text;

            if (uint.TryParse(text, out uint result))
                return result;

            throw new InvalidOperationException($"Expected unsigned int in window extent, found {text}");
        }

        public virtual ExtentType? TryParseExtentType(TokenList tokenList)
        {
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

        private static void TakeKeyword(TokenList tokenList, string keyword)
        {
            string text = tokenList.Take()?.Text?.ToUpperInvariant();
            if (text != keyword)
                throw new InvalidOperationException($"Expected {keyword} keyword, found {text}");
        }
    }
}
