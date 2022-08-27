using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Writers.Safe.Columns.Components;

namespace TreeSqlParser.Writers.Safe.Columns.Oracle
{
    public class OracleFunctionWriter : FunctionWriter
    {
        public OracleFunctionWriter(SafeSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Abs(Column expression) =>
            $"ABS({ColumnSql(expression)})";

        protected override string AddDays(Column baseDate, Column days) =>
            $"({ColumnSql(baseDate)} + {ColumnSql(days)})";

        protected override string AddMonths(Column baseDate, Column months) =>
            $"ADD_MONTHS({ColumnsSql(baseDate, months)})";

        protected override string AddYears(Column baseDate, Column years) =>
            $"ADD_MONTHS({ColumnSql(baseDate)}, 12 * ({ColumnSql(years)})";

        protected override string Ceiling(Column expression) =>
            $"CEIL({ColumnSql(expression)})";

        protected override string CharIndex(Column searchText, Column inputText, Column startIndex = null) =>
            startIndex == null ?
            $"INSTR({ColumnsSql(inputText, searchText)})" :
            $"INSTR({ColumnsSql(inputText, searchText, startIndex)})";

        protected override string Choose(Column index, List<Column> values)
        {
            var sb = new StringBuilder();
            string indexSql = ColumnSql(index);

            sb.Append("CASE");
            for (int i = 0; i < values.Count; i++)
            {
                sb.Append($" WHEN {indexSql} = {i + 1} THEN {ColumnSql(values[i])}");
            }
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
            $"TO_DATE(({ColumnSql(year)}) || ({ColumnSql(month)}) || ({ColumnSql(day)}), 'yyyyMMdd')";

        protected override string Day(Column date) =>
            $"EXTRACT(day FROM {ColumnSql(date)})";

        protected override string DaysBetween(Column date1, Column date2) =>
            $"({ColumnSql(date2)} - {ColumnSql(date1)})";

        protected override string Exp(Column expression) =>
            $"EXP({ColumnSql(expression)})";

        protected override string GetDate() =>
            "CURRENT_DATE";

        protected override string GetTimestamp() =>
            "CURRENT_TIMESTAMP";

        protected override string IsNull(Column expression, Column value) =>
            $"NVL({ColumnsSql(expression, value)})";

        protected override string Left(Column inputText, Column numberOfChars) =>
            $"SUBSTR({ColumnSql(inputText)}, 1, {ColumnSql(numberOfChars)})";

        protected override string Len(Column inputText) =>
            $"LENGTH(RTRIM({ColumnSql(inputText)}))";

        protected override string Lower(Column inputText) =>
            $"LOWER({ColumnSql(inputText)})";

        protected override string LTrim(Column inputText) =>
            $"LTRIM({ColumnSql(inputText)})";

        protected override string NullIf(Column expression1, Column expression2) =>
            $"NULLIF({ColumnsSql(expression1, expression2)})";

        protected override string Replace(Column inputText, Column oldSubstring, Column newSubstring) =>
            $"REPLACE({ColumnsSql(inputText, oldSubstring, newSubstring)})";

        protected override string Replicate(Column inputText, Column timesToRepeat)
        {
            string inputColumnSql = ColumnSql(inputText);
            return $"RPAD('', LENGTH(RTRIM({inputColumnSql})) * {ColumnSql(timesToRepeat)}, {inputColumnSql})";
        }

        protected override string Reverse(Column inputText) =>
            $"REVERSE({ColumnSql(inputText)})";

        protected override string Right(Column inputText, Column numberOfChars)
        {
            string numberOfCharsSql = ColumnSql(numberOfChars);
            string startIndexSql = numberOfChars is IntegerColumn ? "-" + numberOfCharsSql : $"0 - ({numberOfCharsSql})";
            return $"SUBSTR({ColumnSql(inputText)}, {startIndexSql}, {numberOfCharsSql})";
        }

        protected override string RTrim(Column inputText) =>
            $"RTRIM({ColumnSql(inputText)})";

        protected override string Space(Column length) =>
            $"RPAD('', {ColumnSql(length)}, ' ')";

        protected override string Substring(Column inputText, Column startIndex, Column numberOfChars) =>
            $"SUBSTR({ColumnsSql(inputText, startIndex, numberOfChars)})";

        protected override string Trim(Column inputText) =>
            $"TRIM({ColumnSql(inputText)})";

        protected override string Upper(Column inputText) =>
            $"UPPER({ColumnSql(inputText)})";

        protected override string Floor(Column expression) =>
            $"FLOOR({ColumnSql(expression)})";

        protected override string Log(Column expression, Column logBase) =>
            logBase == null ?
            $"LOG({ColumnSql(expression)})" :
            $"LOG({ColumnsSql(logBase, expression)})";

        protected override string Month(Column date) =>
            $"EXTRACT(month FROM {ColumnSql(date)})";

        protected override string MonthsBetween(Column date1, Column date2) =>
            $"FLOOR(MONTHS_BETWEEN({ColumnsSql(date2, date1)}))";

        protected override string Power(Column expression, Column power) =>
            $"POW({ColumnsSql(expression, power)})";

        protected override string Round(Column expression, Column precision) =>
            $"ROUND({ColumnsSql(expression, precision)})";

        protected override string Sign(Column expression) =>
            $"SIGN({ColumnSql(expression)})";

        protected override string ToChar(Column column) =>
            $"TO_CHAR({ColumnSql(column)})";

        protected override string ToTimestamp(Column column) =>
            $"TO_DATE({ColumnSql(column)})";

        protected override string ToNumber(Column column) =>
            $"TO_NUMBER({ColumnSql(column)})";

        protected override string TruncateDate(Column dateTime) =>
            $"TRUNC({ColumnSql(dateTime)})";

        protected override string Year(Column date) =>
            $"EXTRACT(year FROM {ColumnSql(date)})";

        protected override string YearsBetween(Column date1, Column date2) =>
            $"FLOOR(MONTHS_BETWEEN({ColumnsSql(date2, date1)}) / 12)";

    }
}