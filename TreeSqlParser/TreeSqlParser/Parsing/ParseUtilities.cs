using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TSQL;
using TSQL.Tokens;

namespace TreeSqlParser.Parsing
{
    internal class ParseUtilities
    {
        public static void AssertIsKeyword(TSQLToken token, params TSQLKeywords[] keywords)
        {
            var k = token.AsKeyword?.Keyword;
            if (!(k.HasValue && keywords.Contains(k.Value)))
                throw new InvalidOperationException($"Expected keyword {string.Join(",", keywords.Select(x => x.ToString()))}, found {token.Text}");
        }

        public static void AssertIsChar(TSQLToken token, TSQLCharacters character)
        {
            if (!token.IsCharacter(TSQLCharacters.OpenParentheses))
                throw new InvalidOperationException($"Expected character {character.ToString()}, found {token.Text}");
        }

        public static int ParseInteger(TSQLToken token)
        {
            if (token.AsNumericLiteral != null && int.TryParse(token.Text, out int val))
                return val;

            throw new InvalidOperationException($"Expected integer, found {token.Text}");
        }

        
        public static string ParseAlias(TSQLToken token)
        {
            if (token.AsIdentifier != null)
                return token.Text;

            throw new InvalidOperationException($"Expected identifier, found {token.Text}");
        }
        
        public static string TryTakeAlias(TokenList tokenList)
        {
            bool tookAs = tokenList.TryTakeKeywords(TSQLKeywords.AS);
            if (tokenList.Peek()?.AsIdentifier != null)
                return tokenList.Take().Text;
            else if (tookAs)
                throw new InvalidOperationException("Expected alias after AS");
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
