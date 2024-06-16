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

        public virtual OrderBy ParseOrderBy(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.ORDER, TSQLKeywords.BY))
            {
                var result = new OrderBy { Parent = parent };
                result.Columns = ParseOrderByColumns(result, parseContext);
                result.Offset = ParseOffset(result, parseContext);
                result.Fetch = ParseFetch(result, parseContext);

                return result;
            }

            return null;
        }

        public virtual List<OrderByColumn> ParseOrderByColumns(SqlElement parent, ParseContext parseContext)
        {
            var result = new List<OrderByColumn>();

            while (true)
            {
                var orderByColumn = new OrderByColumn { Parent = parent };
                orderByColumn.Column = SelectParser.ColumnParser.ParseNextColumn(orderByColumn, parseContext, false);
                orderByColumn.Collate = TryParseCollate(parseContext);
                orderByColumn.SortOrder = TryParseColumnSortOrder(parseContext);

                result.Add(orderByColumn);

                if (!parseContext.TokenList.TryTakeCharacter(TSQLCharacters.Comma))
                    break;
            }

            return result;
        }

        public string TryParseCollate(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.COLLATE))
                return tokenList.Take().Text;

            return null;
        }

        public ColumnSortOrder TryParseColumnSortOrder(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
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

        public Column ParseFetch(OrderBy parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            if (!tokenList.TryTakeKeywords(parseContext, TSQLKeywords.FETCH))
                return null;

            AssertNextIdentifier(parseContext, "FETCH", "NEXT", "FIRST");

            var column = SelectParser.ColumnParser.ParseNextColumn(parent, parseContext, false);
            
            AssertNextIdentifier(parseContext, "FETCH", "ROWS", "ROW");
            AssertNextIdentifier(parseContext, "FETCH", "ONLY");

            return column;
        }

        public Column ParseOffset(OrderBy parent, ParseContext parseContext)
        {
            if (PeekNextIdentifier(parseContext) != "OFFSET")
                return null;
            parseContext.TokenList.Advance();

            var column = SelectParser.ColumnParser.ParseNextColumn(parent, parseContext, false);

            AssertNextIdentifier(parseContext, "OFFSET", "ROWS", "ROW");

            return column;
        }

        protected void AssertNextIdentifier(ParseContext parseContext, string context, params string[] validIdentifiers)
        {
            string x = PeekNextIdentifier(parseContext);
            if (!(validIdentifiers.Contains(x)))
                throw new InvalidOperationException($"Expected {validIdentifiers[0]} keyword in {context}");
            parseContext.TokenList.Advance();
        }

        protected string PeekNextIdentifier(ParseContext parseContext)
        {
            // OFFSET are not recognised as keyowrds by TSQLParser
            var nextIdentifier = parseContext.TokenList.Peek() as TSQLIdentifier;
            return nextIdentifier?.Text?.ToUpperInvariant();
        }
    }
}

