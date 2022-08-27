using System.Collections.Generic;
using System.Linq;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Writers.Safe.Columns.Components;

namespace TreeSqlParser.Writers.Safe.Columns.SqlServer
{
    public class SqlServerFunctionWriter : FunctionWriter
    {
        public SqlServerFunctionWriter(SafeSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Abs(Column expression) =>
            $"ABS({ColumnSql(expression)})";

        protected override string AddDays(Column baseDate, Column days) =>
            $"DATEADD(d, {ColumnSql(days)}, {ColumnSql(baseDate)})";

        protected override string AddMonths(Column baseDate, Column months) =>
            $"DATEADD(m, {ColumnSql(months)}, {ColumnSql(baseDate)})";

        protected override string AddYears(Column baseDate, Column years) =>
            $"DATEADD(y, {ColumnSql(years)}, {ColumnSql(baseDate)})";

        protected override string Ceiling(Column expression) =>
            $"CEILING({ColumnSql(expression)})";

        protected override string CharIndex(Column searchText, Column inputText, Column startIndex = null) =>
            startIndex == null ?
            $"CHARINDEX({ColumnsSql(searchText, inputText)})" :
            $"CHARINDEX({ColumnsSql(searchText, inputText, startIndex)})";

        protected override string Choose(Column index, List<Column> values) =>
            $"CHOOSE({ColumnSql(index)}, {ColumnsSql(values.ToArray())})";

        protected override string Coalesce(IReadOnlyList<Column> expressions) =>
            $"COALESCE({ColumnsSql(expressions.ToArray())})";

        protected override string Concat(IReadOnlyList<Column> strings) =>
            $"CONCAT({ColumnsSql(strings.ToArray())})";

        protected override string ConcatWithSeperator(Column seperator, IReadOnlyList<Column> strings) =>
            $"CONCAT_WS({ColumnSql(seperator)}, {ColumnsSql(strings.ToArray())})";

        protected override string DateFromParts(Column year, Column month, Column day) =>
            $"DATEFROMPARTS({ColumnsSql(year, month, day)})";

        protected override string Day(Column date) =>
            $"DAY({ColumnSql(date)})";

        protected override string DaysBetween(Column date1, Column date2) =>
            $"DATEDIFF(d, {ColumnSql(date1)}, {ColumnSql(date2)})";

        protected override string Exp(Column expression) =>
            $"EXP({ColumnSql(expression)})";

        protected override string Floor(Column expression) =>
            $"FLOOR({ColumnSql(expression)})";

        protected override string GetDate() =>
            $"CONVERT(date, {GetTimestamp()})";

        protected override string GetTimestamp() =>
            "GETDATE()";            

        protected override string IsNull(Column expression, Column value) =>
            $"ISNULL({ColumnsSql(expression, value)})";

        protected override string Left(Column inputText, Column numberOfChars) =>
            $"LEFT({ColumnsSql(inputText, numberOfChars)})";

        protected override string Len(Column inputText) =>
            $"LEN({ColumnSql(inputText)})";

        protected override string Log(Column expression, Column logBase) =>
            logBase == null ?
            $"LOG({ColumnSql(expression)})" :
            $"LOG({ColumnsSql(expression, logBase)})";

        protected override string Lower(Column inputText) =>
            $"LOWER({ColumnSql(inputText)})";

        protected override string LTrim(Column inputText) =>
            $"LTRIM({ColumnSql(inputText)})";

        protected override string Month(Column date) =>
            $"MONTH({ColumnSql(date)})";

        protected override string MonthsBetween(Column date1, Column date2) =>
            $"DATEDIFF(m, {ColumnSql(date1)}, {ColumnSql(date2)})";

        protected override string NullIf(Column expression1, Column expression2) =>
            $"NULLIF({ColumnsSql(expression1, expression2)})";

        protected override string Power(Column expression, Column power) =>
            $"POWER({ColumnsSql(expression, power)})";

        protected override string Replace(Column inputText, Column oldSubstring, Column newSubstring) =>
            $"REPLACE({ColumnsSql(inputText, oldSubstring, newSubstring)})";

        protected override string Replicate(Column inputText, Column timesToRepeat) =>
            $"REPLICATE({ColumnsSql(inputText, timesToRepeat)})";

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
            $"SUBSTRING({ColumnsSql(inputText, startIndex, numberOfChars)})";

        protected override string ToNumber(Column column) =>
            $"CONVERT(real, {ColumnSql(column)})";

        protected override string ToTimestamp(Column column) =>
            $"CONVERT(datetime, {ColumnSql(column)})";

        protected override string ToChar(Column column) =>
            $"STR({ColumnSql(column)})";

        protected override string Trim(Column inputText) =>
            $"TRIM({ColumnSql(inputText)})";

        protected override string TruncateDate(Column dateTime) =>
            $"CONVERT(date, {ColumnSql(dateTime)})";

        protected override string Upper(Column inputText) =>
            $"UPPER({ColumnSql(inputText)})";

        protected override string Year(Column date) =>
            $"YEAR({ColumnSql(date)})";

        protected override string YearsBetween(Column date1, Column date2) =>
            $"DATEDIFF(y, {ColumnSql(date1)}, {ColumnSql(date2)})";
    }
}
