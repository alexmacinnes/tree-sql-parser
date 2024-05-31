using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;
using TSQL;
using TSQL.Tokens;

namespace TreeSqlParser.Parsing
{
    public class OrderByParser
    {
        public SelectParser SelectParser { get; set; }

        private static readonly IReadOnlyDictionary<TSQLKeywords, ColumnSortOrder> ColumnSortOrderMap = new Dictionary<TSQLKeywords, ColumnSortOrder>
        {
            { TSQLKeywords.ASC, ColumnSortOrder.Asc },
            { TSQLKeywords.DESC, ColumnSortOrder.Desc }
        };

        internal protected virtual OrderBy ParseOrderBy(SqlElement parent, TokenList tokenList)
        {
            if (tokenList.TryTakeKeywords(TSQLKeywords.ORDER, TSQLKeywords.BY))
            {
                var result = new OrderBy { Parent = parent };
                result.Columns = ParseOrderByColumns(result, tokenList);
                result.Offset = ParseOffset(result, tokenList);
                result.Fetch = ParseFetch(result, tokenList);

                return result;
            }

            return null;
        }

        internal protected virtual List<OrderByColumn> ParseOrderByColumns(SqlElement parent, TokenList tokenList)
        {
            var result = new List<OrderByColumn>();

            while (true)
            {
                var orderByColumn = new OrderByColumn { Parent = parent };
                orderByColumn.Column = SelectParser.ColumnParser.ParseNextColumn(orderByColumn, tokenList, false);
                orderByColumn.Collate = TryParseCollate(tokenList);
                orderByColumn.SortOrder = TryParseColumnSortOrder(tokenList);

                result.Add(orderByColumn);

                if (!tokenList.TryTakeCharacter(TSQLCharacters.Comma))
                    break;
            }

            return result;
        }

        protected virtual string TryParseCollate(TokenList tokenList)
        {
            if (tokenList.TryTakeKeywords(TSQLKeywords.COLLATE))
                return tokenList.Take().Text;

            return null;
        }

        protected virtual ColumnSortOrder TryParseColumnSortOrder(TokenList tokenList)
        {
            var nextToken = tokenList.Peek();
            if (nextToken == null || !(nextToken is TSQLKeyword))
                return ColumnSortOrder.Asc;

            var keyword = ((TSQLKeyword)nextToken).Keyword;
            if (ColumnSortOrderMap.TryGetValue(keyword, out ColumnSortOrder order))
            {
                tokenList.Advance();
                return order;
            }

            return ColumnSortOrder.Asc;
        }

        protected virtual Column ParseFetch(OrderBy parent, TokenList tokenList)
        {
            if (!tokenList.TryTakeKeywords(TSQLKeywords.FETCH))
                return null;

            AssertNextIdentifier(tokenList, "FETCH", "NEXT", "FIRST");

            var column = SelectParser.ColumnParser.ParseNextColumn(parent, tokenList, false);
            
            AssertNextIdentifier(tokenList, "FETCH", "ROWS", "ROW");
            AssertNextIdentifier(tokenList, "FETCH", "ONLY");

            return column;
        }

        protected virtual Column ParseOffset(OrderBy parent, TokenList tokenList)
        {
            if (PeekNextIdentifier(tokenList) != "OFFSET")
                return null;
            tokenList.Advance();

            var column = SelectParser.ColumnParser.ParseNextColumn(parent, tokenList, false);

            AssertNextIdentifier(tokenList, "OFFSET", "ROWS", "ROW");

            return column;
        }

        protected void AssertNextIdentifier(TokenList tokenList, string context, params string[] validIdentifiers)
        {
            string x = PeekNextIdentifier(tokenList);
            if (!(validIdentifiers.Contains(x)))
                throw new InvalidOperationException($"Expected {validIdentifiers[0]} keyword in {context}");
            tokenList.Advance();
        }

        protected string PeekNextIdentifier(TokenList tokenList)
        {
            // OFFSET are not recognised as keyowrds by TSQLParser
            var nextIdentifier = tokenList.Peek() as TSQLIdentifier;
            return nextIdentifier?.Text?.ToUpperInvariant();
        }
    }
}

