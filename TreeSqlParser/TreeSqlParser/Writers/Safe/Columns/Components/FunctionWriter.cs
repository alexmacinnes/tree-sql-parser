using System;
using System.Collections.Generic;
using System.Linq;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public abstract class FunctionWriter : IFunctionWriter
    {
        private readonly IDictionary<string, Func<FunctionColumn, string>> functionWriters =
            new Dictionary<string, Func<FunctionColumn, string>>(StringComparer.InvariantCultureIgnoreCase);

        private SafeSqlWriter SqlWriter { get; }

        protected string ColumnSql(Column c) => SqlWriter.ColumnSql(c);

        protected string ColumnsSql(params Column[] c) => string.Join(", ", c.Select(ColumnSql));

        public FunctionWriter(SafeSqlWriter sqlWriter)
        {
            SqlWriter = sqlWriter;

            functionWriters["Abs"] = x => Abs(x.Parameters[0]);
            functionWriters["AddDays"] = x => AddDays(x.Parameters[0], x.Parameters[1]);
            functionWriters["AddMonths"] = x => AddMonths(x.Parameters[0], x.Parameters[1]);
            functionWriters["AddYears"] = x => AddYears(x.Parameters[0], x.Parameters[1]);
            functionWriters["Ceiling"] = x => Ceiling(x.Parameters[0]);
            functionWriters["CharIndex"] = x => CharIndex(x.Parameters[0], x.Parameters[1], x.Parameters.Count == 3 ? x.Parameters[2] : null);
            functionWriters["Coalesce"] = x => Coalesce(x.Parameters);
            functionWriters["Choose"] = x => Choose(x.Parameters[0], x.Parameters.Skip(1).ToList());
            functionWriters["Concat"] = x => Concat(x.Parameters);
            functionWriters["Concat_WS"] = x => ConcatWithSeperator(x.Parameters[0], x.Parameters.Skip(1).ToList());
            functionWriters["DateFromParts"] = x => DateFromParts(x.Parameters[0], x.Parameters[1], x.Parameters[2]);
            functionWriters["Day"] = x => Day(x.Parameters[0]);
            functionWriters["DaysBetween"] = x => DaysBetween(x.Parameters[0], x.Parameters[1]);
            functionWriters["Exp"] = x => Exp(x.Parameters[0]);
            functionWriters["Floor"] = x => Floor(x.Parameters[0]);
            functionWriters["GetDate"] = _ => GetDate();
            functionWriters["GetTimestamp"] = _ => GetTimestamp();
            functionWriters["IsNull"] = x => IsNull(x.Parameters[0], x.Parameters[1]);
            functionWriters["Left"] = x => Left(x.Parameters[0], x.Parameters[1]);
            functionWriters["Len"] = x => Len(x.Parameters[0]);
            functionWriters["Log"] = x => Log(x.Parameters[0], x.Parameters.Count == 2 ? x.Parameters[1] : null);
            functionWriters["Lower"] = x => Lower(x.Parameters[0]);
            functionWriters["LTrim"] = x => LTrim(x.Parameters[0]);
            functionWriters["Month"] = x => Month(x.Parameters[0]);
            functionWriters["MonthsBetween"] = x => MonthsBetween(x.Parameters[0], x.Parameters[1]);
            functionWriters["NullIf"] = x => NullIf(x.Parameters[0], x.Parameters[1]);
            functionWriters["Power"] = x => Power(x.Parameters[0], x.Parameters[1]);
            functionWriters["Replace"] = x => Replace(x.Parameters[0], x.Parameters[1], x.Parameters[2]);
            functionWriters["Replicate"] = x => Replicate(x.Parameters[0], x.Parameters[1]);
            functionWriters["Reverse"] = x => Reverse(x.Parameters[0]);
            functionWriters["Right"] = x => Right(x.Parameters[0], x.Parameters[1]);
            functionWriters["Round"] = x => Round(x.Parameters[0], x.Parameters[1]);
            functionWriters["RTrim"] = x => RTrim(x.Parameters[0]);
            functionWriters["Sign"] = x => Sign(x.Parameters[0]);
            functionWriters["Space"] = x => Space(x.Parameters[0]);
            functionWriters["Substring"] = x => Substring(x.Parameters[0], x.Parameters[1], x.Parameters[2]);
            functionWriters["ToChar"] = x => ToChar(x.Parameters[0]);
            functionWriters["ToTimestamp"] = x => ToTimestamp(x.Parameters[0]);
            functionWriters["ToNumber"] = x => ToNumber(x.Parameters[0]);
            functionWriters["Trim"] = x => Trim(x.Parameters[0]);
            functionWriters["TruncateDate"] = x => TruncateDate(x.Parameters[0]);
            functionWriters["Upper"] = x => Upper(x.Parameters[0]);
            functionWriters["Year"] = x => Year(x.Parameters[0]);
            functionWriters["YearsBetween"] = x => YearsBetween(x.Parameters[0], x.Parameters[1]);
        }

        public virtual string FunctionSql(FunctionColumn column)
        {
            Func<FunctionColumn, string> sqlFunction = null;

            if (column.Name.Length == 1)
                functionWriters.TryGetValue(column.Name[0].Name, out sqlFunction);

            if (sqlFunction == null)
                throw new NotSupportedException("Unsupported SQL function: " + string.Join(".", column.Name.Select(x => x.Name ?? "")));

            string result = sqlFunction(column);
            return result;
        }

        #region Abstract Functions

        /// <summary>The absolute value of the expression</summary>
        protected abstract string Abs(Column expression);

        /// <summary>Add the specified number of days onto a base date</summary>
        protected abstract string AddDays(Column baseDate, Column days);

        /// <summary>Add the specified number of days onto a base date</summary>
        protected abstract string AddMonths(Column baseDate, Column months);

        /// <summary>Add the specified number of years onto a base date</summary>
        protected abstract string AddYears(Column baseDate, Column years);

        /// <summary>Round the expression up</summary>
        protected abstract string Ceiling(Column expression);

        /// <summary>1 based index of first occurence of searchText within inputText, or 0 if not found</summary>
        /// <param name="searchText">The substring to find</param>
        /// <param name="inputText">The string to search in</param>
        /// <param name="startIndex">Optional. 1 based index from which to start searching</param>
        protected abstract string CharIndex(Column searchText, Column inputText, Column startIndex = null);

        /// <summary>The item in valueExpressions that corresponds to the 1-based index</summary>
        /// <param name="index">1 based index into valueExpressions</param>
        /// <param name="values">Possible return values</param>
        protected abstract string Choose(Column index, List<Column> values);

        /// <summary>Returns the first non null expression result</summary>
        protected abstract string Coalesce(IReadOnlyList<Column> expressions);

        /// <summary>Concatenate 2 or more strings together</summary>
        protected abstract string Concat(IReadOnlyList<Column> strings);

        /// <summary>Concatenate 2 or more strings together</summary>
        /// <param name="seperator">The seperator to insert between concatenated strings</param>
        /// <param name="strings">The string to concatenate</param>
        protected abstract string ConcatWithSeperator(Column seperator, IReadOnlyList<Column> strings);

        /// <summary>Construct a date from individual year, month and day values</summary>
        protected abstract string DateFromParts(Column year, Column month, Column day);

        /// <summary>Extract the day part from a date</summary>
        protected abstract string Day(Column date);

        /// <summary>The number of days between 2 dates. A positive number indicates date2 is the later date</summary>
        protected abstract string DaysBetween(Column date1, Column date2);

        /// <summary>The exponential value of the specified expression</summary>
        protected abstract string Exp(Column expression);

        /// <summary>Round the expression down</summary>
        protected abstract string Floor(Column expression);

        /// <summary>The current system date</summary>
        protected abstract string GetDate();

        /// <summary>The current system timestamp</summary>
        protected abstract string GetTimestamp();

        /// <summary>The first N characters from a string/// 
        protected abstract string Left(Column inputText, Column numberOfChars);

        /// <summary>The number of characters in a string. Trailing spaces are not counted, but leading spaces are.</summary>
        protected abstract string Len(Column inputText);

        /// <summary>The logarithm of the specified expression</summary>
        /// <param name="expression">The input expression</param>
        /// <param name="logBase">Optional base of the log. If null, defaults to natural log</param>
        protected abstract string Log(Column expression, Column logBase);

        /// <summary>Convert string to lower case</summary>
        protected abstract string Lower(Column inputText);

        /// <summary>Strip of leading spaces from a string</summary>
        protected abstract string LTrim(Column inputText);

        /// <summary>Returns a specified value if the expression is NULL. Else return the expression.</summary>
        /// <param name="expression">The expression to test if NULL</param>
        /// <param name="value">The value to return if expression is NULL</param>
        protected abstract string IsNull(Column expression, Column value);

        /// <summary>Extract the month part from a date</summary>
        protected abstract string Month(Column date);

        /// <summary>The number of months between 2 dates. A positive number indicates date2 is the later date</summary>
        protected abstract string MonthsBetween(Column date1, Column date2);

        /// <summary>Null if expression1 and expression2 are equal. Else returns expression1.</summary>
        protected abstract string NullIf(Column expression1, Column expression2);

        /// <summary>The specified expression, raised to the specified power</summary>
        protected abstract string Power(Column expression, Column power);

        /// <summary>Replace all occurrences of a substring within a string, with a new substring.</summary>
        /// <param name="inputText">The original string</param>
        /// <param name="oldSubstring">The substring to be replaced. Case insensitive.</param>
        /// <param name="newSubstring">The new substring to be inserted.</param>
        protected abstract string Replace(Column inputText, Column oldSubstring, Column newSubstring);

        /// <summary>Repeats a string N times </summary>///
        protected abstract string Replicate(Column inputText, Column timesToRepeat);

        /// <summary>Reverse a string </summary>
        protected abstract string Reverse(Column inputText);

        /// <summary>The last N characters from a string/// 
        protected abstract string Right(Column inputText, Column numberOfChars);

        /// <summary>Round the expression to the specified precision</summary>
        /// <param name="expression">The expression to round</param>
        /// <param name="precision">The rounding precision. A positive number specifies number of decimal places. A negative number specified places to the left of the decimal point.</param>
        protected abstract string Round(Column expression, Column precision);

        /// <summary>Strip of leading spaces from a string</summary>
        protected abstract string RTrim(Column inputText);

        /// <summary>The positive (+1), zero (0), or negative (-1) sign of the specified expression</summary>
        protected abstract string Sign(Column expression);

        /// <summary>A whitespace string of spaces of the specified length</summary>
        protected abstract string Space(Column length);

        /// <summary>Extracts a substring from a larger string</summary>
        /// <param name="inputText">The full string from which to extract</param>
        /// <param name="startIndex">1 based index from which to start extraction</param>
        /// <param name="numberOfChars">The number of characters to extract</param>
        protected abstract string Substring(Column inputText, Column startIndex, Column numberOfChars);

        /// <summary>Convert input to a number</summary>
        protected abstract string ToNumber(Column column);

        /// <summary>Convert input to a date</summary>
        protected abstract string ToTimestamp(Column column);

        /// <summary>Convert input to text</summary>
        protected abstract string ToChar(Column column);

        /// <summary>Strip of leading and trailing spaces from a string</summary>
        protected abstract string Trim(Column inputText);

        /// <summary>The date portion of a timestamp</summary>
        protected abstract string TruncateDate(Column dateTime);

        /// <summary>Convert string to upper case</summary>
        protected abstract string Upper(Column inputText);

        /// <summary>Extract the year part from a date</summary>
        protected abstract string Year(Column date);

        /// <summary>The number of years between 2 dates. A positive number indicates date2 is the later date</summary>
        protected abstract string YearsBetween(Column date1, Column date2);

        #endregion



    }
}
