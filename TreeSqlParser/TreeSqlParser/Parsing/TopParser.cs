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

            if (!tokenList.TryTakeKeywords(TSQLKeywords.TOP, parseContext))
                return null;

            var result = new SelectTop { Parent = parent };
            result.Top = SelectParser.ColumnParser.ParseNextColumnSegment(result, parseContext);
            result.Percent = tokenList.TryTakeKeywords(TSQLKeywords.PERCENT, parseContext);
            result.WithTies = tokenList.TryTakeKeywords(TSQLKeywords.WITH, parseContext);
            if (result.WithTies)
            {
                string nextToken = tokenList.Take()?.Text?.ToUpperInvariant();
                if (nextToken != "TIES")
                    throw new InvalidOperationException("Expected TIES following WITH in TOP");
            }

            return result;
        }
    }
}
