using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Hints;
using TSQL;

namespace TreeSqlParser.Parsing
{
    public class SelectOptionsParser
    {
        public virtual SelectOptions ParseSelectOptions(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            if (!tokenList.TryTakeKeywords(parseContext, TSQLKeywords.OPTION))
                return null;

            if (!tokenList.TryTakeCharacter(TSQLCharacters.OpenParentheses))
                throw parseContext.ErrorGenerator.ParseException("Expected open parentheses after OPTION", tokenList.Peek());

            var innerTokens = tokenList.TakeBracketedTokens(parseContext.ErrorGenerator);
            var result = new SelectOptions() { Parent = parent, Options = new List<string>() };

            while (innerTokens.HasMore)
                result.Options.Add(innerTokens.ParseTextUntilComma(parseContext.ErrorGenerator, " "));

            return result;
        }
    }
}
