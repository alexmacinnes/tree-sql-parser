using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Selects;
using TSQL;

namespace TreeSqlParser.Parsing
{
    internal class TopParser
    {
        internal static SelectTop ParseTop(SqlElement parent, TokenList tokenList)
        {
            if (!tokenList.TryTakeKeywords(TSQLKeywords.TOP))
                return null;

            var result = new SelectTop { Parent = parent };
            result.Top = ColumnParser.ParseNextColumnSegment(result, tokenList);
            result.Percent = tokenList.TryTakeKeywords(TSQLKeywords.PERCENT);
            result.WithTies = tokenList.TryTakeKeywords(TSQLKeywords.WITH);
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
