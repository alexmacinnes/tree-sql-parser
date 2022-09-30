using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Db2
{
    public class Db2FunctionWriter : FunctionWriter
    {
        public Db2FunctionWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Abs(Column expression) =>
            $"ABS({ColumnSql(expression)})";

        protected override string AddDays(Column baseDate, Column days) =>
            $"ADD_DAYS({ColumnsSql(baseDate, days)})";

        protected override string AddMonths(Column baseDate, Column months) =>
            $"ADD_MONTHS({ColumnsSql(baseDate, months)})";

        protected override string AddYears(Column baseDate, Column years) =>
            $"ADD_YEARS({ColumnsSql(baseDate, years)})";

        protected override string Ceiling(Column expression) =>
            $"CEIL({ColumnSql(expression)})";

        protected override string CharIndex(Column searchText, Column inputText, Column startIndex = null) =>
            startIndex == null ?
            $"INSTR({ColumnsSql(inputText, searchText)})" :
            $"INSTR({ColumnsSql(inputText, searchText, startIndex)})";

        protected override string Choose(Column index, List<Column> values)
        {
            var sb = new StringBuilder();

            sb.Append("DECODE(").Append(ColumnSql(index));
            for (int i = 0; i < values.Count; i++)
                sb.Append($", {i + 1}, {ColumnSql(values[i])}");
            sb.Append(")");

            return sb.ToString();
        }

        protected override string Coalesce(IReadOnlyList<Column> expressions) =>
            $"COALESCE({ColumnsSql(expressions.ToArray())})";

        protected override string Concat(IReadOnlyList<Column> strings) =>
            ConcatWithOperator(strings);

        protected override string ConcatWithSeperator(Column seperator, IReadOnlyList<Column> strings) =>
            ConcatWithSeperatorWithOperator(seperator, strings);

        protected override string DateFromParts(Column year, Column month, Column day) =>
            $"DATE(({ColumnSql(year)}) || '-' || ({ColumnSql(month)}) || '-' || ({ColumnSql(day)}))";

        protected override string Day(Column date) =>
            $"DAYOFMONTH({ColumnSql(date)})";

        protected override string DaysBetween(Column date1, Column date2) =>
            $"DAYS_BETWEEN({ColumnsSql(date2, date1)})";

        protected override string Exp(Column expression) =>
            $"EXP({ColumnSql(expression)})";

        protected override string Floor(Column expression) =>
            $"FLOOR({ColumnSql(expression)})";

        protected override string GetDate() =>
            $"(CURRENT DATE)";

        protected override string GetTimestamp() =>
            "(CURRENT TIMESTAMP)";            

        protected override string IsNull(Column expression, Column value) =>
            $"NVL({ColumnsSql(expression, value)})";

        protected override string Left(Column inputText, Column numberOfChars) =>
            $"LEFT({ColumnsSql(inputText, numberOfChars)})";

        protected override string Len(Column inputText) =>
            $"LENGTH({ColumnSql(inputText)})";

        protected override string Log(Column expression, Column logBase)
        {
            if (logBase == null)
                return $"LN({ColumnSql(expression)})";

            var intColumn = logBase as IntegerColumn;
            if (intColumn?.Value == 10)
                return $"LOG10({ColumnSql(expression)})";

            throw new InvalidOperationException("Variable log base not supported on DB2. Valid options are <null> for natural log, or 10.");
        }
            

        protected override string Lower(Column inputText) =>
            $"LOWER({ColumnSql(inputText)})";

        protected override string LTrim(Column inputText) =>
            $"LTRIM({ColumnSql(inputText)})";

        protected override string Month(Column date) =>
            $"MONTH({ColumnSql(date)})";

        protected override string MonthsBetween(Column date1, Column date2) =>
            MonthsBetweenFromMonthsAndYears(date1, date2);

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
            $"DOUBLE({ColumnSql(column)})";

        protected override string ToTimestamp(Column column) =>
            $"TIMESTAMP({ColumnSql(column)})";

        protected override string ToChar(Column column) =>
            $"NVARCHAR({ColumnSql(column)})";

        protected override string Trim(Column inputText) =>
            $"TRIM({ColumnSql(inputText)})";

        protected override string TruncateDate(Column dateTime) =>
            $"DATE({ColumnSql(dateTime)})";

        protected override string Upper(Column inputText) =>
            $"UPPER({ColumnSql(inputText)})";

        protected override string Year(Column date) =>
            $"YEAR({ColumnSql(date)})";

        protected override string YearsBetween(Column date1, Column date2) =>
            YearsBetweenFromYears(date1, date2);
    }
}
