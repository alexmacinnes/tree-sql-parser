using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Pivoting;
using TSQL;

namespace TreeSqlParser.Parsing
{
    public class PivotParser
    {
        public SelectParser SelectParser { get; set; }

        public virtual Pivot TryParsePivot(SqlElement parent, TokenList tokenList)
        {
            if (!tokenList.TryTakeKeywords(TSQLKeywords.PIVOT))
                return null;

            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses);
            var innerTokens = tokenList.TakeBracketedTokens();

            var result = new Pivot { Parent = parent };
            result.AggregatedColumn = SelectParser.ColumnParser.ParseNextColumn(result, innerTokens);

            ParseUtilities.AssertIsKeyword(innerTokens.Take(), TSQLKeywords.FOR);
            result.PivotColumn = SelectParser.ColumnParser.ParseNextColumn(result, innerTokens);

            ParseUtilities.AssertIsKeyword(innerTokens.Take(), TSQLKeywords.IN);
            ParseUtilities.AssertIsChar(innerTokens.Take(), TSQLCharacters.OpenParentheses);
            var innerInnerTokens = innerTokens.TakeBracketedTokens();
            result.PivotValues = SelectParser.ColumnParser.ParseColumns(result, innerInnerTokens);

            ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.AS);
            result.Alias = new SqlIdentifier(ParseUtilities.ParseAlias(tokenList.Take()));

            return result;
        }
    }
}
