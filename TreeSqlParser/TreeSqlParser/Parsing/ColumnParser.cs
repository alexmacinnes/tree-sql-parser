using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Parsing.Enums;
using TSQL;
using TSQL.Tokens;
using static System.Net.Mime.MediaTypeNames;


namespace TreeSqlParser.Parsing
{
    public class ColumnParser
    {
        private static readonly IReadOnlyDictionary<string, Aggregation> AggregationsMap = EnumUtilities.ToDictionary<Aggregation>();

        private static readonly IReadOnlyDictionary<string, ArithmeticOperator> ArithmeticOperatorsMap = EnumUtilities.ToDictionary<ArithmeticOperator, EnumParseTextAttribute>();

        public SelectParser SelectParser { get; set; }

        public virtual List<Column> ParseColumns(SqlElement parent, ParseContext parseContext)
        {
            var result = new List<Column>();

            var tokenList = parseContext.TokenList;
            if (tokenList.HasMore)
            {
                while (true)
                {
                    var column = ParseNextColumn(parent, parseContext);
                    result.Add(column);

                    if (!tokenList.TryTakeCharacter(TSQLCharacters.Comma))
                        break;
                }
            }

            return result;
        }

        public virtual Column ParseNextColumnSegment(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            var nextToken = tokenList.Peek();            
            string nextTokenText = nextToken.Text.ToUpperInvariant();

            if (nextToken is TSQLIdentifier && nextToken.Text.StartsWith("{"))
                return ParseDateTimeEscapeSequence(parent, parseContext);
            if (nextToken is TSQLOperator && nextTokenText == "-" && tokenList.Peek(1) is TSQLNumericLiteral)
                return ParseNegativeNumericLiteral(parent, parseContext);
            if (nextToken.AsLiteral != null)
                return ParseLiteralColumn(parent, parseContext);
            if (nextToken.AsVariable != null)
                return ParseVariableColumn(parent, parseContext);
            else if (nextToken.IsKeyword(TSQLKeywords.LEFT) || nextToken.IsKeyword(TSQLKeywords.RIGHT) || nextTokenText == "REPLICATE" || nextTokenText == "ABS")
                return ParseKeywordAsFunc(parent, parseContext);
            else if (nextToken.AsIdentifier != null)
                return ParseIdentifier(parent, parseContext);
            else if (nextToken.IsCharacter(TSQLCharacters.OpenParentheses))
                return ParseBracketedColumn(parent, parseContext);
            else if (nextToken.IsKeyword(TSQLKeywords.CASE))
                return ParseCaseColumn(parent, parseContext);
            else if (nextToken.IsKeyword(TSQLKeywords.NULL))
                return ParseNullColumn(parent, parseContext);
            else if (nextToken.AsOperator != null && nextToken.Text == "*")
            {
                tokenList.Advance();
                return new StarColumn() { Parent = parent };
            }
            else
                throw parseContext.ErrorGenerator.ParseException("Unkown column type", nextToken);
        }

        public virtual Column ParseDateTimeEscapeSequence(SqlElement parent, ParseContext parseContext)
        {
            string dateTimeType;

            var tokenList = parseContext.TokenList;
            var token = tokenList.Take();
            string firstText = token.Text;
            if (firstText.Length > 1)
                dateTimeType = firstText.Substring(1);
            else
            {
                token = tokenList.Take();
                dateTimeType = token.Text;
            }

            Column result;
            switch (dateTimeType)
            {
                case "d":
                    result = new DateColumn { Parent = parent, Value = DateTime.Parse(Unquote(parseContext)) };
                    break;
                case "ts":
                    result = new DateTimeColumn { Parent = parent, Value = DateTime.Parse(Unquote(parseContext)) };
                    break;
                case "t":
                    result = new TimeColumn { Parent = parent, Value = TimeSpan.Parse(Unquote(parseContext)) };
                    break;
                default:
                    throw parseContext.ErrorGenerator.ParseException("Unknown datetime literal type: " + dateTimeType, token);
            }

            token = tokenList.Take();
            if (token.Text != "}")
                throw parseContext.ErrorGenerator.ParseException("Expected closing brace: }", token);

            return result;
        }

        public string Unquote(ParseContext parseContext)
        {
            var token = parseContext.TokenList.Take();
            string text = token?.Text;
            if (text != null && text.Length > 1 && text.StartsWith("'") && text.EndsWith("'"))
                return text.Substring(1, text.Length - 2);

            throw parseContext.ErrorGenerator.ParseException("Expected quoted text", token);
        }



        public virtual Column ParseNegativeNumericLiteral(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            var token = tokenList.Take();
            string neg = token.Text;
            if (neg != "-")
                throw parseContext.ErrorGenerator.ParseException("", token);

            var numLiteral = (TSQLNumericLiteral) tokenList.Take();

            Column result;
            if (long.TryParse(numLiteral.Text, out long l))
                result = new IntegerColumn { Parent = parent, Value = 0 - l };
            else
            {
                decimal d = decimal.Parse(numLiteral.Text);
                result = new DecimalColumn { Parent = parent, Value = 0m - d };
            }

            return result;
        }

        public virtual Column ParseKeywordAsFunc(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            var errorToken = tokenList.Peek();

            var fullName = new[] { new SqlIdentifier() { Name = tokenList.Take().Text } };
            if (!tokenList.TryTakeCharacter(TSQLCharacters.OpenParentheses))
                throw parseContext.ErrorGenerator.ParseException("Expected open parenthesis after: " + fullName[0].Name, errorToken);

            var funcColumn = new FunctionColumn { Name = fullName, Parent = parent };

            var subContext = parseContext.Subcontext(tokenList.TakeBracketedTokens(parseContext.ErrorGenerator));
            funcColumn.Parameters = ParseColumns(funcColumn, subContext);

            return funcColumn;
        }

        public virtual VariableColumn ParseVariableColumn(SqlElement parent, ParseContext parseContext)
        {
            return new VariableColumn
            {
                Parent = parent,
                VariableName = parseContext.TokenList.Take().AsVariable.Text.Substring(1)
            };
        }

        public virtual Column ParseCastColumn(SqlElement parent, bool tryCast, ParseContext parseContext)
        {
            var errorToken = parseContext.TokenList.Peek();
            var innerTokens = parseContext.TokenList.TakeBracketedTokens(parseContext.ErrorGenerator);

            int? asIndex = innerTokens.FindLastIndex(x => x.IsKeyword(TSQLKeywords.AS));
            if (!asIndex.HasValue)
                throw parseContext.ErrorGenerator.ParseException("Expected token: AS", errorToken);
            var asTokens = innerTokens.RemainingTokens.Skip(asIndex.Value + 1).ToList();           // everything following the AS keyword                   
            var typeText = string.Join(string.Empty, asTokens.Select(x => x.Text));
            innerTokens.RemoveLastNTokens(asTokens.Count() + 1);

            var subContext = parseContext.Subcontext(innerTokens);
            var columns = ParseColumns(null, subContext);
            if (columns.Count != 1)
                throw parseContext.ErrorGenerator.ParseException($"Expected single column element in CAST, found {columns.Count}", errorToken);

            var result = new CastColumn { Parent = parent, TryCast = tryCast };
            result.Column = columns[0];
            result.Column.Parent = result;
            result.DataType = new ColumnDataType { Parent = result, Value = typeText };

            return result;
        }

        public virtual Column ParseConvertColumn(SqlElement parent, bool tryConvert, ParseContext parseContext)
        {
            var errorToken = parseContext.TokenList.Peek();
            var innerTokens = parseContext.TokenList.TakeBracketedTokens(parseContext.ErrorGenerator);

            string typeText = innerTokens.ParseTextUntilComma(parseContext.ErrorGenerator);

            var result = new ConvertColumn { Parent = parent, TryConvert = tryConvert };
            result.DataType = new ColumnDataType { Parent = result, Value = typeText };

            var subContext = parseContext.Subcontext(innerTokens);
            var columns = ParseColumns(result, subContext);
            if (columns.Count < 1 || columns.Count > 2)
                throw parseContext.ErrorGenerator.ParseException($"Expected 1 or 2 inner columns in CONVERT column, found {columns.Count}", errorToken);

            result.Column = columns[0];
            if (columns.Count == 2)
                result.Style = columns[1];

            return result;
        }

        public virtual Column ParseParseColumn(SqlElement parent, bool tryParse, ParseContext parseContext)
        {
            var errorToken = parseContext.TokenList.Peek();
            var innerTokens = parseContext.TokenList.TakeBracketedTokens(parseContext.ErrorGenerator);

            string culture = null;
            if (innerTokens.RemainingCount >= 2)
            {
                var t2 = innerTokens.PeekEnd(1);
                if (t2.AsIdentifier != null && t2.Text.ToUpperInvariant() == "USING")
                {
                    var t1 = innerTokens.PeekEnd();
                    if (t1.AsStringLiteral == null)
                        throw parseContext.ErrorGenerator.ParseException("Expected literal text column, found " + t1.Text, errorToken);

                    culture = t1.Text.Substring(1, t1.Text.Length - 2);
                    innerTokens.RemoveLastNTokens(2);
                }
            }

            int? asIndex = innerTokens.FindLastIndex(x => x.IsKeyword(TSQLKeywords.AS));
            if (!asIndex.HasValue)
                throw parseContext.ErrorGenerator.ParseException("Expected token: AS", errorToken);

            var asTokens = innerTokens.RemainingTokens.Skip(asIndex.Value + 1).ToList();           // everything following the AS keyword                   
            var typeText = string.Join(string.Empty, asTokens.Select(x => x.Text));
            innerTokens.RemoveLastNTokens(asTokens.Count() + 1);

            var result = new ParseColumn { Parent = parent, TryParse = tryParse, Culture = culture };
            var subContext = parseContext.Subcontext(innerTokens);
            result.Column = ParseNextColumn(result, subContext);
            result.DataType = new ColumnDataType { Parent = result, Value = typeText };

            return result;
        }

        public virtual Column ParseNullColumn(SqlElement parent, ParseContext parseContext)
        {
            ParseUtilities.AssertIsKeyword(parseContext.TokenList.Take(), parseContext, TSQLKeywords.NULL);

            return new NullColumn { Parent = parent };
        }


        public virtual Column ParseNextColumn(SqlElement parent, ParseContext parseContext, bool allowAlias = true)
        {
            var tokenList = parseContext.TokenList;

            Column result = ParseNextColumnSegment(parent, parseContext);

            while (true)
            {
                var op = TryParseArithmeticOperator(parseContext);
                if (!op.HasValue)
                    break;

                if (!(result is ArithmeticChain arithChain))
                {
                    arithChain = new ArithmeticChain
                    {
                        Parent = parent,
                        LeftColumn = result,
                        Operations = new List<ArithmeticOperation>()
                    };
                    result.Parent = arithChain;
                    result = arithChain;
                }

                var arithColumn = new ArithmeticOperation { Parent = arithChain, Operator = op.Value };
                arithColumn.RightColumn = ParseNextColumnSegment(arithColumn, parseContext);
                arithChain.Operations.Add(arithColumn);
            }

            if (tokenList.Peek()?.IsKeyword(TSQLKeywords.OVER) == true)
            {
                var overColumn = new OverColumn { Parent = parent, Column = result };
                result.Parent = overColumn;
                result = overColumn;

                overColumn.Over = SelectParser.OverParser.ParseOver(overColumn, parseContext);
            }

            if (allowAlias)
            {
                string alias = ParseUtilities.TryTakeAlias(parseContext);
                if (alias != null)
                {
                    var aliasColumn = new AliasColumn
                    {
                        Parent = parent,
                        InnerColumn = result,
                        Alias = new SqlIdentifier(alias)
                    };

                    result.Parent = aliasColumn;
                    result = aliasColumn;
                }
            }

            return result;
        }

        public virtual ArithmeticOperator? TryParseArithmeticOperator(ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            if (tokenList.HasMore)
            {
                var op = tokenList.Peek().AsOperator;
                if (op != null)
                {
                    if (op.Text == "|")
                    {
                        if (tokenList.Peek(1)?.Text == "|")
                        {
                            tokenList.Advance(2);
                            return ArithmeticOperator.Concat;
                        }
                    }

                    if (ArithmeticOperatorsMap.TryGetValue(op.Text, out ArithmeticOperator result))
                    {
                        tokenList.Advance();
                        return result;
                    }
                }
            }

            return null;
        }

        public virtual Column ParseCaseColumn(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;

            ParseUtilities.AssertIsKeyword(tokenList.Take(), parseContext, TSQLKeywords.CASE);

            var result = new CaseColumn { Parent = parent, Branches = new List<CaseBranch>() };

            while (tokenList.HasMore)
            {
                if (!(tokenList.TryTakeKeywords(parseContext, TSQLKeywords.WHEN)))
                    break;

                var branch = new CaseBranch { Parent = result };
                branch.Condition = SelectParser.ConditionParser.ParseCondition(branch, parseContext);

                ParseUtilities.AssertIsKeyword(tokenList.Take(), parseContext, TSQLKeywords.THEN);

                branch.Column = ParseNextColumn(branch, parseContext);

                result.Branches.Add(branch);
            }

            if (tokenList.TryTakeKeywords(parseContext, TSQLKeywords.ELSE))
            {
                result.DefaultColumn = ParseNextColumn(result, parseContext);
            }

            ParseUtilities.AssertIsKeyword(tokenList.Take(), parseContext, TSQLKeywords.END);

            return result;
        }

        public virtual Column ParseBracketedColumn(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            var errorToken = tokenList.Peek();

            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses, parseContext);

            var innerTokens = tokenList.TakeBracketedTokens(parseContext.ErrorGenerator);

            if (!innerTokens.HasMore)
                throw parseContext.ErrorGenerator.ParseException("Empty brackets found", errorToken);

            var subContext = parseContext.Subcontext(innerTokens);
            Column result;
            if (innerTokens.Peek()?.IsKeyword(TSQLKeywords.SELECT) == true)
                result = CreateSubselectColumn(parent, subContext);
            else
                result = CreateBracketedColumn(parent, subContext); 

            return result;
        }

        public virtual Column CreateSubselectColumn(SqlElement parent, ParseContext parseContext)
        {
            var subselect = new SubselectColumn { Parent = parent };
            subselect.InnerSelect = SelectParser.ParseSelectStatement(subselect, parseContext);

            return subselect;
        }

        public virtual BracketedColumn CreateBracketedColumn(SqlElement parent, ParseContext parseContext)
        {
            var errorToken = parseContext.TokenList.Peek();

            var bracket = new BracketedColumn { Parent = parent };
            var columns = ParseColumns(bracket, parseContext);

            if (columns.Count != 1)
                throw parseContext.ErrorGenerator.ParseException($"Expected 1 bracketed column, found {columns.Count}", errorToken);

            bracket.InnerColumn = columns[0];
            return bracket;
        }

        public virtual Column ParseAggregation(SqlElement parent, string name, ParseContext parseContext)
        {
            var errorToken = parseContext.TokenList.Peek();

            var aggColumn = new AggregatedColumn { Parent = parent, Aggregation = AggregationsMap[name] };       
            var innerTokens = parseContext.TokenList.TakeBracketedTokens(parseContext.ErrorGenerator);

            if (innerTokens.TryTakeKeywords(parseContext, TSQLKeywords.DISTINCT))
            {
                aggColumn.Distinct = true;
            }

            var subContext = parseContext.Subcontext(innerTokens);
            aggColumn.InnerColumns = ParseColumns(aggColumn, subContext);

            if (!aggColumn.InnerColumns.Any())
                throw parseContext.ErrorGenerator.ParseException($"Aggregtion {aggColumn.Aggregation} with no inner columns", errorToken);

            return aggColumn;
        }

        public virtual FunctionColumn ParseFunc(SqlElement parent, ParseContext parseContext, List<string> names)
        {
            var fullName = names.Select(x => new SqlIdentifier(x)).ToArray();
            var funcColumn = new FunctionColumn { Name = fullName, Parent = parent };

            var innerTokens = parseContext.TokenList.TakeBracketedTokens(parseContext.ErrorGenerator);
            var subContext = parseContext.Subcontext(innerTokens);
            funcColumn.Parameters = ParseColumns(funcColumn, subContext);

            return funcColumn;
        }

        public virtual Column ParseIdentifier(SqlElement parent, ParseContext parseContext)
        {
            var tokenList = parseContext.TokenList;
            var errorToken = tokenList.Peek();

            var names = SelectParser.ParseMultiPartIndentifier(parseContext);
            string singleName = names.Count == 1 ? names[0].ToUpperInvariant() : null;

            if (tokenList.TryTakeCharacter(TSQLCharacters.OpenParentheses))
            {
                if (singleName == "CAST")
                    return ParseCastColumn(parent, false, parseContext);
                if (singleName == "TRY_CAST")
                    return ParseCastColumn(parent, true, parseContext);
                if (singleName == "CONVERT")
                    return ParseConvertColumn(parent, false, parseContext);
                if (singleName == "TRY_CONVERT")
                    return ParseConvertColumn(parent, true, parseContext);
                if (singleName == "PARSE")
                    return ParseParseColumn(parent, false, parseContext);
                if (singleName == "TRY_PARSE")
                    return ParseParseColumn(parent, true, parseContext);
                if (singleName == "IIF")
                    return ParseIifColumn(parent, parseContext);
                else if (AggregationsMap.ContainsKey(names[0]))
                    return ParseAggregation(parent, names[0], parseContext);
                else
                    return ParseFunc(parent, parseContext, names);
            }
            else if (names.Count == 1)
            {
                return new PrimitiveColumn { Name = new SqlIdentifier(names[0]), Parent = parent };
            }
            else if (names.Count == 2)
            {
                return new PrimitiveColumn { TableAlias = new SqlIdentifier(names[0]), Name = new SqlIdentifier(names[1]), Parent = parent };
            }

            throw parseContext.ErrorGenerator.ParseException($"Unexpected multi part column name {string.Join(".", names)}", errorToken);        
        }

        public virtual Column ParseIifColumn(SqlElement parent, ParseContext parseContext)
        {
            var errorToken = parseContext.TokenList.Peek();
            void consumeComma()
            {
                if (!parseContext.TokenList.TryTakeCharacter(TSQLCharacters.Comma))
                    throw parseContext.ErrorGenerator.ParseException("Expected comma in IIF column", errorToken);
            }

            var result = new IifColumn { Parent = parent };
            result.Condition = SelectParser.ConditionParser.ParseCondition(result, parseContext);

            consumeComma();
            result.TrueColumn = ParseNextColumn(result, parseContext);

            consumeComma();
            result.FalseColumn = ParseNextColumn(result, parseContext);

            if (!parseContext.TokenList.TryTakeCharacter(TSQLCharacters.CloseParentheses))
                throw parseContext.ErrorGenerator.ParseException("Expected closing bracket in IIF column", errorToken);

            return result;
        }

        public virtual Column ParseLiteralColumn(SqlElement parent, ParseContext parseContext)
        {
            Column result = null;

            var tokenList = parseContext.TokenList;
            var nextToken = tokenList.Peek();
            if (nextToken.AsNumericLiteral != null)
            {
                if (long.TryParse(nextToken.Text, out long l))
                    result = new IntegerColumn { Parent = parent, Value = l };
                else if (decimal.TryParse(nextToken.Text, out decimal d))
                    result = new DecimalColumn { Parent = parent, Value = d };

            }
            else if (nextToken.AsLiteral != null)
            {
                result = new StringColumn { Parent = parent, Value = nextToken.Text.Substring(1, nextToken.Text.Length-2).Replace("''", "'") };
            }

            if (result == null)
                throw parseContext.ErrorGenerator.ParseException("Expected literal column, found {nextToken.Text}", nextToken);

            tokenList.Advance();
            return result;
        }
    }
}
