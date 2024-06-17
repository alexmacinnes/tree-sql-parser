using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Selects;
using TSQL;

namespace TreeSqlParser.Parsing
{
    public class TopParser
    {
        public SelectParser SelectParser { get; set; }

        public virtual SelectTop ParseTop(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            if (!tokenList.TryTakeKeywords(parseContext, TSQLKeywords.TOP))
                return null;

            var result = new SelectTop { Parent = parent };
            result.Top = SelectParser.ColumnParser.ParseNextColumnSegment(result, parseContext);
            result.Percent = tokenList.TryTakeKeywords(parseContext, TSQLKeywords.PERCENT);
            result.WithTies = tokenList.TryTakeKeywords(parseContext, TSQLKeywords.WITH);
            if (result.WithTies)
            {
                var token = tokenList.Take();
                string nextTokenText = token?.Text?.ToUpperInvariant();
                if (nextTokenText != "TIES")
                    parseContext.ErrorGenerator.ParseException("Expected TIES following WITH in TOP", token);
            }

            return result;
        }
    }
}
