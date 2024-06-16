﻿using System;
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
                throw new InvalidOperationException("Expected open parentheses after OPTION");

            var innerTokens = tokenList.TakeBracketedTokens();
            var result = new SelectOptions() { Parent = parent, Options = new List<string>() };

            while (innerTokens.HasMore)
                result.Options.Add(innerTokens.ParseTextUntilComma(" "));

            return result;
        }
    }
}
