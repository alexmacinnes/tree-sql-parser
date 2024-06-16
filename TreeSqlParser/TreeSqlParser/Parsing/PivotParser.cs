using TreeSqlParser.Model;
using TreeSqlParser.Model.Pivoting;
using TSQL;

namespace TreeSqlParser.Parsing
{
    public class PivotParser
    {
        public SelectParser SelectParser { get; set; }

        public virtual Pivot TryParsePivot(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            if (!tokenList.TryTakeKeywords(parseContext, TSQLKeywords.PIVOT))
                return null;

            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses, parseContext);
            var innerTokens = tokenList.TakeBracketedTokens();
            var subcontext = parseContext.Subcontext(innerTokens);

            var result = new Pivot { Parent = parent };
            result.AggregatedColumn = SelectParser.ColumnParser.ParseNextColumn(result, subcontext);

            ParseUtilities.AssertIsKeyword(innerTokens.Take(), subcontext, TSQLKeywords.FOR);
            result.PivotColumn = SelectParser.ColumnParser.ParseNextColumn(result, subcontext);

            ParseUtilities.AssertIsKeyword(innerTokens.Take(), parseContext, TSQLKeywords.IN);
            ParseUtilities.AssertIsChar(innerTokens.Take(), TSQLCharacters.OpenParentheses, parseContext);

            var innerInnerTokens = innerTokens.TakeBracketedTokens();
            var subSubContext = subcontext.Subcontext(innerInnerTokens);
            result.PivotValues = SelectParser.ColumnParser.ParseColumns(result, subSubContext);

            ParseUtilities.AssertIsKeyword(tokenList.Take(), parseContext, TSQLKeywords.AS);
            result.Alias = new SqlIdentifier(ParseUtilities.ParseAlias(tokenList.Take(), parseContext));

            return result;
        }
    }
}
