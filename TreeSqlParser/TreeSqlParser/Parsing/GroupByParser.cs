using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Grouping;
using TSQL;

namespace TreeSqlParser.Parsing
{
    public class GroupByParser
    {
        public SelectParser SelectParser { get; set; }

        internal protected virtual List<GroupingSet> ParseGroupBy(SqlElement parent, TokenList tokenList)
        {
            if (!tokenList.TryTakeKeywords(TSQLKeywords.GROUP, TSQLKeywords.BY))
                return null;

            var result = new List<GroupingSet>();
            if (TryTakeText(tokenList, "GROUPING", "SETS"))
            {
                ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses);
                var innerTokens = tokenList.TakeBracketedTokens();
                while (true)
                {
                    result.Add(ParseNextSet(parent, innerTokens));

                    if (!innerTokens.TryTakeCharacter(TSQLCharacters.Comma))
                        break;
                }
            }
            else
            {
                result.Add(ParseNextSet(parent, tokenList));
            }

            return result;
        }

        protected virtual GroupingSet ParseNextSet(SqlElement parent, TokenList tokenList)
        {
            GroupingSetType t =
                TryTakeText(tokenList, "ROLLUP") ? GroupingSetType.Rollup :
                TryTakeText(tokenList, "CUBE") ? GroupingSetType.Cube :
                GroupingSetType.Columns;

            TokenList tokens;
            if (tokenList.TryTakeCharacter(TSQLCharacters.OpenParentheses))
                tokens = tokenList.TakeBracketedTokens();
            else
                tokens = tokenList;

            var result = new GroupingSet
            {
                Parent = parent,
                SetType = t
            };
            result.Columns = SelectParser.ColumnParser.ParseColumns(result, tokens);

            return result;         
        }

        protected static bool TryTakeText(TokenList tokenList, params string[] words)
        {
            for (int i=0; i<words.Length; i++)
            {
                if (tokenList.Peek(i)?.Text?.ToUpperInvariant() != words[i])
                    return false;
            }

            tokenList.Advance(words.Length);
            return true;
        }
    }
}
