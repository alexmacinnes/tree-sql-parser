using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using NUnit.Framework.Api;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    public class SingleColumnTests
    {
        [Test]
        public void ParseInteger() => SelectParserMother.TestParse("SELECT 1");

        [Test]
        public void ParseNegativeInteger() => SelectParserMother.TestParse("SELECT -1");

        [Test]
        public void ParseDecimal() => SelectParserMother.TestParse("SELECT 1.2");

        [Test]
        public void ParseNegativeDecimal() => SelectParserMother.TestParse("SELECT -1.2");

        [Test]
        public void ParseString() => SelectParserMother.TestParse("SELECT ''");

        [Test]
        public void ParseEmptyString() => SelectParserMother.TestParse("SELECT 'foo'");

        [Test]
        public void ParseDate() => SelectParserMother.TestParse("SELECT {d '2000-12-31'}");

        [Test]
        public void ParseDateTime() => SelectParserMother.TestParse("SELECT {ts '2000-12-31 23:59:59'}");

        [Test]
        public void ParseDateTimeNanoseconds() => SelectParserMother.TestParse("SELECT {ts '2000-12-31 23:59:59.1234567'}");

        [Test]
        public void ParseTime() => SelectParserMother.TestParse("SELECT {t '23:59:59'}");

        [Test]
        public void ParseTimeNanoseconds() => SelectParserMother.TestParse("SELECT {t '23:59:59.1234567'}");

        [Test]
        public void ParseTrue() => SelectParserMother.TestParse("SELECT tRUe", "SELECT 1");

        [Test]
        public void ParseFalse() => SelectParserMother.TestParse("SELECT fALSe", "SELECT 0");

        [Test]
        public void ParseTrueAsIdentifier() => SelectParserMother.TestParse("SELECT [true]", "SELECT [true]");

        [Test]
        public void ParseStar() => SelectParserMother.TestParse("SELECT *");

        [Test]
        public void ParseTableStar() => SelectParserMother.TestParse("SELECT tbl.*");

        [Test]
        public void ParseAlias() => SelectParserMother.TestParse("SELECT 1 AS foo");

        [Test]
        public void ParseAliasWithoutAs() => SelectParserMother.TestParse("SELECT 1 foo", "SELECT 1 AS foo");

        [Test]
        public void ParseSinglePartColumn() => SelectParserMother.TestParse("SELECT x");

        [Test]
        public void ParseTwoPartColumn() => SelectParserMother.TestParse("SELECT x.y");

        [Test]
        public void ParseThreePartColumnThrows() => SelectParserMother.TestParseThrows("SELECT x.y.z", "Unexpected multi part column name x.y.z");

        [Test]
        public void ParseFunc0Args() => SelectParserMother.TestParse("SELECT fooFunc()");

        [Test]
        public void ParseFunc1Arg() => SelectParserMother.TestParse("SELECT fooFunc(1)");

        [Test]
        public void ParseFunc2Args() => SelectParserMother.TestParse("SELECT fooFunc(1, 'abc')");

        [Test]
        public void ParseMultipartFunction() => SelectParserMother.TestParse("SELECT [x y].z.fooFunc()");

        [Test]
        public void ParseFuncNested() => SelectParserMother.TestParse("SELECT func1(1, func2(2, 3))");

        [Test]
        public void ParseSimpleAggregations() => SelectParserMother.TestParse("SELECT SUM(1), MIN(2), MAX(3), AVG(4), COUNT(5)");

        [Test]
        public void ParseCountDistinct() => SelectParserMother.TestParse("SELECT COUNT(DISTINCT 1)");

        [Test]
        public void ParseCountAll() => SelectParserMother.TestParse("SELECT COUNT(*)");

        [Test]
        public void ParseCountTableAll() => SelectParserMother.TestParse("SELECT COUNT(tbl.*)");

        [Test]
        public void ParseAggregationWithNesting() => SelectParserMother.TestParse("SELECT SUM(foo(1))");

        [Test]
        public void ParseSum0ArgsThrows() => SelectParserMother.TestParseThrows("SELECT SUM()", "Aggregtion Sum with no inner columns");

        [Test]
        public void ParseSum2Args() => SelectParserMother.TestParse("SELECT SUM(1, 2)");      // small number of aggregations must accept multiple args

        [Test]
        public void ParseBracketedColumn() => SelectParserMother.TestParse("SELECT (1)");

        [Test]
        public void ParseNestedBracketedColumn() => SelectParserMother.TestParse("SELECT ((Foo((1), (2))))");

        [Test]
        public void ParseSubselectColumn() => SelectParserMother.TestParse("SELECT (SELECT 1)");

        [Test]
        public void ParseMultipartAlias() => SelectParserMother.TestParse("SELECT 1 AS [a b]");

        [Test]
        public void ParseMultipartColumn() => SelectParserMother.TestParse("SELECT [a b].[c d]");

        [Test]
        public void ParseIif() => SelectParserMother.TestParse("SELECT IIF(1=2, 3, 4)");

        [Test]
        public void ParseSingleBranchCase() => SelectParserMother.TestParse("SELECT CASE WHEN 1=2 THEN 3 END");

        [Test]
        public void ParseSingleBranchCaseWithElse() => SelectParserMother.TestParse("SELECT CASE WHEN 1=2 THEN 3 ELSE 4 END");

        [Test]
        public void ParseMultiBranchCase() => SelectParserMother.TestParse("SELECT CASE WHEN 1=2 THEN 3 WHEN 4=5 THEN 6 WHEN 7=8 THEN 9 END");

        [Test]
        public void ParseMultiBranchCaseWithElse() => SelectParserMother.TestParse("SELECT CASE WHEN 1=2 THEN 3 WHEN 4=5 THEN 6 WHEN 7=8 THEN 9 ELSE 0 END");

        [Test]
        public void ParsePlusColumn() => SelectParserMother.TestParse("SELECT 1+2");

        [Test]
        public void ParseMinusColumn() => SelectParserMother.TestParse("SELECT 1-2");

        [Test]
        public void ParseMultiplyColumn() => SelectParserMother.TestParse("SELECT 1*2");

        [Test]
        public void ParseDivideColumn() => SelectParserMother.TestParse("SELECT 1/2");

        [Test]
        public void ParseModuloColumn() => SelectParserMother.TestParse("SELECT 1%2");

        [Test]
        public void ParseConcatColumn() => SelectParserMother.TestParse("SELECT 'a'||'b'");

        [Test]
        public void ParseChainedPlusColumns() => SelectParserMother.TestParse("SELECT 1+2+3");

        [Test]
        public void ParseChainedAritmeticWithNestedBrackets() => SelectParserMother.TestParse("SELECT 1+(2*3)-4");

        [Test]
        public void ParseNullColumn() => SelectParserMother.TestParse("SELECT NULL");

        [Test]
        public void ParseCastColumn() => SelectParserMother.TestParse("SELECT CAST(NULL AS foo)");

        [Test]
        public void ParseCastComplexTypeColumn() => SelectParserMother.TestParse("SELECT CAST(NULL AS foo(1,2))");

        [Test]
        public void ParseTryCastComplexTypeColumn() => SelectParserMother.TestParse("SELECT TRY_CAST(NULL AS foo)");

        [Test]
        public void ParseConvertColumn() => SelectParserMother.TestParse("SELECT CONVERT(foo, 1)");

        [Test]
        public void ParseConvertComplexTypeColumn() => SelectParserMother.TestParse("SELECT CONVERT(foo(1,2), 1, 101)");

        [Test]
        public void ParseTryConvertComplexTypeColumn() => SelectParserMother.TestParse("SELECT TRY_CONVERT(foo(1,2), 1, 101)");

        [Test]
        public void ParseParseColumn() => SelectParserMother.TestParse("SELECT PARSE('1' AS foo)");

        [Test]
        public void ParseParseComplexTypeColumn() => SelectParserMother.TestParse("SELECT PARSE('1' AS foo(1,2) USING 'en-US')");

        [Test]
        public void ParseTryParseComplexTypeColumn() => SelectParserMother.TestParse("SELECT TRY_PARSE('1' AS foo(1,2) USING 'en-US')");

        [Test]
        public void ParseVariableColumn() => SelectParserMother.TestParse("SELECT @foo");

        [Test]
        public void ParseLeftFunctionColumn() => SelectParserMother.TestParse("SELECT LEFT('abc', 2)");

        [Test]
        public void ParseRightFunctionColumn() => SelectParserMother.TestParse("SELECT RIGHT('abc', 2)");

        [Test]
        public void ParseReplicateFunctionColumn() => SelectParserMother.TestParse("SELECT REPLICATE('abc', 5)");

        [Test]
        public void ParseAbsFunctionColumn() => SelectParserMother.TestParse("SELECT ABS(-1)");
    }
}
