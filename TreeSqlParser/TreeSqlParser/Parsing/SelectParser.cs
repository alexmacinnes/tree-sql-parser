using System;
using System.Collections.Generic;
using System.Linq;
using TreeSqlParser.Metadata;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;
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

        private static TokenList GetTokenList(string sql)
        {
            var tokens = TSQLTokenizer.ParseTokens(sql);
            KeywordMetadata.CheckKeywordBlacklist(tokens);
            return new TokenList(tokens);
        }

        public SqlRootElement ParseSelectStatement(string sql)
        {
            var tokenList = GetTokenList(sql);

            var result = new SqlRootElement();
            result.Child = ParseSelectStatement(result, tokenList);

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
            var tokenList = GetTokenList(sql);
            return ColumnParser.ParseNextColumn(null, tokenList);
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
            var tokenList = GetTokenList(sql);
            return ConditionParser.ParseCondition(null, tokenList);
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
            var tokenList = GetTokenList(sql);
            return ParseNextSelect(null, tokenList);
        }

        public List<SqlIdentifier> ParseMultiPartIdentifier(string sql)
        {
            var tokenList = GetTokenList(sql);
            return ParseMultiPartIndentifier(tokenList).Select(x => new SqlIdentifier(x)).ToList();
        }

        public virtual SelectStatement ParseSelectStatement(SqlElement parent, TokenList tokenList)
        {
            var selectStatement = new SelectStatement { Parent = parent };

            selectStatement.WithSelects = ParseWithSelects(selectStatement, tokenList);
            selectStatement.Selects = ParseSelects(selectStatement, tokenList);

            selectStatement.OrderBy = OrderByParser.ParseOrderBy(selectStatement, tokenList);

            selectStatement.Options = SelectOptionsParser.ParseSelectOptions(selectStatement, tokenList);

            if (tokenList.HasMore)
                throw new InvalidCastException("Parsing incomplete");

            return selectStatement;
        }

        protected virtual List<CteSelect> ParseWithSelects(SelectStatement parent, TokenList tokenList)
        {
            var result = new List<CteSelect>();

            if (!tokenList.TryTakeKeywords(TSQLKeywords.WITH))
                return result;

            while(true)
            {
                result.Add(ParseNextWithSelect(parent, tokenList));

                if (!tokenList.TryTakeCharacter(TSQLCharacters.Comma))
                    break;
            }

            return result;
        }

        protected virtual CteSelect ParseNextWithSelect(SelectStatement parent, TokenList tokenList)
        {
            var result = new CteSelect { 
                Parent = parent, 
                Alias = new SqlIdentifier(ParseUtilities.ParseAlias(tokenList.Take())) 
            };

            ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.AS);
            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses);

            var innerTokenList = tokenList.TakeBracketedTokens();
            var innerStatement = ParseSelectStatement(null, innerTokenList);

            innerStatement.Selects.ForEach(x => x.Parent = result);
            result.Selects = innerStatement.Selects;

            return result;
        }

        protected virtual List<Select> ParseSelects(SelectStatement parent, TokenList tokenList)
        {
            var result = new List<Select>();

            while (true)
            {
                var select = ParseNextSelect(parent, tokenList);
                if (select != null)
                    result.Add(select);
                else
                    break;
            }

            return result;
        }

        protected virtual Select ParseNextSelect(SelectStatement parent, TokenList tokenList)
        {
            if (!tokenList.HasMore)
                return null;

            var select = new Select { Parent = parent };

            select.SetModifier = ParseSetModifier(tokenList);

            if (!tokenList.TryTakeKeywords(TSQLKeywords.SELECT))
                return null;

            select.Top = TopParser.ParseTop(select, tokenList);
            select.Distinct = ParseDistinct(tokenList);

            select.Columns = ColumnParser.ParseColumns(select, tokenList);
            select.From = RelationParser.ParseRelations(select, tokenList);
            select.Pivot = PivotParser.TryParsePivot(select, tokenList);

            if (tokenList.TryTakeKeywords(TSQLKeywords.WHERE))
                select.WhereCondition = ConditionParser.ParseCondition(select, tokenList);

            select.GroupBy = GroupByParser.ParseGroupBy(select, tokenList);

            if (tokenList.TryTakeKeywords(TSQLKeywords.HAVING))
                select.HavingCondition = ConditionParser.ParseCondition(select, tokenList);

            return select;
        }

        protected virtual bool ParseDistinct(TokenList tokenList)
        {
            return tokenList.TryTakeKeywords(TSQLKeywords.DISTINCT);
        }

        protected virtual SetModifier ParseSetModifier(TokenList tokenList)
        {
            SetModifier result = SetModifier.None;

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

        internal protected virtual List<string> ParseMultiPartIndentifier(TokenList tokenList)
        {
            if (!(tokenList.Peek()?.AsIdentifier is TSQLIdentifier))
                throw new InvalidOperationException($"Expected identifier, got {tokenList.Peek()?.Text}");

            var result = new List<string> { tokenList.Take().Text };

            while (tokenList.HasMore
                && tokenList.Peek().IsCharacter(TSQLCharacters.Period)
                && tokenList.Peek(1)?.AsIdentifier is TSQLIdentifier)
            {
                result.Add(tokenList.Peek(1).Text);
                tokenList.Advance(2);
            }

            return result;
        }
    }
}
