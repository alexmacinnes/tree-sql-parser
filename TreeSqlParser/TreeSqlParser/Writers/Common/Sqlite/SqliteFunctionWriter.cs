using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Sqlite
{
    public class SqliteFunctionWriter : FunctionWriter
    {
        public SqliteFunctionWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Abs(Column expression) =>
            $"ABS({ColumnSql(expression)})";

        protected override string AddDays(Column baseDate, Column days) =>
            $"DATETIME({ColumnSql(baseDate)}, ''||({ColumnSql(days)})||' DAYS')";

        protected override string AddMonths(Column baseDate, Column months) =>
            $"DATETIME({ColumnSql(baseDate)}, ''||({ColumnSql(months)})||' MONTHS')";

        protected override string AddYears(Column baseDate, Column years) =>
            $"DATETIME({ColumnSql(baseDate)}, ''||({ColumnSql(years)})||' YEARS')";

        protected override string Ceiling(Column expression)
        {
            string c = ColumnSql(expression);
            return $"(CAST({c} AS INT) + ({c} > CAST({c} AS INT)))";
        }

        protected override string CharIndex(Column searchText, Column inputText, Column startIndex = null) =>
            startIndex == null ?
            $"INSTR({ColumnsSql(inputText, searchText)})" :
            $"IFNULL((NULLIF(INSTR(SUBSTR({ColumnsSql(inputText, startIndex)}, {ColumnSql(searchText)}), 0)) + {ColumnSql(startIndex)} - 1), 0)";
  
        protected override string Choose(Column index, List<Column> values)
        {
            string indexSql = ColumnSql(index);

            var sb = new StringBuilder("CASE ");
            for (int i=1; i<=values.Count; i++)
                sb.Append($" WHEN {indexSql} = {i} THEN {ColumnSql(values[i-1])}");
            
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
            $"DATE(SUBSTR('0000' || {ColumnSql(year)}, -4, 4) || '-' || SUBSTR('00' || {ColumnSql(month)}, -2, 2) || '-' || SUBSTR('00' || {ColumnSql(day)}, -2, 2))";

        protected override string Day(Column date) =>
            $"CAST(STRFTIME('%d', {ColumnSql(date)}) AS INT)";

        protected override string DaysBetween(Column date1, Column date2) =>
            $"CAST(JULIANDAY({ColumnSql(date2)} AS INT)) - CAST(JULIANDAY({ColumnSql(date1)} AS INT))";

        protected override string Exp(Column expression) =>
            throw new NotSupportedException("EXP function not available on Sqlite");

        protected override string Floor(Column expression) 
        {
            string c = ColumnSql(expression);
            return $"(CAST({c} AS INT) - ({c} < CAST({c} AS INT)))";
        }

        protected override string GetDate() =>
            $"DATE()";

        protected override string GetTimestamp() =>
            "DATETIME()";            

        protected override string IsNull(Column expression, Column value) =>
            $"IFNULL({ColumnsSql(expression, value)})";

        protected override string Left(Column inputText, Column numberOfChars) =>
            $"SUBSTR({ColumnSql(inputText)}, 1, {ColumnSql(numberOfChars)})";

        protected override string Len(Column inputText) =>
            $"LENGTH({ColumnSql(inputText)})";

        protected override string Log(Column expression, Column logBase) =>
            throw new NotSupportedException("LOG function not available on Sqlite");

        protected override string Lower(Column inputText) =>
            $"LOWER({ColumnSql(inputText)})";

        protected override string LTrim(Column inputText) =>
            $"LTRIM({ColumnSql(inputText)})";

        protected override string Month(Column date) =>
            $"CAST(STRFTIME('%m', { ColumnSql(date)}) AS INT)";

        protected override string MonthsBetween(Column date1, Column date2) =>
            throw new NotSupportedException("MONTHSBETWEEN function not available on Sqlite");

        protected override string NullIf(Column expression1, Column expression2) =>
            $"NULLIF({ColumnsSql(expression1, expression2)})";

        protected override string Power(Column expression, Column power) =>
            throw new NotSupportedException("POWER function not available on Sqlite");

        protected override string Replace(Column inputText, Column oldSubstring, Column newSubstring) =>
            $"REPLACE({ColumnsSql(inputText, oldSubstring, newSubstring)})";

        protected override string Replicate(Column inputText, Column timesToRepeat) =>
            $"REPLACE(PRINTF('%.' || ({ColumnSql(timesToRepeat)}) || 'c', '/'),'/', {ColumnSql(inputText)})";

        protected override string Reverse(Column inputText) =>
            throw new NotSupportedException("REVERSE function not available on Sqlite");

        protected override string Right(Column inputText, Column numberOfChars)
        {
            string numberOfCharsSql = ColumnSql(numberOfChars);
            string startIndexSql = numberOfChars is IntegerColumn ? "-" + numberOfCharsSql : $"0 - ({numberOfCharsSql})";
            return $"SUBSTR({ColumnSql(inputText)}, {startIndexSql}, {numberOfCharsSql})";
        }
           

        protected override string Round(Column expression, Column precision) =>
            $"ROUND({ColumnsSql(expression, precision)})";

        protected override string RTrim(Column inputText) =>
            $"RTRIM({ColumnSql(inputText)})";

        protected override string Sign(Column expression) =>
            $"SIGN({ColumnSql(expression)})";

        protected override string Space(Column length) =>
            $"REPLACE(PRINTF('%.' || ({ColumnSql(length)}) || 'c', '/'),'/', ' ')";

        protected override string Substring(Column inputText, Column startIndex, Column numberOfChars) =>
            $"SUBSTR({ColumnsSql(inputText, startIndex, numberOfChars)})";

        protected override string ToNumber(Column column) =>
            $"CAST({ColumnSql(column)} AS REAL)";

        protected override string ToTimestamp(Column column) =>
            $"DATETIME({ColumnSql(column)})";

        protected override string ToChar(Column column) =>
            $"CAST({ColumnSql(column)} AS NVARCHAR)";

        protected override string Trim(Column inputText) =>
            $"TRIM({ColumnSql(inputText)})";

        protected override string TruncateDate(Column dateTime) =>
            $"DATE({ColumnSql(dateTime)})";

        protected override string Upper(Column inputText) =>
            $"UPPER({ColumnSql(inputText)})";

        protected override string Year(Column date) =>
            $"CAST(STRFTIME('%Y', {ColumnSql(date)}) AS INT)";

        protected override string YearsBetween(Column date1, Column date2) =>
            throw new NotSupportedException("YEARSBETWEEN function not available on Sqlite");
    }
}
