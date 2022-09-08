using NUnit.Framework;
using System;
using System.Collections.Generic;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Writers.Common.MySql;
using TreeSqlParser.Writers.Common.Oracle;
using TreeSqlParser.Writers.Common.Sqlite;
using TreeSqlParser.Writers.Common.SqlServer;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    class FunctionWriterTests
    {
        private readonly IReadOnlyDictionary<DbFamily, ISqlWriter> writers = new Dictionary<DbFamily, ISqlWriter>
        {
            { DbFamily.SqlServer, new CommonSqlServerSqlWriter() },
            { DbFamily.Oracle, new CommonOracleSqlWriter() },
            { DbFamily.MySql, new CommonMySqlSqlWriter() },
            { DbFamily.Sqlite, new CommonSqliteSqlWriter() },
        };

        private Column ParseColumn(string sql) =>
            SelectParser.ParseColumn(sql);

        private string Sql(Column c, DbFamily db)
        {
            try
            {
                return writers[db].GenerateSql(c);
            }
            catch (Exception x)
            {
                return "EXCEPTION: " + x.Message;
            }
        }

        private void AssertSql(Column c, DbFamily db, string expected) =>
            Assert.AreEqual(expected, Sql(c, db));

        [Test]
        public void Abs()
        {
            var c = ParseColumn("ABS(-1.23)");

            AssertSql(c, DbFamily.SqlServer, "ABS(-1.23)");
            AssertSql(c, DbFamily.Oracle, "ABS(-1.23)");
            AssertSql(c, DbFamily.MySql, "ABS(-1.23)");
            AssertSql(c, DbFamily.Sqlite, "ABS(-1.23)");
        }

        [Test]
        public void AddDays()
        {
            var c = ParseColumn("ADDDAYS({d '2000-12-31'}, 365)");

            AssertSql(c, DbFamily.SqlServer, "DATEADD(d, 365, {d '2000-12-31'})");
            AssertSql(c, DbFamily.Oracle, "(DATE '2000-12-31' + 365)");
            AssertSql(c, DbFamily.MySql, "ADDDATE(DATE('2000-12-31'), INTERVAL (365) DAY)");
            AssertSql(c, DbFamily.Sqlite, "DATETIME(DATE('2000-12-31'), ''||(365)||' DAYS')");
        }

        [Test]
        public void AddMonths()
        {
            var c = ParseColumn("ADDMONTHS({d '2000-12-31'}, 3)");

            AssertSql(c, DbFamily.SqlServer, "DATEADD(m, 3, {d '2000-12-31'})");
            AssertSql(c, DbFamily.Oracle, "ADD_MONTHS(DATE '2000-12-31', 3)");
            AssertSql(c, DbFamily.MySql, "ADDDATE(DATE('2000-12-31'), INTERVAL (3) MONTH)");
            AssertSql(c, DbFamily.Sqlite, "DATETIME(DATE('2000-12-31'), ''||(3)||' MONTHS')");
        }

        [Test]
        public void AddYears()
        {
            var c = ParseColumn("ADDYEARS({d '2000-12-31'}, 3)");

            AssertSql(c, DbFamily.SqlServer, "DATEADD(y, 3, {d '2000-12-31'})");
            AssertSql(c, DbFamily.Oracle, "ADD_MONTHS(DATE '2000-12-31', 12 * (3)");
            AssertSql(c, DbFamily.MySql, "ADDDATE(DATE('2000-12-31'), INTERVAL (3) YEAR)");
            AssertSql(c, DbFamily.Sqlite, "DATETIME(DATE('2000-12-31'), ''||(3)||' YEARS')");
        }

        [Test]
        public void Ceiling()
        {
            var c = ParseColumn("CEILING(1.23)");

            AssertSql(c, DbFamily.SqlServer, "CEILING(1.23)");
            AssertSql(c, DbFamily.Oracle, "CEIL(1.23)");
            AssertSql(c, DbFamily.MySql, "CEIL(1.23)");
            AssertSql(c, DbFamily.Sqlite, "(CAST(1.23 AS INT) + (1.23 > CAST(1.23 AS INT)))");
        }

        [Test]
        public void CharIndex()
        {
            var c = ParseColumn("CHARINDEX('abcde', 'd')");

            AssertSql(c, DbFamily.SqlServer, "CHARINDEX('abcde', 'd')");
            AssertSql(c, DbFamily.Oracle, "INSTR('d', 'abcde')");
            AssertSql(c, DbFamily.MySql, "INSTR('d', 'abcde')");
            AssertSql(c, DbFamily.Sqlite, "INSTR('d', 'abcde')");
        }

        [Test]
        public void CharIndexWithStartIndex()
        {
            var c = ParseColumn("CHARINDEX('abcde', 'd', 2)");

            AssertSql(c, DbFamily.SqlServer, "CHARINDEX('abcde', 'd', 2)");
            AssertSql(c, DbFamily.Oracle, "INSTR('d', 'abcde', 2)");
            AssertSql(c, DbFamily.MySql, "IFNULL((NULLIF(INSTR(SUBSTR('d', 2, 'abcde'), 0)) + 2 - 1, 0)");
            AssertSql(c, DbFamily.Sqlite, "IFNULL((NULLIF(INSTR(SUBSTR('d', 2, 'abcde'), 0)) + 2 - 1), 0)");
        }

        [Test]
        public void Choose()
        {
            var c = ParseColumn("CHOOSE(2, 'one', 'two', 'three')");

            AssertSql(c, DbFamily.SqlServer, "CHOOSE(2, 'one', 'two', 'three')");
            AssertSql(c, DbFamily.Oracle, "CASE WHEN 2 = 1 THEN 'one' WHEN 2 = 2 THEN 'two' WHEN 2 = 3 THEN 'three' END");
            AssertSql(c, DbFamily.MySql, "ELT(2, 'one', 'two', 'three')");
            AssertSql(c, DbFamily.Sqlite, "CASE  WHEN 2 = 1 THEN 'one' WHEN 2 = 2 THEN 'two' WHEN 2 = 3 THEN 'three' END");
        }

        [Test]
        public void Coalesce()
        {
            var c = ParseColumn("COALESCE(1, 2, 3)");

            AssertSql(c, DbFamily.SqlServer, "COALESCE(1, 2, 3)");
            AssertSql(c, DbFamily.Oracle, "COALESCE(1, 2, 3)");
            AssertSql(c, DbFamily.MySql, "COALESCE(1, 2, 3)");
            AssertSql(c, DbFamily.Sqlite, "COALESCE(1, 2, 3)");
        }

        [Test]
        public void Concat()
        {
            var c = ParseColumn("CONCAT('a','b','c')");

            AssertSql(c, DbFamily.SqlServer, "CONCAT('a', 'b', 'c')");
            AssertSql(c, DbFamily.Oracle, "'a' || 'b' || 'c'");
            AssertSql(c, DbFamily.MySql, "CONCAT('a', 'b', 'c')");
            AssertSql(c, DbFamily.Sqlite, "'a' || 'b' || 'c'");
        }

        [Test]
        public void ConcatWithSeperator()
        {
            var c = ParseColumn("CONCAT_WS('___', 'a','b','c')");

            AssertSql(c, DbFamily.SqlServer, "CONCAT_WS('___', 'a', 'b', 'c')");
            AssertSql(c, DbFamily.Oracle, "'a' || '___' || 'b' || '___' || 'c'");
            AssertSql(c, DbFamily.MySql, "CONCAT_WS('___', 'a', 'b', 'c')");
            AssertSql(c, DbFamily.Sqlite, "'a' || '___' || 'b' || '___' || 'c'");
        }

        [Test]
        public void DateFromParts()
        {
            var c = ParseColumn("DATEFROMPARTS(2020, 12, 31)");

            AssertSql(c, DbFamily.SqlServer, "DATEFROMPARTS(2020, 12, 31)");
            AssertSql(c, DbFamily.Oracle, "TO_DATE((2020) || (12) || (31), 'yyyyMMdd')");
            AssertSql(c, DbFamily.MySql, "DATE(CONCAT_WS('-', 2020, 12, 31)))");
            AssertSql(c, DbFamily.Sqlite, "DATE(SUBSTR('0000' || 2020, -4, 4) || '-' || SUBSTR('00' || 12, -2, 2) || '-' || SUBSTR('00' || 31, -2, 2))");
        }

        [Test]
        public void Day()
        {
            var c = ParseColumn("DAY({d '2000-12-31'})");

            AssertSql(c, DbFamily.SqlServer, "DAY({d '2000-12-31'})");
            AssertSql(c, DbFamily.Oracle, "EXTRACT(day FROM DATE '2000-12-31')");
            AssertSql(c, DbFamily.MySql, "DAY(DATE('2000-12-31'))");
            AssertSql(c, DbFamily.Sqlite, "CAST(STRFTIME('%d', DATE('2000-12-31')) AS INT)");
        }

        [Test]
        public void DaysBetween()
        {
            var c = ParseColumn("DAYSBETWEEN({d '2000-12-31'}, {d '2001-12-31'})");

            AssertSql(c, DbFamily.SqlServer, "DATEDIFF(d, {d '2000-12-31'}, {d '2001-12-31'})");
            AssertSql(c, DbFamily.Oracle, "(DATE '2001-12-31' - DATE '2000-12-31')");
            AssertSql(c, DbFamily.MySql, "DATEDIFF(DATE('2001-12-31'), DATE('2000-12-31'))");
            AssertSql(c, DbFamily.Sqlite, "CAST(JULIANDAY(DATE('2001-12-31') AS INT)) - CAST(JULIANDAY(DATE('2000-12-31') AS INT))");
        }

        [Test]
        public void Exp()
        {
            var c = ParseColumn("EXP(1.23)");

            AssertSql(c, DbFamily.SqlServer, "EXP(1.23)");
            AssertSql(c, DbFamily.Oracle, "EXP(1.23)");
            AssertSql(c, DbFamily.MySql, "EXP(1.23)");
            AssertSql(c, DbFamily.Sqlite, "EXCEPTION: EXP function not available on Sqlite");
        }

        [Test]
        public void Floor()
        {
            var c = ParseColumn("FLOOR(1.23)");

            AssertSql(c, DbFamily.SqlServer, "FLOOR(1.23)");
            AssertSql(c, DbFamily.Oracle, "FLOOR(1.23)");
            AssertSql(c, DbFamily.MySql, "FLOOR(1.23)");
            AssertSql(c, DbFamily.Sqlite, "(CAST(1.23 AS INT) - (1.23 < CAST(1.23 AS INT)))");

        }

        [Test]
        public void GetDate()
        {
            var c = ParseColumn("GETDATE()");

            AssertSql(c, DbFamily.SqlServer, "CONVERT(date, GETDATE())");
            AssertSql(c, DbFamily.Oracle, "CURRENT_DATE");
            AssertSql(c, DbFamily.MySql, "DATE(CURRENT_DATE())");
            AssertSql(c, DbFamily.Sqlite, "DATE()");
        }

        [Test]
        public void GetTimestamp()
        {
            var c = ParseColumn("GETTIMESTAMP()");

            AssertSql(c, DbFamily.SqlServer, "GETDATE()");
            AssertSql(c, DbFamily.Oracle, "CURRENT_TIMESTAMP");
            AssertSql(c, DbFamily.MySql, "TIMESTAMP(CURRENT_TIMESTAMP())");
            AssertSql(c, DbFamily.Sqlite, "DATETIME()");
        }

        [Test]
        public void IsNull()
        {
            var c = ParseColumn("ISNULL(x.y, 0)");

            AssertSql(c, DbFamily.SqlServer, "ISNULL([x].[y], 0)");
            AssertSql(c, DbFamily.Oracle, "NVL(\"x\".\"y\", 0)");
            AssertSql(c, DbFamily.MySql, "ISNULL(`x`.`y`, 0)");
            AssertSql(c, DbFamily.Sqlite, "IFNULL([x].[y], 0)");
        }

        [Test]
        public void Left()
        {
            var c = ParseColumn("LEFT('abcde', 2)");

            AssertSql(c, DbFamily.SqlServer, "LEFT('abcde', 2)");
            AssertSql(c, DbFamily.Oracle, "SUBSTR('abcde', 1, 2)");
            AssertSql(c, DbFamily.MySql, "LEFT('abcde', 2)");
            AssertSql(c, DbFamily.Sqlite, "SUBSTR('abcde', 1, 2)");
        }

        [Test]
        public void Len()
        {
            var c = ParseColumn("LEN('abc')");

            AssertSql(c, DbFamily.SqlServer, "LEN('abc')");
            AssertSql(c, DbFamily.Oracle, "LENGTH(RTRIM('abc'))");
            AssertSql(c, DbFamily.MySql, "LENGTH('abc')");
            AssertSql(c, DbFamily.Sqlite, "LENGTH('abc')");
        }

        [Test]
        public void LogDefaultBase()
        {
            var c = ParseColumn("LOG(1000)");

            AssertSql(c, DbFamily.SqlServer, "LOG(1000)");
            AssertSql(c, DbFamily.Oracle, "LOG(1000)");
            AssertSql(c, DbFamily.MySql, "LOG(1000)");
            AssertSql(c, DbFamily.Sqlite, "EXCEPTION: LOG function not available on Sqlite");
        }

        [Test]
        public void LogWithBase()
        {
            var c = ParseColumn("LOG(1000, 10)");

            AssertSql(c, DbFamily.SqlServer, "LOG(1000, 10)");
            AssertSql(c, DbFamily.Oracle, "LOG(10, 1000)");
            AssertSql(c, DbFamily.MySql, "LOG(10, 1000)");
            AssertSql(c, DbFamily.Sqlite, "EXCEPTION: LOG function not available on Sqlite");
        }

        [Test]
        public void Lower()
        {
            var c = ParseColumn("LOWER('abc')");

            AssertSql(c, DbFamily.SqlServer, "LOWER('abc')");
            AssertSql(c, DbFamily.Oracle, "LOWER('abc')");
            AssertSql(c, DbFamily.MySql, "LOWER('abc')");
            AssertSql(c, DbFamily.Sqlite, "LOWER('abc')");
        }

        [Test]
        public void LTrim()
        {
            var c = ParseColumn("LTRIM('abc')");

            AssertSql(c, DbFamily.SqlServer, "LTRIM('abc')");
            AssertSql(c, DbFamily.Oracle, "LTRIM('abc')");
            AssertSql(c, DbFamily.MySql, "LTRIM('abc')");
            AssertSql(c, DbFamily.Sqlite, "LTRIM('abc')");
        }

        [Test]
        public void Month()
        {
            var c = ParseColumn("MONTH({d '2000-12-31'})");

            AssertSql(c, DbFamily.SqlServer, "MONTH({d '2000-12-31'})");
            AssertSql(c, DbFamily.Oracle, "EXTRACT(month FROM DATE '2000-12-31')");
            AssertSql(c, DbFamily.MySql, "MONTH(DATE('2000-12-31'))");
            AssertSql(c, DbFamily.Sqlite, "CAST(STRFTIME('%m', DATE('2000-12-31')) AS INT)");
        }

        [Test]
        public void MonthsBetween()
        {
            var c = ParseColumn("MONTHSBETWEEN({d '2000-12-30'}, {d '2001-12-31'})");

            AssertSql(c, DbFamily.SqlServer, "DATEDIFF(m, {d '2000-12-30'}, {d '2001-12-31'})");
            AssertSql(c, DbFamily.Oracle, "FLOOR(MONTHS_BETWEEN(DATE '2001-12-31', DATE '2000-12-30'))");
            AssertSql(c, DbFamily.MySql, "TIMESTAMPDIFF(MONTH, DATE('2000-12-30'), DATE('2001-12-31'))");
            AssertSql(c, DbFamily.Sqlite, "EXCEPTION: MONTHSBETWEEN function not available on Sqlite");
        }

        [Test]
        public void NullIf()
        {
            var c = ParseColumn("NULLIF('abc', 'N/A')");

            AssertSql(c, DbFamily.SqlServer, "NULLIF('abc', 'N/A')");
            AssertSql(c, DbFamily.Oracle, "NULLIF('abc', 'N/A')");
            AssertSql(c, DbFamily.MySql, "NULLIF('abc', 'N/A')");
            AssertSql(c, DbFamily.Sqlite, "NULLIF('abc', 'N/A')");
        }

        [Test]
        public void Power()
        {
            var c = ParseColumn("POWER(10, 2)");

            AssertSql(c, DbFamily.SqlServer, "POWER(10, 2)");
            AssertSql(c, DbFamily.Oracle, "POW(10, 2)");
            AssertSql(c, DbFamily.MySql, "POW(10, 2)");
            AssertSql(c, DbFamily.Sqlite, "EXCEPTION: POWER function not available on Sqlite");
        }

        [Test]
        public void Replace()
        {
            var c = ParseColumn("REPLACE('abCde', 'C', 'c')");

            AssertSql(c, DbFamily.SqlServer, "REPLACE('abCde', 'C', 'c')");
            AssertSql(c, DbFamily.Oracle, "REPLACE('abCde', 'C', 'c')");
            AssertSql(c, DbFamily.MySql, "REPLACE('abCde', 'C', 'c')");
            AssertSql(c, DbFamily.Sqlite, "REPLACE('abCde', 'C', 'c')");
        }

        [Test]
        public void Replicate()
        {
            var c = ParseColumn("REPLICATE('abc', 5)");

            AssertSql(c, DbFamily.SqlServer, "REPLICATE('abc', 5)");
            AssertSql(c, DbFamily.Oracle, "RPAD('', LENGTH(RTRIM('abc')) * 5, 'abc')");
            AssertSql(c, DbFamily.MySql, "REPEAT('abc', 5)");
            AssertSql(c, DbFamily.Sqlite, "REPLACE(PRINTF('%.' || (5) || 'c', '/'),'/', 'abc')");
        }

        [Test]
        public void Reverse()
        {
            var c = ParseColumn("REVERSE('abc')");

            AssertSql(c, DbFamily.SqlServer, "REVERSE('abc')");
            AssertSql(c, DbFamily.Oracle, "REVERSE('abc')");
            AssertSql(c, DbFamily.MySql, "REVERSE('abc')");
            AssertSql(c, DbFamily.Sqlite, "EXCEPTION: REVERSE function not available on Sqlite");
        }

        [Test]
        public void RightFixedWidth()
        {
            var c = ParseColumn("RIGHT('abcde', 2)");

            AssertSql(c, DbFamily.SqlServer, "RIGHT('abcde', 2)");
            AssertSql(c, DbFamily.Oracle, "SUBSTR('abcde', -2, 2)");
            AssertSql(c, DbFamily.MySql, "RIGHT('abcde', 2)");
            AssertSql(c, DbFamily.Sqlite, "SUBSTR('abcde', -2, 2)");
        }

        [Test]
        public void RightVariableWidth()
        {
            var c = ParseColumn("RIGHT('abcde', 2*3)");

            AssertSql(c, DbFamily.SqlServer, "RIGHT('abcde', 2*3)");
            AssertSql(c, DbFamily.Oracle, "SUBSTR('abcde', 0 - (2*3), 2*3)");
            AssertSql(c, DbFamily.MySql, "RIGHT('abcde', 2*3)");
            AssertSql(c, DbFamily.Sqlite, "SUBSTR('abcde', 0 - (2*3), 2*3)");
        }

        [Test]
        public void Round()
        {
            var c = ParseColumn("ROUND(12.34, -1)");

            AssertSql(c, DbFamily.SqlServer, "ROUND(12.34, -1)");
            AssertSql(c, DbFamily.Oracle, "ROUND(12.34, -1)");
            AssertSql(c, DbFamily.MySql, "ROUND(12.34, -1)");
            AssertSql(c, DbFamily.Sqlite, "ROUND(12.34, -1)");
        }

        [Test]
        public void RTrim()
        {
            var c = ParseColumn("RTRIM('abc')");

            AssertSql(c, DbFamily.SqlServer, "RTRIM('abc')");
            AssertSql(c, DbFamily.Oracle, "RTRIM('abc')");
            AssertSql(c, DbFamily.MySql, "RTRIM('abc')");
            AssertSql(c, DbFamily.Sqlite, "RTRIM('abc')");
        }

        [Test]
        public void Sign()
        {
            var c = ParseColumn("SIGN(-1.23)");

            AssertSql(c, DbFamily.SqlServer, "SIGN(-1.23)");
            AssertSql(c, DbFamily.Oracle, "SIGN(-1.23)");
            AssertSql(c, DbFamily.MySql, "SIGN(-1.23)");
            AssertSql(c, DbFamily.Sqlite, "SIGN(-1.23)");
        }

        [Test]
        public void Space()
        {
            var c = ParseColumn("SPACE(5)");

            AssertSql(c, DbFamily.SqlServer, "SPACE(5)");
            AssertSql(c, DbFamily.Oracle, "RPAD('', 5, ' ')");
            AssertSql(c, DbFamily.MySql, "SPACE(5)");
            AssertSql(c, DbFamily.Sqlite, "REPLACE(PRINTF('%.' || (5) || 'c', '/'),'/', ' ')");
        }

        [Test]
        public void Substring()
        {
            var c = ParseColumn("SUBSTRING('.bcd..', 2, 3)");

            AssertSql(c, DbFamily.SqlServer, "SUBSTRING('.bcd..', 2, 3)");
            AssertSql(c, DbFamily.Oracle, "SUBSTR('.bcd..', 2, 3)");
            AssertSql(c, DbFamily.MySql, "SUBSTR('.bcd..', 2, 3)");
            AssertSql(c, DbFamily.Sqlite, "SUBSTR('.bcd..', 2, 3)");
        }

        [Test]
        public void ToChar()
        {
            var c = ParseColumn("TOCHAR(123)");

            AssertSql(c, DbFamily.SqlServer, "STR(123)");
            AssertSql(c, DbFamily.Oracle, "TO_CHAR(123)");
            AssertSql(c, DbFamily.MySql, "CONVERT(123, NCHAR)");
            AssertSql(c, DbFamily.Sqlite, "CAST(123 AS NVARCHAR)");
        }

        [Test]
        public void ToTimestamp()
        {
            var c = ParseColumn("TOTIMESTAMP('2020-12-31 23:59.59.123')");

            AssertSql(c, DbFamily.SqlServer, "CONVERT(datetime, '2020-12-31 23:59.59.123')");
            AssertSql(c, DbFamily.Oracle, "TO_DATE('2020-12-31 23:59.59.123')");
            AssertSql(c, DbFamily.MySql, "CONVERT('2020-12-31 23:59.59.123', DATETIME)");
            AssertSql(c, DbFamily.Sqlite, "DATETIME('2020-12-31 23:59.59.123')");
        }

        [Test]
        public void ToNumber()
        {
            var c = ParseColumn("TONUMBER('123')");

            AssertSql(c, DbFamily.SqlServer, "CONVERT(real, '123')");
            AssertSql(c, DbFamily.Oracle, "TO_NUMBER('123')");
            AssertSql(c, DbFamily.MySql, "CONVERT('123', DECIMAL)");
            AssertSql(c, DbFamily.Sqlite, "CAST('123' AS REAL)");
        }

        [Test]
        public void Trim()
        {
            var c = ParseColumn("TRIM('abc')");

            AssertSql(c, DbFamily.SqlServer, "TRIM('abc')");
            AssertSql(c, DbFamily.Oracle, "TRIM('abc')");
            AssertSql(c, DbFamily.MySql, "TRIM('abc')");
            AssertSql(c, DbFamily.Sqlite, "TRIM('abc')");
        }

        [Test]
        public void TruncateDate()
        {
            var c = ParseColumn("TRUNCATEDATE({ts '2000-12-31 23:59:59.123'})");

            AssertSql(c, DbFamily.SqlServer, "CONVERT(date, {ts '2000-12-31 23:59:59.123'})");
            AssertSql(c, DbFamily.Oracle, "TRUNC(TIMESTAMP '2000-12-31 23:59:59.123')");
            AssertSql(c, DbFamily.MySql, "DATE(TIMESTAMP('2000-12-31  23:59:59.123'))");
            AssertSql(c, DbFamily.Sqlite, "DATE(DATETIME('2000-12-31 23:59:59'))");
        }

        [Test]
        public void Upper()
        {
            var c = ParseColumn("UPPER('abc')");

            AssertSql(c, DbFamily.SqlServer, "UPPER('abc')");
            AssertSql(c, DbFamily.Oracle, "UPPER('abc')");
            AssertSql(c, DbFamily.MySql, "UPPER('abc')");
            AssertSql(c, DbFamily.Sqlite, "UPPER('abc')");
        }

        [Test]
        public void Year()
        {
            var c = ParseColumn("YEAR({d '2000-12-31'})");

            AssertSql(c, DbFamily.SqlServer, "YEAR({d '2000-12-31'})");
            AssertSql(c, DbFamily.Oracle, "EXTRACT(year FROM DATE '2000-12-31')");
            AssertSql(c, DbFamily.MySql, "YEAR(DATE('2000-12-31'))");
            AssertSql(c, DbFamily.Sqlite, "CAST(STRFTIME('%Y', DATE('2000-12-31')) AS INT)");
        }

        [Test]
        public void YearsBetween()
        {
            var c = ParseColumn("YEARSBETWEEN({d '2000-12-31'}, {d '2001-12-31'})");

            AssertSql(c, DbFamily.SqlServer, "DATEDIFF(y, {d '2000-12-31'}, {d '2001-12-31'})");
            AssertSql(c, DbFamily.Oracle, "FLOOR(MONTHS_BETWEEN(DATE '2001-12-31', DATE '2000-12-31') / 12)");
            AssertSql(c, DbFamily.MySql, "TIMESTAMPDIFF(YEAR, DATE('2000-12-31'), DATE('2001-12-31'))");
            AssertSql(c, DbFamily.Sqlite, "EXCEPTION: YEARSBETWEEN function not available on Sqlite");
        }
    }
}
