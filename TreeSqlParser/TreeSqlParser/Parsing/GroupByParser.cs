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

        public virtual List<GroupingSet> ParseGroupBy(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            if (!tokenList.TryTakeKeywords(TSQLKeywords.GROUP, parseContext, TSQLKeywords.BY))
                return null;

            var result = new List<GroupingSet>();
            if (TryTakeText(tokenList, "GROUPING", "SETS"))
            {
                ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses, parseContext);
                var innerTokens = tokenList.TakeBracketedTokens();
                while (true)
                {
                    var subContext = parseContext.Subcontext(innerTokens);
                    result.Add(ParseNextSet(parent, subContext));

                    if (!innerTokens.TryTakeCharacter(TSQLCharacters.Comma))
                        break;
                }
            }
            else
            {
                result.Add(ParseNextSet(parent, parseContext));
            }

            return result;
        }

        public virtual GroupingSet ParseNextSet(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            GroupingSetType t =
                TryTakeText(tokenList, "ROLLUP") ? GroupingSetType.Rollup :
                TryTakeText(tokenList, "CUBE") ? GroupingSetType.Cube :
                GroupingSetType.Columns;

            ParseContext subContext;
            if (tokenList.TryTakeCharacter(TSQLCharacters.OpenParentheses))
                subContext = parseContext.Subcontext(tokenList.TakeBracketedTokens());
            else
                subContext = parseContext;

            var result = new GroupingSet
            {
                Parent = parent,
                SetType = t
            };
            result.Columns = SelectParser.ColumnParser.ParseColumns(result, subContext);

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
