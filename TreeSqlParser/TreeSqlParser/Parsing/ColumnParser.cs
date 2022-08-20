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


namespace TreeSqlParser.Parsing
{
    internal class ColumnParser
    {
        private static readonly IReadOnlyDictionary<string, Aggregation> AggregationsMap = EnumUtilities.ToDictionary<Aggregation>();

        private static readonly IReadOnlyDictionary<string, ArithmeticOperator> ArithmeticOperatorsMap = EnumUtilities.ToDictionary<ArithmeticOperator, EnumParseTextAttribute>();

        //internal static List<Column> ParseColumns(SqlElement parent, TokenList tokenList)
        //{
        //    return ParseColumns(parent, tokens, ref nextIndex);
        //}

        internal static List<Column> ParseColumns(SqlElement parent, TokenList tokenList)
        {
            var result = new List<Column>();

            if (tokenList.HasMore)
            {
                while (true)
                {
                    var column = ParseNextColumn(parent, tokenList);
                    result.Add(column);

                    if (!tokenList.TryTakeCharacter(TSQLCharacters.Comma))
                        break;
                }
            }

            return result;
        }

        public static Column ParseNextColumnSegment(SqlElement parent, TokenList tokenList)
        {
            var nextToken = tokenList.Peek();            
            string nextTokenText = nextToken.Text.ToUpperInvariant();

            if (nextToken is TSQLIdentifier && nextToken.Text.StartsWith("{"))
                return ParseDateTimeEscapeSequence(parent, tokenList);
            if (nextToken is TSQLOperator && nextTokenText == "-" && tokenList.Peek(1) is TSQLNumericLiteral)
                return ParseNegativeNumericLiteral(parent, tokenList);
            if (nextToken.AsLiteral != null)
                return ParseLiteralColumn(parent, tokenList);
            if (nextToken.AsVariable != null)
                return ParseVariableColumn(parent, tokenList);
            else if (nextToken.IsKeyword(TSQLKeywords.LEFT) || nextToken.IsKeyword(TSQLKeywords.RIGHT) || nextTokenText == "REPLICATE" || nextTokenText == "ABS")
                return ParseKeywordAsFunc(parent, tokenList);
            else if (nextToken.AsIdentifier != null)
                return ParseIdentifier(parent, tokenList);
            else if (nextToken.IsCharacter(TSQLCharacters.OpenParentheses))
                return ParseBracketedColumn(parent, tokenList);
            else if (nextToken.IsKeyword(TSQLKeywords.CASE))
                return ParseCaseColumn(parent, tokenList);
            else if (nextToken.IsKeyword(TSQLKeywords.NULL))
                return ParseNullColumn(parent, tokenList);             
            else if (nextToken.AsOperator != null && nextToken.Text == "*")
            {
                tokenList.Advance();
                return new StarColumn() { Parent = parent };
            }
            else
                throw new InvalidOperationException("Unkown column type");
        }

        private static Column ParseDateTimeEscapeSequence(SqlElement parent, TokenList tokenList)
        {
            string dateTimeType;
            
            string firstText = tokenList.Take().Text;
            if (firstText.Length > 1)
                dateTimeType = firstText.Substring(1);
            else
                dateTimeType = tokenList.Take().Text;

            Column result;
            switch (dateTimeType)
            {
                case "d":
                    result = new DateColumn { Parent = parent, Value = DateTime.Parse(Unquote(tokenList.Take().Text)) };
                    break;
                case "ts":
                    result = new DateTimeColumn { Parent = parent, Value = DateTime.Parse(Unquote(tokenList.Take().Text)) };
                    break;
                case "t":
                    result = new TimeColumn { Parent = parent, Value = TimeSpan.Parse(Unquote(tokenList.Take().Text)) };
                    break;
                default:
                    throw new InvalidOperationException("Unknown datetime literal type: " + dateTimeType);
            }

            if (tokenList.Take().Text != "}")
                throw new InvalidOperationException("Expected closing brace: }");

            return result;
        }

        private static string Unquote(string text) =>
            text.Length > 1 && text.StartsWith("'") && text.EndsWith("'") ?
            text.Substring(1, text.Length-2) :
            throw new InvalidOperationException("Expected quoted text");


        private static Column ParseNegativeNumericLiteral(SqlElement parent, TokenList tokenList)
        {
            string neg = tokenList.Take().Text;
            if (neg != "-")
                throw new InvalidOperationException("Expected negative operator: -");

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

        private static Column ParseKeywordAsFunc(SqlElement parent, TokenList tokenList)
        {
            var fullName = new[] { new SqlIdentifier() { Name = tokenList.Take().Text } };
            if (!tokenList.TryTakeCharacter(TSQLCharacters.OpenParentheses))
                throw new NotSupportedException("Expected open parenthesis after: " + fullName[0].Name);

            var funcColumn = new FunctionColumn { Name = fullName, Parent = parent };
            funcColumn.Parameters = ParseColumns(funcColumn, tokenList.TakeBracketedTokens());

            return funcColumn;
        }

        private static VariableColumn ParseVariableColumn(SqlElement parent, TokenList tokenList)
        {
            return new VariableColumn
            {
                Parent = parent,
                VariableName = tokenList.Take().AsVariable.Text.Substring(1)
            };
        }

        private static Column ParseCastColumn(SqlElement parent, bool tryCast, TokenList tokenList)
        {
            var innerTokens = tokenList.TakeBracketedTokens();

            int? asIndex = innerTokens.FindLastIndex(x => x.IsKeyword(TSQLKeywords.AS));
            if (!asIndex.HasValue)
                throw new InvalidOperationException("Expected token: AS");
            var asTokens = innerTokens.RemainingTokens.Skip(asIndex.Value + 1).ToList();           // everything following the AS keyword                   
            var typeText = string.Join(string.Empty, asTokens.Select(x => x.Text));
            innerTokens.RemoveLastNTokens(asTokens.Count() + 1);

            var columns = ParseColumns(null, innerTokens);
            if (columns.Count != 1)
                throw new InvalidOperationException($"Expected single column element in CAST, found {columns.Count}");

            var result = new CastColumn { Parent = parent, TryCast = tryCast };
            result.Column = columns[0];
            result.Column.Parent = result;
            result.DataType = new ColumnDataType { Parent = result, Value = typeText };

            return result;
        }

        private static Column ParseConvertColumn(SqlElement parent, bool tryConvert, TokenList tokenList)
        {
            var innerTokens = tokenList.TakeBracketedTokens();

            string typeText = innerTokens.ParseTextUntilComma();

            var result = new ConvertColumn { Parent = parent, TryConvert = tryConvert };
            result.DataType = new ColumnDataType { Parent = result, Value = typeText };

            var columns = ParseColumns(result, innerTokens);
            if (columns.Count < 1 || columns.Count > 2)
                throw new InvalidOperationException($"Expected 1 or 2 inner columns in CONVERT column, found {columns.Count}");

            result.Column = columns[0];
            if (columns.Count == 2)
                result.Style = columns[1];

            return result;
        }

        private static Column ParseParseColumn(SqlElement parent, bool tryParse, TokenList tokenList)
        {
            var innerTokens = tokenList.TakeBracketedTokens();

            string culture = null;
            if (innerTokens.RemainingCount >= 2)
            {
                var t2 = innerTokens.PeekEnd(1);
                if (t2.AsIdentifier != null && t2.Text.ToUpperInvariant() == "USING")
                {
                    var t1 = innerTokens.PeekEnd();
                    if (t1.AsStringLiteral == null)
                        throw new NotSupportedException("Expected literal text column, found " + t1.Text);

                    culture = t1.Text.Substring(1, t1.Text.Length - 2);
                    innerTokens.RemoveLastNTokens(2);
                }
            }

            int? asIndex = innerTokens.FindLastIndex(x => x.IsKeyword(TSQLKeywords.AS));
            if (!asIndex.HasValue)
                throw new InvalidOperationException("Expected token: AS");
            var asTokens = innerTokens.RemainingTokens.Skip(asIndex.Value + 1).ToList();           // everything following the AS keyword                   
            var typeText = string.Join(string.Empty, asTokens.Select(x => x.Text));
            innerTokens.RemoveLastNTokens(asTokens.Count() + 1);

            var result = new ParseColumn { Parent = parent, TryParse = tryParse, Culture = culture };
            result.Column = ParseNextColumn(result, innerTokens);
            result.DataType = new ColumnDataType { Parent = result, Value = typeText };

            return result;
        }

        private static Column ParseNullColumn(SqlElement parent, TokenList tokenList)
        {
            ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.NULL);

            return new NullColumn { Parent = parent };
        }

        //private static Column ParseNextColumn(SqlElement parent, List<TSQLToken> tokens)
        //{
        //    int nextIndex = 0;
        //    return ParseNextColumn(parent, tokens, ref nextIndex);
        //}
        internal static Column ParseNextColumn(SqlElement parent, TokenList tokenList, bool allowAlias = true)
        {
            Column result = ParseNextColumnSegment(parent, tokenList);

            while (true)
            {
                var op = TryParseArithmeticOperator(tokenList);
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
                arithColumn.RightColumn = ParseNextColumnSegment(arithColumn, tokenList);
                arithChain.Operations.Add(arithColumn);
            }

            if (tokenList.Peek()?.IsKeyword(TSQLKeywords.OVER) == true)
            {
                var overColumn = new OverColumn { Parent = parent, Column = result };
                result.Parent = overColumn;
                result = overColumn;

                overColumn.Over = OverParser.ParseOver(overColumn, tokenList);
            }

            if (allowAlias)
            {
                string alias = ParseUtilities.TryTakeAlias(tokenList);
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

        private static ArithmeticOperator? TryParseArithmeticOperator(TokenList tokenList)
        {
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

        private static Column ParseCaseColumn(SqlElement parent, TokenList tokenList)
        {
            ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.CASE);

            var result = new CaseColumn { Parent = parent, Branches = new List<CaseBranch>() };

            while (tokenList.HasMore)
            {
                if (!(tokenList.TryTakeKeywords(TSQLKeywords.WHEN)))
                    break;

                var branch = new CaseBranch { Parent = result };
                branch.Condition = ConditionParser.ParseCondition(branch, tokenList);

                ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.THEN);

                branch.Column = ParseNextColumn(branch, tokenList);

                result.Branches.Add(branch);
            }

            if (tokenList.TryTakeKeywords(TSQLKeywords.ELSE))
            {
                result.DefaultColumn = ParseNextColumn(result, tokenList);
            }

            ParseUtilities.AssertIsKeyword(tokenList.Take(), TSQLKeywords.END);

            return result;
        }

        private static Column ParseBracketedColumn(SqlElement parent, TokenList tokenList)
        {
            ParseUtilities.AssertIsChar(tokenList.Take(), TSQLCharacters.OpenParentheses);

            var innerTokens = tokenList.TakeBracketedTokens();

            if (!innerTokens.HasMore)
                throw new InvalidOperationException("Empty brackets found");

            Column result;
            if (innerTokens.Peek()?.IsKeyword(TSQLKeywords.SELECT) == true)
                result = CreateSubselectColumn(parent, innerTokens);
            else
                result = CreateBracketedColumn(parent, innerTokens); 

            return result;
        }

        private static Column CreateSubselectColumn(SqlElement parent, TokenList tokenList)
        {
            var subselect = new SubselectColumn { Parent = parent };
            subselect.InnerSelect = SelectParser.ParseSelectStatement(subselect, tokenList);

            return subselect;
        }

        private static BracketedColumn CreateBracketedColumn(SqlElement parent, TokenList tokenList)
        {
            var bracket = new BracketedColumn { Parent = parent };
            var columns = ParseColumns(bracket, tokenList);

            if (columns.Count != 1)
                throw new InvalidOperationException($"Expected 1 bracketed column, found {columns.Count}");

            bracket.InnerColumn = columns[0];
            return bracket;
        }

        private static Column ParseAggregation(SqlElement parent, string name, TokenList tokenList)
        {
            var aggColumn = new AggregatedColumn { Parent = parent, Aggregation = AggregationsMap[name] };
            var innerTokens = tokenList.TakeBracketedTokens();

            if (innerTokens.TryTakeKeywords(TSQLKeywords.DISTINCT))
            {
                aggColumn.Distinct = true;
            }

            aggColumn.InnerColumns = ParseColumns(aggColumn, innerTokens);

            if (!aggColumn.InnerColumns.Any())           
                throw new InvalidOperationException($"Aggregtion {aggColumn.Aggregation} with no inner columns");

            return aggColumn;
        }

        private static FunctionColumn ParseFunc(SqlElement parent, TokenList tokenList, List<string> names)
        {
            var fullName = names.Select(x => new SqlIdentifier(x)).ToArray();
            var funcColumn = new FunctionColumn { Name = fullName, Parent = parent };
            funcColumn.Parameters = ParseColumns(funcColumn, tokenList.TakeBracketedTokens());
            return funcColumn;
        }

        private static Column ParseIdentifier(SqlElement parent, TokenList tokenList)
        {
            var names = SelectParser.ParseMultiPartIndentifier(tokenList);
            string singleName = names.Count == 1 ? names[0].ToUpperInvariant() : null;

            if (tokenList.TryTakeCharacter(TSQLCharacters.OpenParentheses))
            {
                if (singleName == "CAST")
                    return ParseCastColumn(parent, false, tokenList);
                if (singleName == "TRY_CAST")
                    return ParseCastColumn(parent, true, tokenList);
                if (singleName == "CONVERT")
                    return ParseConvertColumn(parent, false, tokenList);
                if (singleName == "TRY_CONVERT")
                    return ParseConvertColumn(parent, true, tokenList);
                if (singleName == "PARSE")
                    return ParseParseColumn(parent, false, tokenList);
                if (singleName == "TRY_PARSE")
                    return ParseParseColumn(parent, true, tokenList);
                if (singleName == "IIF")
                    return ParseIifColumn(parent, tokenList);
                else if (AggregationsMap.ContainsKey(names[0]))
                    return ParseAggregation(parent, names[0], tokenList);
                else
                    return ParseFunc(parent, tokenList, names);
            }
            else if (names.Count == 1)
            {
                return new PrimitiveColumn { Name = new SqlIdentifier(names[0]), Parent = parent };
            }
            else if (names.Count == 2)
            {
                return new PrimitiveColumn { TableAlias = new SqlIdentifier(names[0]), Name = new SqlIdentifier(names[1]), Parent = parent };
            }

            throw new InvalidOperationException($"Unexpected multi part column name {string.Join(".", names)}");
        }

        private static Column ParseIifColumn(SqlElement parent, TokenList tokenList)
        {
            void consumeComma()
            {
                if (!tokenList.TryTakeCharacter(TSQLCharacters.Comma))
                    throw new InvalidOperationException("Expected comma in IIF column");
            }

            var result = new IifColumn { Parent = parent };
            result.Condition = ConditionParser.ParseCondition(result, tokenList);

            consumeComma();
            result.TrueColumn = ParseNextColumn(result, tokenList);

            consumeComma();
            result.FalseColumn = ParseNextColumn(result, tokenList);

            if (!tokenList.TryTakeCharacter(TSQLCharacters.CloseParentheses))
                throw new InvalidOperationException("Expected closing bracket in IIF column");

            return result;
        }

        private static Column ParseLiteralColumn(SqlElement parent, TokenList tokenList)
        {
            Column result = null;

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
                throw new InvalidOperationException($"Expected literal column, found {nextToken.Text}");

            tokenList.Advance();
            return result;
        }
    }
}
