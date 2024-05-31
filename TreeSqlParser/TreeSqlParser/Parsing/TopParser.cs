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

        internal protected virtual SelectTop ParseTop(SqlElement parent, TokenList tokenList)
        {
            if (!tokenList.TryTakeKeywords(TSQLKeywords.TOP))
                return null;

            var result = new SelectTop { Parent = parent };
            result.Top = SelectParser.ColumnParser.ParseNextColumnSegment(result, tokenList);
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
