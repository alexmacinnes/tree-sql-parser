using NUnit.Framework;
using System;
using System.Collections.Generic;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using TreeSqlParser.Writers.Common.MySql;
using TreeSqlParser.Writers.Common.Oracle;
using TreeSqlParser.Writers.Common.Sqlite;
using TreeSqlParser.Writers.Common.SqlServer;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    class FunctionWriterTests
    {
        private Column ParseColumn(string sql) =>
            SelectParser.ParseColumn(sql);

        private string Sql(Column c, SqlWriterType db) => CommonMother.Sql(c, db);

        private void AssertSql(Column c, SqlWriterType db, string expected) =>
            Assert.AreEqual(expected, Sql(c, db));

        [Test]
        public void Abs()
        {
            var c = ParseColumn("ABS(-1.23)");

            AssertSql(c, SqlWriterType.SqlServer, "ABS(-1.23)");
            AssertSql(c, SqlWriterType.Oracle, "ABS(-1.23)");
            AssertSql(c, SqlWriterType.MySql, "ABS(-1.23)");
            AssertSql(c, SqlWriterType.Sqlite, "ABS(-1.23)");
        }

        [Test]
        public void AddDays()
        {
            var c = ParseColumn("ADDDAYS({d '2000-12-31'}, 365)");

            AssertSql(c, SqlWriterType.SqlServer, "DATEADD(d, 365, {d '2000-12-31'})");
            AssertSql(c, SqlWriterType.Oracle, "(DATE '2000-12-31' + 365)");
            AssertSql(c, SqlWriterType.MySql, "ADDDATE(DATE('2000-12-31'), INTERVAL (365) DAY)");
            AssertSql(c, SqlWriterType.Sqlite, "DATETIME(DATE('2000-12-31'), ''||(365)||' DAYS')");
        }

        [Test]
        public void AddMonths()
        {
            var c = ParseColumn("ADDMONTHS({d '2000-12-31'}, 3)");

            AssertSql(c, SqlWriterType.SqlServer, "DATEADD(m, 3, {d '2000-12-31'})");
            AssertSql(c, SqlWriterType.Oracle, "ADD_MONTHS(DATE '2000-12-31', 3)");
            AssertSql(c, SqlWriterType.MySql, "ADDDATE(DATE('2000-12-31'), INTERVAL (3) MONTH)");
            AssertSql(c, SqlWriterType.Sqlite, "DATETIME(DATE('2000-12-31'), ''||(3)||' MONTHS')");
        }

        [Test]
        public void AddYears()
        {
            var c = ParseColumn("ADDYEARS({d '2000-12-31'}, 3)");

            AssertSql(c, SqlWriterType.SqlServer, "DATEADD(y, 3, {d '2000-12-31'})");
            AssertSql(c, SqlWriterType.Oracle, "ADD_MONTHS(DATE '2000-12-31', 12 * (3)");
            AssertSql(c, SqlWriterType.MySql, "ADDDATE(DATE('2000-12-31'), INTERVAL (3) YEAR)");
            AssertSql(c, SqlWriterType.Sqlite, "DATETIME(DATE('2000-12-31'), ''||(3)||' YEARS')");
        }

        [Test]
        public void Ceiling()
        {
            var c = ParseColumn("CEILING(1.23)");

            AssertSql(c, SqlWriterType.SqlServer, "CEILING(1.23)");
            AssertSql(c, SqlWriterType.Oracle, "CEIL(1.23)");
            AssertSql(c, SqlWriterType.MySql, "CEIL(1.23)");
            AssertSql(c, SqlWriterType.Sqlite, "(CAST(1.23 AS INT) + (1.23 > CAST(1.23 AS INT)))");
        }

        [Test]
        public void CharIndex()
        {
            var c = ParseColumn("CHARINDEX('abcde', 'd')");

            AssertSql(c, SqlWriterType.SqlServer, "CHARINDEX('abcde', 'd')");
            AssertSql(c, SqlWriterType.Oracle, "INSTR('d', 'abcde')");
            AssertSql(c, SqlWriterType.MySql, "INSTR('d', 'abcde')");
            AssertSql(c, SqlWriterType.Sqlite, "INSTR('d', 'abcde')");
        }

        [Test]
        public void CharIndexWithStartIndex()
        {
            var c = ParseColumn("CHARINDEX('abcde', 'd', 2)");

            AssertSql(c, SqlWriterType.SqlServer, "CHARINDEX('abcde', 'd', 2)");
            AssertSql(c, SqlWriterType.Oracle, "INSTR('d', 'abcde', 2)");
            AssertSql(c, SqlWriterType.MySql, "IFNULL((NULLIF(INSTR(SUBSTR('d', 2, 'abcde'), 0)) + 2 - 1, 0)");
            AssertSql(c, SqlWriterType.Sqlite, "IFNULL((NULLIF(INSTR(SUBSTR('d', 2, 'abcde'), 0)) + 2 - 1), 0)");
        }

        [Test]
        public void Choose()
        {
            var c = ParseColumn("CHOOSE(2, 'one', 'two', 'three')");

            AssertSql(c, SqlWriterType.SqlServer, "CHOOSE(2, 'one', 'two', 'three')");
            AssertSql(c, SqlWriterType.Oracle, "CASE WHEN 2 = 1 THEN 'one' WHEN 2 = 2 THEN 'two' WHEN 2 = 3 THEN 'three' END");
            AssertSql(c, SqlWriterType.MySql, "ELT(2, 'one', 'two', 'three')");
            AssertSql(c, SqlWriterType.Sqlite, "CASE  WHEN 2 = 1 THEN 'one' WHEN 2 = 2 THEN 'two' WHEN 2 = 3 THEN 'three' END");
        }

        [Test]
        public void Coalesce()
        {
            var c = ParseColumn("COALESCE(1, 2, 3)");

            AssertSql(c, SqlWriterType.SqlServer, "COALESCE(1, 2, 3)");
            AssertSql(c, SqlWriterType.Oracle, "COALESCE(1, 2, 3)");
            AssertSql(c, SqlWriterType.MySql, "COALESCE(1, 2, 3)");
            AssertSql(c, SqlWriterType.Sqlite, "COALESCE(1, 2, 3)");
        }

        [Test]
        public void Concat()
        {
            var c = ParseColumn("CONCAT('a','b','c')");

            AssertSql(c, SqlWriterType.SqlServer, "CONCAT('a', 'b', 'c')");
            AssertSql(c, SqlWriterType.Oracle, "'a' || 'b' || 'c'");
            AssertSql(c, SqlWriterType.MySql, "CONCAT('a', 'b', 'c')");
            AssertSql(c, SqlWriterType.Sqlite, "'a' || 'b' || 'c'");
        }

        [Test]
        public void ConcatWithSeperator()
        {
            var c = ParseColumn("CONCAT_WS('___', 'a','b','c')");

            AssertSql(c, SqlWriterType.SqlServer, "CONCAT_WS('___', 'a', 'b', 'c')");
            AssertSql(c, SqlWriterType.Oracle, "'a' || '___' || 'b' || '___' || 'c'");
            AssertSql(c, SqlWriterType.MySql, "CONCAT_WS('___', 'a', 'b', 'c')");
            AssertSql(c, SqlWriterType.Sqlite, "'a' || '___' || 'b' || '___' || 'c'");
        }

        [Test]
        public void DateFromParts()
        {
            var c = ParseColumn("DATEFROMPARTS(2020, 12, 31)");

            AssertSql(c, SqlWriterType.SqlServer, "DATEFROMPARTS(2020, 12, 31)");
            AssertSql(c, SqlWriterType.Oracle, "TO_DATE((2020) || (12) || (31), 'yyyyMMdd')");
            AssertSql(c, SqlWriterType.MySql, "DATE(CONCAT_WS('-', 2020, 12, 31)))");
            AssertSql(c, SqlWriterType.Sqlite, "DATE(SUBSTR('0000' || 2020, -4, 4) || '-' || SUBSTR('00' || 12, -2, 2) || '-' || SUBSTR('00' || 31, -2, 2))");
        }

        [Test]
        public void Day()
        {
            var c = ParseColumn("DAY({d '2000-12-31'})");

            AssertSql(c, SqlWriterType.SqlServer, "DAY({d '2000-12-31'})");
            AssertSql(c, SqlWriterType.Oracle, "EXTRACT(day FROM DATE '2000-12-31')");
            AssertSql(c, SqlWriterType.MySql, "DAY(DATE('2000-12-31'))");
            AssertSql(c, SqlWriterType.Sqlite, "CAST(STRFTIME('%d', DATE('2000-12-31')) AS INT)");
        }

        [Test]
        public void DaysBetween()
        {
            var c = ParseColumn("DAYSBETWEEN({d '2000-12-31'}, {d '2001-12-31'})");

            AssertSql(c, SqlWriterType.SqlServer, "DATEDIFF(d, {d '2000-12-31'}, {d '2001-12-31'})");
            AssertSql(c, SqlWriterType.Oracle, "(DATE '2001-12-31' - DATE '2000-12-31')");
            AssertSql(c, SqlWriterType.MySql, "DATEDIFF(DATE('2001-12-31'), DATE('2000-12-31'))");
            AssertSql(c, SqlWriterType.Sqlite, "CAST(JULIANDAY(DATE('2001-12-31') AS INT)) - CAST(JULIANDAY(DATE('2000-12-31') AS INT))");
        }

        [Test]
        public void Exp()
        {
            var c = ParseColumn("EXP(1.23)");

            AssertSql(c, SqlWriterType.SqlServer, "EXP(1.23)");
            AssertSql(c, SqlWriterType.Oracle, "EXP(1.23)");
            AssertSql(c, SqlWriterType.MySql, "EXP(1.23)");
            AssertSql(c, SqlWriterType.Sqlite, "EXCEPTION: EXP function not available on Sqlite");
        }

        [Test]
        public void Floor()
        {
            var c = ParseColumn("FLOOR(1.23)");

            AssertSql(c, SqlWriterType.SqlServer, "FLOOR(1.23)");
            AssertSql(c, SqlWriterType.Oracle, "FLOOR(1.23)");
            AssertSql(c, SqlWriterType.MySql, "FLOOR(1.23)");
            AssertSql(c, SqlWriterType.Sqlite, "(CAST(1.23 AS INT) - (1.23 < CAST(1.23 AS INT)))");

        }

        [Test]
        public void GetDate()
        {
            var c = ParseColumn("GETDATE()");

            AssertSql(c, SqlWriterType.SqlServer, "CONVERT(date, GETDATE())");
            AssertSql(c, SqlWriterType.Oracle, "CURRENT_DATE");
            AssertSql(c, SqlWriterType.MySql, "DATE(CURRENT_DATE())");
            AssertSql(c, SqlWriterType.Sqlite, "DATE()");
        }

        [Test]
        public void GetTimestamp()
        {
            var c = ParseColumn("GETTIMESTAMP()");

            AssertSql(c, SqlWriterType.SqlServer, "GETDATE()");
            AssertSql(c, SqlWriterType.Oracle, "CURRENT_TIMESTAMP");
            AssertSql(c, SqlWriterType.MySql, "TIMESTAMP(CURRENT_TIMESTAMP())");
            AssertSql(c, SqlWriterType.Sqlite, "DATETIME()");
        }

        [Test]
        public void IsNull()
        {
            var c = ParseColumn("ISNULL(x.y, 0)");

            AssertSql(c, SqlWriterType.SqlServer, "ISNULL([x].[y], 0)");
            AssertSql(c, SqlWriterType.Oracle, "NVL(\"x\".\"y\", 0)");
            AssertSql(c, SqlWriterType.MySql, "ISNULL(`x`.`y`, 0)");
            AssertSql(c, SqlWriterType.Sqlite, "IFNULL([x].[y], 0)");
        }

        [Test]
        public void Left()
        {
            var c = ParseColumn("LEFT('abcde', 2)");

            AssertSql(c, SqlWriterType.SqlServer, "LEFT('abcde', 2)");
            AssertSql(c, SqlWriterType.Oracle, "SUBSTR('abcde', 1, 2)");
            AssertSql(c, SqlWriterType.MySql, "LEFT('abcde', 2)");
            AssertSql(c, SqlWriterType.Sqlite, "SUBSTR('abcde', 1, 2)");
        }

        [Test]
        public void Len()
        {
            var c = ParseColumn("LEN('abc')");

            AssertSql(c, SqlWriterType.SqlServer, "LEN('abc')");
            AssertSql(c, SqlWriterType.Oracle, "LENGTH(RTRIM('abc'))");
            AssertSql(c, SqlWriterType.MySql, "LENGTH('abc')");
            AssertSql(c, SqlWriterType.Sqlite, "LENGTH('abc')");
        }

        [Test]
        public void LogDefaultBase()
        {
            var c = ParseColumn("LOG(1000)");

            AssertSql(c, SqlWriterType.SqlServer, "LOG(1000)");
            AssertSql(c, SqlWriterType.Oracle, "LOG(1000)");
            AssertSql(c, SqlWriterType.MySql, "LOG(1000)");
            AssertSql(c, SqlWriterType.Sqlite, "EXCEPTION: LOG function not available on Sqlite");
        }

        [Test]
        public void LogWithBase()
        {
            var c = ParseColumn("LOG(1000, 10)");

            AssertSql(c, SqlWriterType.SqlServer, "LOG(1000, 10)");
            AssertSql(c, SqlWriterType.Oracle, "LOG(10, 1000)");
            AssertSql(c, SqlWriterType.MySql, "LOG(10, 1000)");
            AssertSql(c, SqlWriterType.Sqlite, "EXCEPTION: LOG function not available on Sqlite");
        }

        [Test]
        public void Lower()
        {
            var c = ParseColumn("LOWER('abc')");

            AssertSql(c, SqlWriterType.SqlServer, "LOWER('abc')");
            AssertSql(c, SqlWriterType.Oracle, "LOWER('abc')");
            AssertSql(c, SqlWriterType.MySql, "LOWER('abc')");
            AssertSql(c, SqlWriterType.Sqlite, "LOWER('abc')");
        }

        [Test]
        public void LTrim()
        {
            var c = ParseColumn("LTRIM('abc')");

            AssertSql(c, SqlWriterType.SqlServer, "LTRIM('abc')");
            AssertSql(c, SqlWriterType.Oracle, "LTRIM('abc')");
            AssertSql(c, SqlWriterType.MySql, "LTRIM('abc')");
            AssertSql(c, SqlWriterType.Sqlite, "LTRIM('abc')");
        }

        [Test]
        public void Month()
        {
            var c = ParseColumn("MONTH({d '2000-12-31'})");

            AssertSql(c, SqlWriterType.SqlServer, "MONTH({d '2000-12-31'})");
            AssertSql(c, SqlWriterType.Oracle, "EXTRACT(month FROM DATE '2000-12-31')");
            AssertSql(c, SqlWriterType.MySql, "MONTH(DATE('2000-12-31'))");
            AssertSql(c, SqlWriterType.Sqlite, "CAST(STRFTIME('%m', DATE('2000-12-31')) AS INT)");
        }

        [Test]
        public void MonthsBetween()
        {
            var c = ParseColumn("MONTHSBETWEEN({d '2000-12-30'}, {d '2001-12-31'})");

            AssertSql(c, SqlWriterType.SqlServer, "DATEDIFF(m, {d '2000-12-30'}, {d '2001-12-31'})");
            AssertSql(c, SqlWriterType.Oracle, "FLOOR(MONTHS_BETWEEN(DATE '2001-12-31', DATE '2000-12-30'))");
            AssertSql(c, SqlWriterType.MySql, "TIMESTAMPDIFF(MONTH, DATE('2000-12-30'), DATE('2001-12-31'))");
            AssertSql(c, SqlWriterType.Sqlite, "EXCEPTION: MONTHSBETWEEN function not available on Sqlite");
        }

        [Test]
        public void NullIf()
        {
            var c = ParseColumn("NULLIF('abc', 'N/A')");

            AssertSql(c, SqlWriterType.SqlServer, "NULLIF('abc', 'N/A')");
            AssertSql(c, SqlWriterType.Oracle, "NULLIF('abc', 'N/A')");
            AssertSql(c, SqlWriterType.MySql, "NULLIF('abc', 'N/A')");
            AssertSql(c, SqlWriterType.Sqlite, "NULLIF('abc', 'N/A')");
        }

        [Test]
        public void Power()
        {
            var c = ParseColumn("POWER(10, 2)");

            AssertSql(c, SqlWriterType.SqlServer, "POWER(10, 2)");
            AssertSql(c, SqlWriterType.Oracle, "POW(10, 2)");
            AssertSql(c, SqlWriterType.MySql, "POW(10, 2)");
            AssertSql(c, SqlWriterType.Sqlite, "EXCEPTION: POWER function not available on Sqlite");
        }

        [Test]
        public void Replace()
        {
            var c = ParseColumn("REPLACE('abCde', 'C', 'c')");

            AssertSql(c, SqlWriterType.SqlServer, "REPLACE('abCde', 'C', 'c')");
            AssertSql(c, SqlWriterType.Oracle, "REPLACE('abCde', 'C', 'c')");
            AssertSql(c, SqlWriterType.MySql, "REPLACE('abCde', 'C', 'c')");
            AssertSql(c, SqlWriterType.Sqlite, "REPLACE('abCde', 'C', 'c')");
        }

        [Test]
        public void Replicate()
        {
            var c = ParseColumn("REPLICATE('abc', 5)");

            AssertSql(c, SqlWriterType.SqlServer, "REPLICATE('abc', 5)");
            AssertSql(c, SqlWriterType.Oracle, "RPAD('', LENGTH(RTRIM('abc')) * 5, 'abc')");
            AssertSql(c, SqlWriterType.MySql, "REPEAT('abc', 5)");
            AssertSql(c, SqlWriterType.Sqlite, "REPLACE(PRINTF('%.' || (5) || 'c', '/'),'/', 'abc')");
        }

        [Test]
        public void Reverse()
        {
            var c = ParseColumn("REVERSE('abc')");

            AssertSql(c, SqlWriterType.SqlServer, "REVERSE('abc')");
            AssertSql(c, SqlWriterType.Oracle, "REVERSE('abc')");
            AssertSql(c, SqlWriterType.MySql, "REVERSE('abc')");
            AssertSql(c, SqlWriterType.Sqlite, "EXCEPTION: REVERSE function not available on Sqlite");
        }

        [Test]
        public void RightFixedWidth()
        {
            var c = ParseColumn("RIGHT('abcde', 2)");

            AssertSql(c, SqlWriterType.SqlServer, "RIGHT('abcde', 2)");
            AssertSql(c, SqlWriterType.Oracle, "SUBSTR('abcde', -2, 2)");
            AssertSql(c, SqlWriterType.MySql, "RIGHT('abcde', 2)");
            AssertSql(c, SqlWriterType.Sqlite, "SUBSTR('abcde', -2, 2)");
        }

        [Test]
        public void RightVariableWidth()
        {
            var c = ParseColumn("RIGHT('abcde', 2*3)");

            AssertSql(c, SqlWriterType.SqlServer, "RIGHT('abcde', 2*3)");
            AssertSql(c, SqlWriterType.Oracle, "SUBSTR('abcde', 0 - (2*3), 2*3)");
            AssertSql(c, SqlWriterType.MySql, "RIGHT('abcde', 2*3)");
            AssertSql(c, SqlWriterType.Sqlite, "SUBSTR('abcde', 0 - (2*3), 2*3)");
        }

        [Test]
        public void Round()
        {
            var c = ParseColumn("ROUND(12.34, -1)");

            AssertSql(c, SqlWriterType.SqlServer, "ROUND(12.34, -1)");
            AssertSql(c, SqlWriterType.Oracle, "ROUND(12.34, -1)");
            AssertSql(c, SqlWriterType.MySql, "ROUND(12.34, -1)");
            AssertSql(c, SqlWriterType.Sqlite, "ROUND(12.34, -1)");
        }

        [Test]
        public void RTrim()
        {
            var c = ParseColumn("RTRIM('abc')");

            AssertSql(c, SqlWriterType.SqlServer, "RTRIM('abc')");
            AssertSql(c, SqlWriterType.Oracle, "RTRIM('abc')");
            AssertSql(c, SqlWriterType.MySql, "RTRIM('abc')");
            AssertSql(c, SqlWriterType.Sqlite, "RTRIM('abc')");
        }

        [Test]
        public void Sign()
        {
            var c = ParseColumn("SIGN(-1.23)");

            AssertSql(c, SqlWriterType.SqlServer, "SIGN(-1.23)");
            AssertSql(c, SqlWriterType.Oracle, "SIGN(-1.23)");
            AssertSql(c, SqlWriterType.MySql, "SIGN(-1.23)");
            AssertSql(c, SqlWriterType.Sqlite, "SIGN(-1.23)");
        }

        [Test]
        public void Space()
        {
            var c = ParseColumn("SPACE(5)");

            AssertSql(c, SqlWriterType.SqlServer, "SPACE(5)");
            AssertSql(c, SqlWriterType.Oracle, "RPAD('', 5, ' ')");
            AssertSql(c, SqlWriterType.MySql, "SPACE(5)");
            AssertSql(c, SqlWriterType.Sqlite, "REPLACE(PRINTF('%.' || (5) || 'c', '/'),'/', ' ')");
        }

        [Test]
        public void Substring()
        {
            var c = ParseColumn("SUBSTRING('.bcd..', 2, 3)");

            AssertSql(c, SqlWriterType.SqlServer, "SUBSTRING('.bcd..', 2, 3)");
            AssertSql(c, SqlWriterType.Oracle, "SUBSTR('.bcd..', 2, 3)");
            AssertSql(c, SqlWriterType.MySql, "SUBSTR('.bcd..', 2, 3)");
            AssertSql(c, SqlWriterType.Sqlite, "SUBSTR('.bcd..', 2, 3)");
        }

        [Test]
        public void ToChar()
        {
            var c = ParseColumn("TOCHAR(123)");

            AssertSql(c, SqlWriterType.SqlServer, "STR(123)");
            AssertSql(c, SqlWriterType.Oracle, "TO_CHAR(123)");
            AssertSql(c, SqlWriterType.MySql, "CONVERT(123, NCHAR)");
            AssertSql(c, SqlWriterType.Sqlite, "CAST(123 AS NVARCHAR)");
        }

        [Test]
        public void ToTimestamp()
        {
            var c = ParseColumn("TOTIMESTAMP('2020-12-31 23:59.59.123')");

            AssertSql(c, SqlWriterType.SqlServer, "CONVERT(datetime, '2020-12-31 23:59.59.123')");
            AssertSql(c, SqlWriterType.Oracle, "TO_DATE('2020-12-31 23:59.59.123')");
            AssertSql(c, SqlWriterType.MySql, "CONVERT('2020-12-31 23:59.59.123', DATETIME)");
            AssertSql(c, SqlWriterType.Sqlite, "DATETIME('2020-12-31 23:59.59.123')");
        }

        [Test]
        public void ToNumber()
        {
            var c = ParseColumn("TONUMBER('123')");

            AssertSql(c, SqlWriterType.SqlServer, "CONVERT(real, '123')");
            AssertSql(c, SqlWriterType.Oracle, "TO_NUMBER('123')");
            AssertSql(c, SqlWriterType.MySql, "CONVERT('123', DECIMAL)");
            AssertSql(c, SqlWriterType.Sqlite, "CAST('123' AS REAL)");
        }

        [Test]
        public void Trim()
        {
            var c = ParseColumn("TRIM('abc')");

            AssertSql(c, SqlWriterType.SqlServer, "TRIM('abc')");
            AssertSql(c, SqlWriterType.Oracle, "TRIM('abc')");
            AssertSql(c, SqlWriterType.MySql, "TRIM('abc')");
            AssertSql(c, SqlWriterType.Sqlite, "TRIM('abc')");
        }

        [Test]
        public void TruncateDate()
        {
            var c = ParseColumn("TRUNCATEDATE({ts '2000-12-31 23:59:59.123'})");

            AssertSql(c, SqlWriterType.SqlServer, "CONVERT(date, {ts '2000-12-31 23:59:59.123'})");
            AssertSql(c, SqlWriterType.Oracle, "TRUNC(TIMESTAMP '2000-12-31 23:59:59.123')");
            AssertSql(c, SqlWriterType.MySql, "DATE(TIMESTAMP('2000-12-31  23:59:59.123'))");
            AssertSql(c, SqlWriterType.Sqlite, "DATE(DATETIME('2000-12-31 23:59:59'))");
        }

        [Test]
        public void Upper()
        {
            var c = ParseColumn("UPPER('abc')");

            AssertSql(c, SqlWriterType.SqlServer, "UPPER('abc')");
            AssertSql(c, SqlWriterType.Oracle, "UPPER('abc')");
            AssertSql(c, SqlWriterType.MySql, "UPPER('abc')");
            AssertSql(c, SqlWriterType.Sqlite, "UPPER('abc')");
        }

        [Test]
        public void Year()
        {
            var c = ParseColumn("YEAR({d '2000-12-31'})");

            AssertSql(c, SqlWriterType.SqlServer, "YEAR({d '2000-12-31'})");
            AssertSql(c, SqlWriterType.Oracle, "EXTRACT(year FROM DATE '2000-12-31')");
            AssertSql(c, SqlWriterType.MySql, "YEAR(DATE('2000-12-31'))");
            AssertSql(c, SqlWriterType.Sqlite, "CAST(STRFTIME('%Y', DATE('2000-12-31')) AS INT)");
        }

        [Test]
        public void YearsBetween()
        {
            var c = ParseColumn("YEARSBETWEEN({d '2000-12-31'}, {d '2001-12-31'})");

            AssertSql(c, SqlWriterType.SqlServer, "DATEDIFF(y, {d '2000-12-31'}, {d '2001-12-31'})");
            AssertSql(c, SqlWriterType.Oracle, "FLOOR(MONTHS_BETWEEN(DATE '2001-12-31', DATE '2000-12-31') / 12)");
            AssertSql(c, SqlWriterType.MySql, "TIMESTAMPDIFF(YEAR, DATE('2000-12-31'), DATE('2001-12-31'))");
            AssertSql(c, SqlWriterType.Sqlite, "EXCEPTION: YEARSBETWEEN function not available on Sqlite");
        }
    }
}
