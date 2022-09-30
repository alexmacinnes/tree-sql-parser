using System.Collections.Generic;
using System.Linq;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.MySql
{
    public class MySqlFunctionWriter : FunctionWriter
    {
        public MySqlFunctionWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Abs(Column expression) =>
            $"ABS({ColumnSql(expression)})";

        protected override string AddDays(Column baseDate, Column days) =>
            $"ADDDATE({ColumnSql(baseDate)}, INTERVAL ({ColumnSql(days)}) DAY)";

        protected override string AddMonths(Column baseDate, Column months) =>
            $"ADDDATE({ColumnSql(baseDate)}, INTERVAL ({ColumnSql(months)}) MONTH)";

        protected override string AddYears(Column baseDate, Column years) =>
            $"ADDDATE({ColumnSql(baseDate)}, INTERVAL ({ColumnSql(years)}) YEAR)";

        protected override string Ceiling(Column expression) =>
            $"CEIL({ColumnSql(expression)})";

        protected override string CharIndex(Column searchText, Column inputText, Column startIndex = null) =>
            startIndex == null ?
            $"INSTR({ColumnsSql(inputText, searchText)})" :
            $"IFNULL((NULLIF(INSTR(SUBSTR({ColumnsSql(inputText, startIndex)}), {ColumnSql(searchText)}), 0)) + {ColumnSql(startIndex)} - 1, 0)";

        protected override string Choose(Column index, List<Column> values) =>
            $"ELT({ColumnSql(index)}, {ColumnsSql(values.ToArray())})";

        protected override string Coalesce(IReadOnlyList<Column> expressions) =>
            $"COALESCE({ColumnsSql(expressions.ToArray())})";

        protected override string Concat(IReadOnlyList<Column> strings) =>
            $"CONCAT({ColumnsSql(strings.ToArray())})";
        protected override string ConcatWithSeperator(Column seperator, IReadOnlyList<Column> strings) =>
            $"CONCAT_WS({ColumnSql(seperator)}, {ColumnsSql(strings.ToArray())})";

        protected override string DateFromParts(Column year, Column month, Column day) =>
            $"DATE(CONCAT_WS('-', {ColumnSql(year)}, {ColumnSql(month)}, {ColumnSql(day)})))";

        protected override string Day(Column date) =>
            $"DAY({ColumnSql(date)})";

        protected override string DaysBetween(Column date1, Column date2) =>
            $"DATEDIFF({ColumnsSql(date2, date1)})";

        protected override string Exp(Column expression) =>
            $"EXP({ColumnSql(expression)})";

        protected override string Floor(Column expression) =>
            $"FLOOR({ColumnSql(expression)})";

        protected override string GetDate() =>
            $"DATE(CURRENT_DATE())";

        protected override string GetTimestamp() =>
            "TIMESTAMP(CURRENT_TIMESTAMP())";            

        protected override string IsNull(Column expression, Column value) =>
            $"ISNULL({ColumnsSql(expression, value)})";

        protected override string Left(Column inputText, Column numberOfChars) =>
            $"LEFT({ColumnsSql(inputText, numberOfChars)})";

        protected override string Len(Column inputText) =>
            $"LENGTH({ColumnSql(inputText)})";

        protected override string Log(Column expression, Column logBase) =>
            logBase == null ?
            $"LOG({ColumnSql(expression)})" :
            $"LOG({ColumnsSql(logBase, expression)})";

        protected override string Lower(Column inputText) =>
            $"LOWER({ColumnSql(inputText)})";

        protected override string LTrim(Column inputText) =>
            $"LTRIM({ColumnSql(inputText)})";

        protected override string Month(Column date) =>
            $"MONTH({ColumnSql(date)})";

        protected override string MonthsBetween(Column date1, Column date2) =>
            $"TIMESTAMPDIFF(MONTH, {ColumnSql(date1)}, {ColumnSql(date2)})";

        protected override string NullIf(Column expression1, Column expression2) =>
            $"NULLIF({ColumnsSql(expression1, expression2)})";

        protected override string Power(Column expression, Column power) =>
            $"POW({ColumnsSql(expression, power)})";

        protected override string Replace(Column inputText, Column oldSubstring, Column newSubstring) =>
            $"REPLACE({ColumnsSql(inputText, oldSubstring, newSubstring)})";

        protected override string Replicate(Column inputText, Column timesToRepeat) =>
            $"REPEAT({ColumnsSql(inputText, timesToRepeat)})";

        protected override string Reverse(Column inputText) =>
            $"REVERSE({ColumnSql(inputText)})";

        protected override string Right(Column inputText, Column numberOfChars) =>
            $"RIGHT({ColumnsSql(inputText, numberOfChars)})";

        protected override string Round(Column expression, Column precision) =>
            $"ROUND({ColumnsSql(expression, precision)})";

        protected override string RTrim(Column inputText) =>
            $"RTRIM({ColumnSql(inputText)})";

        protected override string Sign(Column expression) =>
            $"SIGN({ColumnSql(expression)})";

        protected override string Space(Column length) =>
            $"SPACE({ColumnSql(length)})";

        protected override string Substring(Column inputText, Column startIndex, Column numberOfChars) =>
            $"SUBSTR({ColumnsSql(inputText, startIndex, numberOfChars)})";

        protected override string ToNumber(Column column) =>
            $"CONVERT({ColumnSql(column)}, DECIMAL)";

        protected override string ToTimestamp(Column column) =>
            $"CONVERT({ColumnSql(column)}, DATETIME)";

        protected override string ToChar(Column column) =>
            $"CONVERT({ColumnSql(column)}, NCHAR)";

        protected override string Trim(Column inputText) =>
            $"TRIM({ColumnSql(inputText)})";

        protected override string TruncateDate(Column dateTime) =>
            $"DATE({ColumnSql(dateTime)})";

        protected override string Upper(Column inputText) =>
            $"UPPER({ColumnSql(inputText)})";

        protected override string Year(Column date) =>
            $"YEAR({ColumnSql(date)})";

        protected override string YearsBetween(Column date1, Column date2) =>
            $"TIMESTAMPDIFF(YEAR, {ColumnSql(date1)}, {ColumnSql(date2)})";
    }
}
