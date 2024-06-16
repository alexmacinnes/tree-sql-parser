using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using TSQL;
using TSQL.Tokens;

namespace TreeSqlParser.Parsing
{
    [DebuggerDisplay("{DebugText}")]
    public class TokenList
    {
        private readonly List<TSQLToken> tokens;

        private int currentIndex;

        private int endIndex;                       // inclusive index of final token in range

        public TokenList(List<TSQLToken> tokens)
            : this(tokens, 0, tokens.Count - 1)
        { }

        public TokenList(List<TSQLToken> tokens, int startIndex, int endIndex)
        {
            this.tokens = tokens;
            this.endIndex = endIndex;
            this.currentIndex = startIndex;
        }

        public TSQLToken Peek(int forward = 0) => TryGet(currentIndex + forward);

        public TSQLToken PeekEnd(int back = 0) => TryGet(endIndex - back);

        public TSQLToken Take() => TryGet(currentIndex++);

        public void Advance(int n = 1) => currentIndex += n;

        public bool HasMore => currentIndex <= endIndex;

        public bool HasNMore(int n) => currentIndex + n - 1 <= endIndex;

        public TokenList TakeBracketedTokens()
        {
            int nestLevel = 1;          // assume nextToken is immediately following opening bracket
            for (int i = currentIndex; i <= endIndex; i++)
            {
                var t = tokens[i];
                if (t.IsCharacter(TSQLCharacters.OpenParentheses))
                    nestLevel++;
                else if (t.IsCharacter(TSQLCharacters.CloseParentheses))
                {
                    nestLevel--;
                    if (nestLevel == 0)
                    {

                        var innerRange = tokens.GetRange(currentIndex, i - currentIndex);
                        var innerTokenList = new TokenList(innerRange);
                        currentIndex = i + 1;          //point after the closing bracket
                        return innerTokenList;
                    }
                }
            }

            throw new InvalidOperationException("Did not find closing bracket");
        }

        /*
        public TokenList TruncateAfter(Predicate<TSQLToken> selector)
        {
            int index = FindLastIndex(selector);
            if (!index.ha)
            for (int i=endIndex; i>=currentIndex; i--)
            {
                if (selector(TryGet(i)))
                {
                    endIndex = i - 1;
                    var endRange = tokens.GetRange(i + 1, endIndex - i);
                    return new TokenList(endRange);
                }
            }

            return null;
        }
        */

        public int? FindLastIndex(Predicate<TSQLToken> selector)
        {
            for (int i = endIndex; i >= currentIndex; i--)
                if (selector(TryGet(i)))
                    return i;
            return null;
        }

        public void RemoveLastNTokens(int n)
        {
            endIndex -= n;
            tokens.RemoveRange(tokens.Count() - n, n);
        }

        public string ConcatText => string.Join(string.Empty, RemainingTokens.Select(x => x.Text));

        public int RemainingCount => endIndex - currentIndex + 1;

        public IEnumerable<TSQLToken> RemainingTokens => tokens.Skip(currentIndex);

        private TSQLToken TryGet(int index) => index > endIndex ? null : tokens[index];

        public string ParseTextUntilComma(string separator = "")
        {
            var sb = new StringBuilder();

            int nestLevel = 0;
            while (currentIndex <= endIndex)
            {
                var t = Peek();
                if (t.IsCharacter(TSQLCharacters.OpenParentheses))
                    nestLevel++;
                else if (t.IsCharacter(TSQLCharacters.CloseParentheses))
                {
                    nestLevel--;
                    if (nestLevel < 0)
                        throw new InvalidOperationException("Unexpected closing bracket");
                }

                if (nestLevel == 0 && t.IsCharacter(TSQLCharacters.Comma))
                {
                    currentIndex++;         // skip past the comma
                    break;
                }
                else
                {
                    if (sb.Length > 0)
                        sb.Append(separator);
                    sb.Append(t.Text);
                }                

                currentIndex++;
            }

            return sb.ToString();
        }

        public bool TryTakeKeywords(TSQLKeywords first, ParseContext parseContext, params TSQLKeywords[] others)
        {
            if (Peek()?.IsKeyword(first) != true)
                return false;
            Advance();

            foreach (var x in others)
                ParseUtilities.AssertIsKeyword(Take(), parseContext, x);

            return true;
        }

        public bool TryTakeCharacter(TSQLCharacters c)
        {
            if (Peek()?.IsCharacter(c) != true)
                return false;
            Advance();

            return true;
        }

        public TokenList CloneFromCurrentPosition()
        {
            var remainingTokens = tokens.Skip(currentIndex).ToList();
            var result = new TokenList(remainingTokens);

            return result;
        }

        public string DebugText => string.Join(" | ", RemainingTokens.Select(x => x.Text));

    }
}
