using System;
using System.Collections.Generic;
using System.Linq;
using TreeSqlParser.Metadata;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing.Errors;
using TSQL;
using TSQL.Tokens;

namespace TreeSqlParser.Parsing
{
    public class SelectParser
    {
        public ColumnParser ColumnParser { get; protected set; }

        public ConditionParser ConditionParser { get; protected set; }

        public GroupByParser GroupByParser { get; protected set; }

        public OrderByParser OrderByParser { get; protected set; }

        public OverParser OverParser { get; protected set; }

        public PivotParser PivotParser { get; protected set; }

        public RelationParser RelationParser { get; protected set; }

        public SelectOptionsParser SelectOptionsParser { get; protected set; }

        public TopParser TopParser { get; protected set; }

        public SelectParser()
        {
            ColumnParser = new ColumnParser() { SelectParser = this };
            ConditionParser = new ConditionParser() { SelectParser = this };  
            GroupByParser = new GroupByParser() { SelectParser = this };
            OrderByParser = new OrderByParser() { SelectParser = this };
            OverParser = new OverParser() { SelectParser = this };
            PivotParser = new PivotParser() { SelectParser = this };
            RelationParser = new RelationParser() { SelectParser = this };
            SelectOptionsParser = new SelectOptionsParser();
            TopParser = new TopParser() { SelectParser = this };
        }

        private static readonly Dictionary<TSQLKeywords, SetModifier> SetModifiersMap = new Dictionary<TSQLKeywords, SetModifier>()
        {
            { TSQLKeywords.UNION, SetModifier.Union },
            { TSQLKeywords.INTERSECT, SetModifier.Intersect },
            { TSQLKeywords.EXCEPT, SetModifier.Except }
        };

        private static ParseContext GetParseContext(string sql)
        {
            var tokens = TSQLTokenizer.ParseTokens(sql, includeWhitespace: true);
            var grouped = tokens.GroupBy(x => x is TSQLWhitespace).ToArray();
            var whitespace = grouped.FirstOrDefault(g => g.Key)?.Cast<TSQLWhitespace>()?.ToList() ?? new List<TSQLWhitespace>();
            var nonWhitespace = grouped.FirstOrDefault(g => !g.Key).ToList() ?? new List<TSQLToken>();

            var errorGenerator = new ErrorGenerator(whitespace);
            KeywordMetadata.CheckKeywordBlacklist(nonWhitespace, errorGenerator);
            
            var tokenList = new TokenList(nonWhitespace);

            return new ParseContext
            {
                TokenList = tokenList,
                ErrorGenerator = errorGenerator
            };
        }

        public SqlRootElement ParseSelectStatement(string sql)
        {
            var parseContext = GetParseContext(sql);

            var result = new SqlRootElement();
            result.Child = ParseSelectStatement(result, parseContext);

            return result;
        }

        public SqlRootElement ParseColumn(string sql)
        {
            var c = ParseColumnInternal(sql);
            var result = new SqlRootElement
            {
                Child = c
            };
            c.Parent = result;

            return result;
        }

        internal Column ParseColumnInternal(string sql)
        {
            var parseContext = GetParseContext(sql);
            return ColumnParser.ParseNextColumn(null, parseContext);
        }

        public SqlRootElement ParseCondition(string sql)
        {
            var c = ParseConditionInternal(sql);
            var result = new SqlRootElement
            {
                Child = c
            };
            c.Parent = result;

            return result;
        }

        internal Condition ParseConditionInternal(string sql)
        {
            var parseContext = GetParseContext(sql);
            return ConditionParser.ParseCondition(null, parseContext);
        }

        public SqlRootElement ParseSelect(string sql)
        {
            var s = ParseSelectInternal(sql);
            var result = new SqlRootElement
            {
                Child = s
            };
            s.Parent = result;

            return result;
        }

        internal Select ParseSelectInternal(string sql)
        {
            var parseContext = GetParseContext(sql);
            return ParseNextSelect(null, parseContext);
        }

        public List<SqlIdentifier> ParseMultiPartIdentifier(string sql)
        {
            var parseContext = GetParseContext(sql);
            return ParseMultiPartIndentifier(parseContext).Select(x => new SqlIdentifier(x)).ToList();
        }

        public virtual SelectStatement ParseSelectStatement(SqlElement parent, ParseContext parseContext)
        {
            var selectStatement = new SelectStatement { Parent = parent };

            selectStatement.WithSelects = ParseWithSelects(selectStatement, parseContext);
            selectStatement.Selects = ParseSelects(selectStatement, parseContext);

            selectStatement.OrderBy = OrderByParser.ParseOrderBy(selectStatement, parseContext);

            selectStatement.Options = SelectOptionsParser.ParseSelectOptions(selectStatement, parseContext);

            if (parseContext.TokenList.HasMore)
                throw parseContext.ErrorGenerator.ParseException("Parsing incomplete", parseContext.TokenList.Peek());

            return selectStatement;
        }

        protected virtual List<CteSelect> ParseWithSelects(SelectStatement parent, ParseContext parseContext)
        {
            var result = new List<CteSelect>();

            var tokenList = parseContext.TokenList;

            if (!tokenList.TryTakeKeywords(parseContext, TSQLKeywords.WITH))
                return result;

            while(true)
            {
                result.Add(ParseNextWithSelect(parent, parseContext));

                if (!tokenList.TryTakeCharacter(TSQLCharacters.Comma))
                    break;
            }

            return result;
        }

        protected virtual CteSelect ParseNextWithSelect(SelectStatement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            var result = new CteSelect { 
                Parent = parent, 
                Alias = new SqlIdentifier(ParseUtilities.ParseAlias(tokenList.Take(), parseContext)) 
            };

            ParseUtilities.AssertIsKeyword(tokenList.Take(), parseContext, TSQLKeywords.AS);
            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses, parseContext);

            var innerContext = parseContext.Subcontext(tokenList.TakeBracketedTokens(parseContext.ErrorGenerator));
            var innerStatement = ParseSelectStatement(null, innerContext);

            innerStatement.Selects.ForEach(x => x.Parent = result);
            result.Selects = innerStatement.Selects;

            return result;
        }

        protected virtual List<Select> ParseSelects(SelectStatement parent, ParseContext parseContext)
        {
            var result = new List<Select>();

            while (true)
            {
                var select = ParseNextSelect(parent, parseContext);
                if (select != null)
                    result.Add(select);
                else
                    break;
            }

            return result;
        }

        protected virtual Select ParseNextSelect(SelectStatement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            if (!tokenList.HasMore)
                return null;

            var select = new Select { Parent = parent };

            select.SetModifier = ParseSetModifier(parseContext);

            if (!tokenList.TryTakeKeywords(parseContext, TSQLKeywords.SELECT))
                return null;

            select.Top = TopParser.ParseTop(select, parseContext);
            select.Distinct = ParseDistinct(parseContext);

            select.Columns = ColumnParser.ParseColumns(select, parseContext);
            select.From = RelationParser.ParseRelations(select, parseContext);
            select.Pivot = PivotParser.TryParsePivot(select, parseContext);

            if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.WHERE))
                select.WhereCondition = ConditionParser.ParseCondition(select, parseContext);

            select.GroupBy = GroupByParser.ParseGroupBy(select, parseContext);

            if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.HAVING))
                select.HavingCondition = ConditionParser.ParseCondition(select, parseContext);

            return select;
        }

        protected virtual bool ParseDistinct(ParseContext parseContext)
        {
            return parseContext.TokenList.TryTakeKeywords(parseContext, TSQLKeywords.DISTINCT);
        }

        protected virtual SetModifier ParseSetModifier(ParseContext parseContext)
        {
            SetModifier result = SetModifier.None;

            var tokenList = parseContext.TokenList;
            var nextToken = tokenList.Peek()?.AsKeyword;
            if (nextToken != null)
            {
                if (SetModifiersMap.TryGetValue(nextToken.Keyword, out result))
                {
                    tokenList.Advance();
                    if (result == SetModifier.Union && tokenList.Peek()?.IsKeyword(TSQLKeywords.ALL) == true)
                    {
                        result = SetModifier.UnionAll;
                        tokenList.Advance();
                    }
                }
            }

            return result;            
        }

        internal protected virtual List<string> ParseMultiPartIndentifier(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            var token = tokenList.Peek();
            if (!(token?.AsIdentifier is TSQLIdentifier))
                throw parseContext.ErrorGenerator.ParseException($"Expected identifier, got {token?.Text}", token);

            var result = new List<string> { tokenList.Take().Text };

            while (tokenList.HasMore
                && tokenList.Peek().IsCharacter(TSQLCharacters.Period)
                && tokenList.Peek(1)?.AsIdentifier is TSQLIdentifier)
            {
                result.Add(tokenList.Peek(1).Text);
                tokenList.Advance(2);
            }

            if (tokenList.HasMore
                && tokenList.Peek().IsCharacter(TSQLCharacters.Period)
                && tokenList.Peek(1)?.Text == "*")
            {
                result.Add(tokenList.Peek(1).Text);
                tokenList.Advance(2);
            }

            return result;
        }
    }
}
