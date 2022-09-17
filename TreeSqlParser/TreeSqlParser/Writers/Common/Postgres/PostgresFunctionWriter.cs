using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Postgres
{
    public class PostgresFunctionWriter : FunctionWriter
    {
        public PostgresFunctionWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Abs(Column expression) =>
            $"ABS({ColumnSql(expression)})";

        protected override string AddDays(Column baseDate, Column days) =>
            $"{ColumnSql(baseDate)} + (INTERVAL '1d' * {ColumnSql(days)})";

        protected override string AddMonths(Column baseDate, Column months) =>
            $"{ColumnSql(baseDate)} + (INTERVAL '1 month' * {ColumnSql(months)})";

        protected override string AddYears(Column baseDate, Column years) =>
            $"{ColumnSql(baseDate)} + (INTERVAL '1y' * {ColumnSql(years)})";

        protected override string Ceiling(Column expression) =>
            $"CEILING({ColumnSql(expression)})";

        protected override string CharIndex(Column searchText, Column inputText, Column startIndex = null) =>
            startIndex == null ?
            $"STRPOS({ColumnsSql(inputText, searchText)})" :
            $"COALESCE((NULLIF(STRPOS(SUBSTR({ColumnsSql(inputText, startIndex)}), {ColumnSql(searchText)}), 0)) + {ColumnSql(startIndex)} - 1, 0)";

        protected override string Choose(Column index, List<Column> values)
        {
            string indexSql = ColumnSql(index);

            var sb = new StringBuilder("CASE ");
            for (int i = 1; i <= values.Count; i++)
                sb.Append($" WHEN {indexSql} = {i} THEN {ColumnSql(values[i - 1])}");

            sb.Append(" END");

            return sb.ToString();
        }

        protected override string Coalesce(IReadOnlyList<Column> expressions) =>
            $"COALESCE({ColumnsSql(expressions.ToArray())})";

        protected override string Concat(IReadOnlyList<Column> strings) =>
            string.Join(" || ", strings.Select(ColumnSql));

        protected override string ConcatWithSeperator(Column seperator, IReadOnlyList<Column> strings) =>
            string.Join($" || {ColumnSql(seperator)} || ", strings.Select(ColumnSql));

        protected override string DateFromParts(Column year, Column month, Column day) =>
            $"MAKE_DATE({ColumnsSql(year, month, day)})";

        protected override string Day(Column date) =>
            $"EXTRACT(DAY FROM {ColumnSql(date)})";

        protected override string DaysBetween(Column date1, Column date2) =>
            $"(({ColumnSql(date2)})::date - ({ColumnSql(date1)})::date)";

        protected override string Exp(Column expression) =>
            $"EXP({ColumnSql(expression)})";

        protected override string Floor(Column expression) =>
            $"FLOOR({ColumnSql(expression)})";

        protected override string GetDate() =>
            $"CURRENT_DATE";

        protected override string GetTimestamp() =>
            "CURRENT_TIMESTAMP";            

        protected override string IsNull(Column expression, Column value) =>
            $"COALESCE({ColumnsSql(expression, value)})";

        protected override string Left(Column inputText, Column numberOfChars) =>
            $"LEFT({ColumnsSql(inputText, numberOfChars)})";

        protected override string Len(Column inputText) =>
            $"LENGTH({ColumnSql(inputText)})";

        protected override string Log(Column expression, Column logBase) =>
            logBase == null ?
            $"LOG({ColumnSql(expression)})" :
            $"LOG({ColumnsSql(expression, logBase)})";

        protected override string Lower(Column inputText) =>
            $"LOWER({ColumnSql(inputText)})";

        protected override string LTrim(Column inputText) =>
            $"LTRIM({ColumnSql(inputText)})";

        protected override string Month(Column date) =>
            $"EXTRACT(MONTH FROM {ColumnSql(date)})";

        protected override string MonthsBetween(Column date1, Column date2) =>
            $"(12*{YearsBetween(date1, date2)}) + ({Month(date2)} - {Month(date1)})";

        protected override string NullIf(Column expression1, Column expression2) =>
            $"NULLIF({ColumnsSql(expression1, expression2)})";

        protected override string Power(Column expression, Column power) =>
            $"POWER({ColumnsSql(expression, power)})";

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
            $"({ColumnSql(column)})::decimal";

        protected override string ToTimestamp(Column column) =>
            $"({ColumnSql(column)})::timestamp";

        protected override string ToChar(Column column) =>
            $"({ColumnSql(column)})::varchar";

        protected override string Trim(Column inputText) =>
            $"TRIM({ColumnSql(inputText)})";

        protected override string TruncateDate(Column dateTime) =>
            $"DATE({ColumnSql(dateTime)})";

        protected override string Upper(Column inputText) =>
            $"UPPER({ColumnSql(inputText)})";

        protected override string Year(Column date) =>
            $"EXTRACT(YEAR FROM {ColumnSql(date)})";

        protected override string YearsBetween(Column date1, Column date2) =>
            $"({Year(date2)} - {Year(date1)})";
    }
}
