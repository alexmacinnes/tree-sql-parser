using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSQL;
using TSQL.Tokens;

namespace TreeSqlParser.Parsing
{
    public class ParseUtilities
    {
        public static void AssertIsKeyword(TSQLToken token, ParseContext parseContext, params TSQLKeywords[] keywords)
        {
            var k = token.AsKeyword?.Keyword;
            if (!(k.HasValue && keywords.Contains(k.Value)))
                throw parseContext.ErrorGenerator.ParseException($"Expected keyword {string.Join(",", keywords.Select(x => x.ToString()))}, found {token.Text}", token);
        }

        public static void AssertIsChar(TSQLToken token, TSQLCharacters character, ParseContext parseContext)
        {
            if (!token.IsCharacter(TSQLCharacters.OpenParentheses))
                throw parseContext.ErrorGenerator.ParseException($"Expected character {character.ToString()}, found {token.Text}", token);
        }

        public static int ParseInteger(TSQLToken token, ParseContext parseContext)
        {
            if (token.AsNumericLiteral != null && int.TryParse(token.Text, out int val))
                return val;

            throw parseContext.ErrorGenerator.ParseException($"Expected integer, found {token.Text}", token);
        }

        
        public static string ParseAlias(TSQLToken token, ParseContext parseContext)
        {
            if (token.AsIdentifier != null)
                return token.Text;

            throw parseContext.ErrorGenerator.ParseException($"Expected identifier, found {token.Text}", token);
        }
        
        public static string TryTakeAlias(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            bool tookAs = tokenList.TryTakeKeywords(parseContext, TSQLKeywords.AS);
            var token = tokenList.Peek();
            if (token?.AsIdentifier != null)
                return tokenList.Take().Text;
            else if (tookAs)
                throw parseContext.ErrorGenerator.ParseException("Expected alias after AS", token);
            else
                return null;
        }


        public static int FindLastIndex(List<TSQLToken> tokens, Predicate<TSQLToken> predicate)
        {
            int nestLevel = 0;
            for (int i = tokens.Count-1; i>=0; i--)
            {
                var t = tokens[i];
                if (t.IsCharacter(TSQLCharacters.CloseParentheses))
                    nestLevel++;
                else if (t.IsCharacter(TSQLCharacters.OpenParentheses))
                    nestLevel--;

                if (nestLevel == 0 && predicate(t))
                    return i;
            }

            return -1;
        }
    }
}
