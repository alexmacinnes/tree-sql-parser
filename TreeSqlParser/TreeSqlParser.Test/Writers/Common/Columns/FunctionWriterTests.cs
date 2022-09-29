using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    class FunctionWriterTests
    {
        private Column ParseColumn(string sql) =>
            (Column) SelectParser.ParseColumn(sql).Child;


        [Test]
        public void Abs()
        {
            var c = ParseColumn("ABS(-1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("ABS(-1.23)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void AddDays()
        {
            var c = ParseColumn("ADDDAYS({d '2000-12-31'}, 365)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "DATEADD(d, 365, {d '2000-12-31'})")
                .WithSql(SqlWriterType.Oracle, "(DATE '2000-12-31' + 365)")
                .WithSql(SqlWriterType.MySql, "ADDDATE(DATE('2000-12-31'), INTERVAL (365) DAY)")
                .WithSql(SqlWriterType.MariaDb, "ADDDATE(DATE('2000-12-31'), INTERVAL (365) DAY)")
                .WithSql(SqlWriterType.Sqlite, "DATETIME(DATE('2000-12-31'), ''||(365)||' DAYS')")
                .WithSql(SqlWriterType.Postgres, "DATE '2000-12-31' + (INTERVAL '1d' * 365)")
                .WithSql(SqlWriterType.Db2, "ADD_DAYS(DATE '2000-12-31', 365)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void AddMonths()
        {
            var c = ParseColumn("ADDMONTHS({d '2000-12-31'}, 3)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "DATEADD(m, 3, {d '2000-12-31'})")
                .WithSql(SqlWriterType.Oracle, "ADD_MONTHS(DATE '2000-12-31', 3)")
                .WithSql(SqlWriterType.MySql, "ADDDATE(DATE('2000-12-31'), INTERVAL (3) MONTH)")
                .WithSql(SqlWriterType.MariaDb, "ADDDATE(DATE('2000-12-31'), INTERVAL (3) MONTH)")
                .WithSql(SqlWriterType.Sqlite, "DATETIME(DATE('2000-12-31'), ''||(3)||' MONTHS')")
                .WithSql(SqlWriterType.Postgres, "DATE '2000-12-31' + (INTERVAL '1 month' * 3)")
                .WithSql(SqlWriterType.Db2, "ADD_MONTHS(DATE '2000-12-31', 3)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void AddYears()
        {
            var c = ParseColumn("ADDYEARS({d '2000-12-31'}, 3)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "DATEADD(y, 3, {d '2000-12-31'})")
                .WithSql(SqlWriterType.Oracle, "ADD_MONTHS(DATE '2000-12-31', 12 * (3)")
                .WithSql(SqlWriterType.MySql, "ADDDATE(DATE('2000-12-31'), INTERVAL (3) YEAR)")
                .WithSql(SqlWriterType.MariaDb, "ADDDATE(DATE('2000-12-31'), INTERVAL (3) YEAR)")
                .WithSql(SqlWriterType.Sqlite, "DATETIME(DATE('2000-12-31'), ''||(3)||' YEARS')")
                .WithSql(SqlWriterType.Postgres, "DATE '2000-12-31' + (INTERVAL '1y' * 3)")
                .WithSql(SqlWriterType.Db2, "ADD_YEARS(DATE '2000-12-31', 3)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Ceiling()
        {
            var c = ParseColumn("CEILING(1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("CEIL(1.23)")
                .WithSql(SqlWriterType.SqlServer, "CEILING(1.23)")
                .WithSql(SqlWriterType.Sqlite, "(CAST(1.23 AS INT) + (1.23 > CAST(1.23 AS INT)))")
                .WithSql(SqlWriterType.Postgres, "CEILING(1.23)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CharIndex()
        {
            var c = ParseColumn("CHARINDEX('d', 'abcde')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("INSTR('abcde', 'd')")
                .WithSql(SqlWriterType.SqlServer, "CHARINDEX('d', 'abcde')")
                .WithSql(SqlWriterType.Postgres, "STRPOS('abcde', 'd')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CharIndexWithStartIndex()
        {
            var c = ParseColumn("CHARINDEX('d', 'abcde', 2)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "CHARINDEX('d', 'abcde', 2)")
                .WithSql(SqlWriterType.Oracle, "INSTR('abcde', 'd', 2)")
                .WithSql(SqlWriterType.MySql, "IFNULL((NULLIF(INSTR(SUBSTR('abcde', 2), 'd'), 0)) + 2 - 1, 0)")
                .WithSql(SqlWriterType.MariaDb, "IFNULL((NULLIF(INSTR(SUBSTR('abcde', 2), 'd'), 0)) + 2 - 1, 0)")
                .WithSql(SqlWriterType.Sqlite, "IFNULL((NULLIF(INSTR(SUBSTR('abcde', 2), 'd'), 0)) + 2 - 1, 0)")
                .WithSql(SqlWriterType.Postgres, "COALESCE((NULLIF(STRPOS(SUBSTR('abcde', 2), 'd'), 0)) + 2 - 1, 0)")
                .WithSql(SqlWriterType.Db2, "INSTR('abcde', 'd', 2)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Choose()
        {
            var c = ParseColumn("CHOOSE(2, 'one', 'two', 'three')");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "CHOOSE(2, 'one', 'two', 'three')")
                .WithSql(SqlWriterType.Oracle, "DECODE(2, 1, 'one', 2, 'two', 3, 'three')")
                .WithSql(SqlWriterType.MySql, "ELT(2, 'one', 'two', 'three')")
                .WithSql(SqlWriterType.MariaDb, "ELT(2, 'one', 'two', 'three')")
                .WithSql(SqlWriterType.Sqlite, "CASE  WHEN 2 = 1 THEN 'one' WHEN 2 = 2 THEN 'two' WHEN 2 = 3 THEN 'three' END")
                .WithSql(SqlWriterType.Postgres, "CASE  WHEN 2 = 1 THEN 'one' WHEN 2 = 2 THEN 'two' WHEN 2 = 3 THEN 'three' END")
                .WithSql(SqlWriterType.Db2, "DECODE(2, 1, 'one', 2, 'two', 3, 'three')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Coalesce()
        {
            var c = ParseColumn("COALESCE(1, 2, 3)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("COALESCE(1, 2, 3)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Concat()
        {
            var c = ParseColumn("CONCAT('a','b','c')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("'a' || 'b' || 'c'")
                .WithSql(SqlWriterType.SqlServer, "CONCAT('a', 'b', 'c')")
                .WithSql(SqlWriterType.MySql, "CONCAT('a', 'b', 'c')")
                .WithSql(SqlWriterType.MariaDb, "CONCAT('a', 'b', 'c')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ConcatWithSeperator()
        {
            var c = ParseColumn("CONCAT_WS('___', 'a','b','c')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("'a' || '___' || 'b' || '___' || 'c'")
                .WithSql(SqlWriterType.SqlServer, "CONCAT_WS('___', 'a', 'b', 'c')")
                .WithSql(SqlWriterType.MySql, "CONCAT_WS('___', 'a', 'b', 'c')")
                .WithSql(SqlWriterType.MariaDb, "CONCAT_WS('___', 'a', 'b', 'c')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DateFromParts()
        {
            var c = ParseColumn("DATEFROMPARTS(2020, 12, 31)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "DATEFROMPARTS(2020, 12, 31)")
                .WithSql(SqlWriterType.Oracle, "TO_DATE((2020) || (12) || (31), 'yyyyMMdd')")
                .WithSql(SqlWriterType.MySql, "DATE(CONCAT_WS('-', 2020, 12, 31)))")
                .WithSql(SqlWriterType.MariaDb, "DATE(CONCAT_WS('-', 2020, 12, 31)))")
                .WithSql(SqlWriterType.Sqlite, "DATE(SUBSTR('0000' || 2020, -4, 4) || '-' || SUBSTR('00' || 12, -2, 2) || '-' || SUBSTR('00' || 31, -2, 2))")
                .WithSql(SqlWriterType.Postgres, "MAKE_DATE(2020, 12, 31)")
                .WithSql(SqlWriterType.Db2, "DATE(2020 || '-' || 12  || '-' || 31 )");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Day()
        {
            var c = ParseColumn("DAY({d '2000-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "DAY({d '2000-12-31'})")
                .WithSql(SqlWriterType.Oracle, "EXTRACT(day FROM DATE '2000-12-31')")
                .WithSql(SqlWriterType.MySql, "DAY(DATE('2000-12-31'))")
                .WithSql(SqlWriterType.MariaDb, "DAY(DATE('2000-12-31'))")
                .WithSql(SqlWriterType.Sqlite, "CAST(STRFTIME('%d', DATE('2000-12-31')) AS INT)")
                .WithSql(SqlWriterType.Postgres, "EXTRACT(DAY FROM DATE '2000-12-31')")
                .WithSql(SqlWriterType.Db2, "DAYOFMONTH(DATE '2000-12-31')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DaysBetween()
        {
            var c = ParseColumn("DAYSBETWEEN({d '2000-12-31'}, {d '2001-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "DATEDIFF(d, {d '2000-12-31'}, {d '2001-12-31'})")
                .WithSql(SqlWriterType.Oracle, "(DATE '2001-12-31' - DATE '2000-12-31')")
                .WithSql(SqlWriterType.MySql, "DATEDIFF(DATE('2001-12-31'), DATE('2000-12-31'))")
                .WithSql(SqlWriterType.MariaDb, "DATEDIFF(DATE('2001-12-31'), DATE('2000-12-31'))")
                .WithSql(SqlWriterType.Sqlite, "CAST(JULIANDAY(DATE('2001-12-31') AS INT)) - CAST(JULIANDAY(DATE('2000-12-31') AS INT))")
                .WithSql(SqlWriterType.Postgres, "((DATE '2001-12-31')::date - (DATE '2000-12-31')::date)")
                .WithSql(SqlWriterType.Db2, "DAYS_BETWEEN(DATE '2001-12-31', DATE '2000-12-31')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Exp()
        {
            var c = ParseColumn("EXP(1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("EXP(1.23)")
                .WithSql(SqlWriterType.Sqlite, "EXCEPTION: EXP function not available on Sqlite");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Floor()
        {
            var c = ParseColumn("FLOOR(1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("FLOOR(1.23)")
                .WithSql(SqlWriterType.Sqlite, "(CAST(1.23 AS INT) - (1.23 < CAST(1.23 AS INT)))");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void GetDate()
        {
            var c = ParseColumn("GETDATE()");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "CONVERT(date, GETDATE())")
                .WithSql(SqlWriterType.Oracle, "CURRENT_DATE")
                .WithSql(SqlWriterType.MySql, "DATE(CURRENT_DATE())")
                .WithSql(SqlWriterType.MariaDb, "DATE(CURRENT_DATE())")
                .WithSql(SqlWriterType.Sqlite, "DATE()")
                .WithSql(SqlWriterType.Postgres, "CURRENT_DATE")
                .WithSql(SqlWriterType.Db2, "(CURRENT DATE)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void GetTimestamp()
        {
            var c = ParseColumn("GETTIMESTAMP()");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "GETDATE()")
                .WithSql(SqlWriterType.Oracle, "CURRENT_TIMESTAMP")
                .WithSql(SqlWriterType.MySql, "TIMESTAMP(CURRENT_TIMESTAMP())")
                .WithSql(SqlWriterType.MariaDb, "TIMESTAMP(CURRENT_TIMESTAMP())")
                .WithSql(SqlWriterType.Sqlite, "DATETIME()")
                .WithSql(SqlWriterType.Postgres, "CURRENT_TIMESTAMP")
                .WithSql(SqlWriterType.Db2, "(CURRENT TIMESTAMP)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void IsNull()
        {
            var c = ParseColumn("ISNULL(x.y, 0)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "ISNULL([x].[y], 0)")
                .WithSql(SqlWriterType.Oracle, "NVL(\"x\".\"y\", 0)")
                .WithSql(SqlWriterType.MySql, "ISNULL(`x`.`y`, 0)")
                .WithSql(SqlWriterType.MariaDb, "ISNULL(`x`.`y`, 0)")
                .WithSql(SqlWriterType.Sqlite, "IFNULL([x].[y], 0)")
                .WithSql(SqlWriterType.Postgres, "COALESCE(\"x\".\"y\", 0)")
                .WithSql(SqlWriterType.Db2, "NVL(\"x\".\"y\", 0)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Left()
        {
            var c = ParseColumn("LEFT('abcde', 2)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LEFT('abcde', 2)")
                .WithSql(SqlWriterType.Oracle, "SUBSTR('abcde', 1, 2)")
                .WithSql(SqlWriterType.Sqlite, "SUBSTR('abcde', 1, 2)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Len()
        {
            var c = ParseColumn("LEN('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LENGTH('abc')")
                .WithSql(SqlWriterType.SqlServer, "LEN('abc')")
                .WithSql(SqlWriterType.Oracle, "LENGTH(RTRIM('abc'))");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void LogDefaultBase()
        {
            var c = ParseColumn("LOG(1000)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LOG(1000)")
                .WithSql(SqlWriterType.Sqlite, "EXCEPTION: LOG function not available on Sqlite")
                .WithSql(SqlWriterType.Db2, "LN(1000)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void LogBase10()
        {
            var c = ParseColumn("LOG(1000, 10)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "LOG(1000, 10)")
                .WithSql(SqlWriterType.Oracle, "LOG(10, 1000)")
                .WithSql(SqlWriterType.MySql, "LOG(10, 1000)")
                .WithSql(SqlWriterType.MariaDb, "LOG(10, 1000)")
                .WithSql(SqlWriterType.Sqlite, "EXCEPTION: LOG function not available on Sqlite")
                .WithSql(SqlWriterType.Postgres, "LOG(1000, 10)")
                .WithSql(SqlWriterType.Db2, "LOG10(1000)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Lower()
        {
            var c = ParseColumn("LOWER('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LOWER('abc')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void LTrim()
        {
            var c = ParseColumn("LTRIM('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LTRIM('abc')"); 

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Month()
        {
            var c = ParseColumn("MONTH({d '2000-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "MONTH({d '2000-12-31'})")
                .WithSql(SqlWriterType.Oracle, "EXTRACT(month FROM DATE '2000-12-31')")
                .WithSql(SqlWriterType.MySql, "MONTH(DATE('2000-12-31'))")
                .WithSql(SqlWriterType.MariaDb, "MONTH(DATE('2000-12-31'))")
                .WithSql(SqlWriterType.Sqlite, "CAST(STRFTIME('%m', DATE('2000-12-31')) AS INT)")
                .WithSql(SqlWriterType.Postgres, "EXTRACT(MONTH FROM DATE '2000-12-31')")
                .WithSql(SqlWriterType.Db2, "MONTH(DATE '2000-12-31')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void MonthsBetween()
        {
            var c = ParseColumn("MONTHSBETWEEN({d '2000-12-30'}, {d '2001-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "DATEDIFF(m, {d '2000-12-30'}, {d '2001-12-31'})")
                .WithSql(SqlWriterType.Oracle, "FLOOR(MONTHS_BETWEEN(DATE '2001-12-31', DATE '2000-12-30'))")
                .WithSql(SqlWriterType.MySql, "TIMESTAMPDIFF(MONTH, DATE('2000-12-30'), DATE('2001-12-31'))")
                .WithSql(SqlWriterType.MariaDb, "TIMESTAMPDIFF(MONTH, DATE('2000-12-30'), DATE('2001-12-31'))")
                .WithSql(SqlWriterType.Sqlite, "EXCEPTION: MONTHSBETWEEN function not available on Sqlite")
                .WithSql(SqlWriterType.Postgres, "(12*(EXTRACT(YEAR FROM DATE '2001-12-31') - EXTRACT(YEAR FROM DATE '2000-12-30'))) + (EXTRACT(MONTH FROM DATE '2001-12-31') - EXTRACT(MONTH FROM DATE '2000-12-30'))")
                .WithSql(SqlWriterType.Db2, "(12*((YEAR(DATE '2001-12-31') - YEAR(DATE '2000-12-30')))) + (MONTH(DATE '2001-12-31') - MONTH(DATE '2000-12-30'))");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void NullIf()
        {
            var c = ParseColumn("NULLIF('abc', 'N/A')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("NULLIF('abc', 'N/A')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Power()
        {
            var c = ParseColumn("POWER(10, 2)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("POWER(10, 2)")
                .WithSql(SqlWriterType.Oracle, "POW(10, 2)")
                .WithSql(SqlWriterType.MySql, "POW(10, 2)")
                .WithSql(SqlWriterType.MariaDb, "POW(10, 2)")
                .WithSql(SqlWriterType.Sqlite, "EXCEPTION: POWER function not available on Sqlite");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Replace()
        {
            var c = ParseColumn("REPLACE('abCde', 'C', 'c')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("REPLACE('abCde', 'C', 'c')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Replicate()
        {
            var c = ParseColumn("REPLICATE('abc', 5)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("REPEAT('abc', 5)")
                .WithSql(SqlWriterType.SqlServer, "REPLICATE('abc', 5)")
                .WithSql(SqlWriterType.Oracle, "RPAD('', LENGTH(RTRIM('abc')) * 5, 'abc')")
                .WithSql(SqlWriterType.Sqlite, "REPLACE(PRINTF('%.' || (5) || 'c', '/'),'/', 'abc')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Reverse()
        {
            var c = ParseColumn("REVERSE('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("REVERSE('abc')")
                .WithSql(SqlWriterType.Sqlite, "EXCEPTION: REVERSE function not available on Sqlite");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void RightFixedWidth()
        {
            var c = ParseColumn("RIGHT('abcde', 2)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("RIGHT('abcde', 2)")
                .WithSql(SqlWriterType.Oracle, "SUBSTR('abcde', -2, 2)")
                .WithSql(SqlWriterType.Sqlite, "SUBSTR('abcde', -2, 2)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void RightVariableWidth()
        {
            var c = ParseColumn("RIGHT('abcde', 2*3)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("RIGHT('abcde', 2*3)")
                .WithSql(SqlWriterType.Oracle, "SUBSTR('abcde', 0 - (2*3), 2*3)")
                .WithSql(SqlWriterType.Sqlite, "SUBSTR('abcde', 0 - (2*3), 2*3)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Round()
        {
            var c = ParseColumn("ROUND(12.34, -1)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("ROUND(12.34, -1)"); 

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void RTrim()
        {
            var c = ParseColumn("RTRIM('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("RTRIM('abc')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Sign()
        {
            var c = ParseColumn("SIGN(-1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SIGN(-1.23)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Space()
        {
            var c = ParseColumn("SPACE(5)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SPACE(5)")
                .WithSql(SqlWriterType.Oracle, "RPAD('', 5, ' ')")
                .WithSql(SqlWriterType.Sqlite, "REPLACE(PRINTF('%.' || (5) || 'c', '/'),'/', ' ')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Substring()
        {
            var c = ParseColumn("SUBSTRING('.bcd..', 2, 3)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SUBSTR('.bcd..', 2, 3)")
                .WithSql(SqlWriterType.SqlServer, "SUBSTRING('.bcd..', 2, 3)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ToChar()
        {
            var c = ParseColumn("TOCHAR(123)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "STR(123)")
                .WithSql(SqlWriterType.Oracle, "TO_CHAR(123)")
                .WithSql(SqlWriterType.MySql, "CONVERT(123, NCHAR)")
                .WithSql(SqlWriterType.MariaDb, "CONVERT(123, NCHAR)")
                .WithSql(SqlWriterType.Sqlite, "CAST(123 AS NVARCHAR)")
                .WithSql(SqlWriterType.Postgres, "(123)::varchar")
                .WithSql(SqlWriterType.Db2, "NVARCHAR(123)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ToTimestamp()
        {
            var c = ParseColumn("TOTIMESTAMP('2020-12-31 23:59.59.123')");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "CONVERT(datetime, '2020-12-31 23:59.59.123')")
                .WithSql(SqlWriterType.Oracle, "TO_DATE('2020-12-31 23:59.59.123')")
                .WithSql(SqlWriterType.MySql, "CONVERT('2020-12-31 23:59.59.123', DATETIME)")
                .WithSql(SqlWriterType.MariaDb, "CONVERT('2020-12-31 23:59.59.123', DATETIME)")
                .WithSql(SqlWriterType.Sqlite, "DATETIME('2020-12-31 23:59.59.123')")
                .WithSql(SqlWriterType.Postgres, "('2020-12-31 23:59.59.123')::timestamp")
                .WithSql(SqlWriterType.Db2, "TIMESTAMP('2020-12-31 23:59.59.123')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ToNumber()
        {
            var c = ParseColumn("TONUMBER('123')");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "CONVERT(real, '123')")
                .WithSql(SqlWriterType.Oracle, "TO_NUMBER('123')")
                .WithSql(SqlWriterType.MySql, "CONVERT('123', DECIMAL)")
                .WithSql(SqlWriterType.MariaDb, "CONVERT('123', DECIMAL)")
                .WithSql(SqlWriterType.Sqlite, "CAST('123' AS REAL)")
                .WithSql(SqlWriterType.Postgres, "('123')::decimal")
                .WithSql(SqlWriterType.Db2, "DOUBLE('123')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Trim()
        {
            var c = ParseColumn("TRIM('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("TRIM('abc')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void TruncateDate()
        {
            var c = ParseColumn("TRUNCATEDATE({ts '2000-12-31 23:59:59.123'})");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "CONVERT(date, {ts '2000-12-31 23:59:59.123'})")
                .WithSql(SqlWriterType.Oracle, "TRUNC(TIMESTAMP '2000-12-31 23:59:59.123')")
                .WithSql(SqlWriterType.MySql, "DATE(TIMESTAMP('2000-12-31  23:59:59.123'))")
                .WithSql(SqlWriterType.MariaDb, "DATE(TIMESTAMP('2000-12-31  23:59:59.123'))")
                .WithSql(SqlWriterType.Sqlite, "DATE(DATETIME('2000-12-31 23:59:59'))")
                .WithSql(SqlWriterType.Postgres, "DATE(TIMESTAMP '2000-12-31 23:59:59.123')")
                .WithSql(SqlWriterType.Db2, "DATE(TIMESTAMP '2000-12-31 23:59:59.123')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Upper()
        {
            var c = ParseColumn("UPPER('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("UPPER('abc')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Year()
        {
            var c = ParseColumn("YEAR({d '2000-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "YEAR({d '2000-12-31'})")
                .WithSql(SqlWriterType.Oracle, "EXTRACT(year FROM DATE '2000-12-31')")
                .WithSql(SqlWriterType.MySql, "YEAR(DATE('2000-12-31'))")
                .WithSql(SqlWriterType.MariaDb, "YEAR(DATE('2000-12-31'))")
                .WithSql(SqlWriterType.Sqlite, "CAST(STRFTIME('%Y', DATE('2000-12-31')) AS INT)")
                .WithSql(SqlWriterType.Postgres, "EXTRACT(YEAR FROM DATE '2000-12-31')")
                .WithSql(SqlWriterType.Db2, "YEAR(DATE '2000-12-31')");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void YearsBetween()
        {
            var c = ParseColumn("YEARSBETWEEN({d '2000-12-31'}, {d '2001-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "DATEDIFF(year, {d '2000-12-31'}, {d '2001-12-31'})")
                .WithSql(SqlWriterType.Oracle, "FLOOR(MONTHS_BETWEEN(DATE '2001-12-31', DATE '2000-12-31') / 12)")
                .WithSql(SqlWriterType.MySql, "TIMESTAMPDIFF(YEAR, DATE('2000-12-31'), DATE('2001-12-31'))")
                .WithSql(SqlWriterType.MariaDb, "TIMESTAMPDIFF(YEAR, DATE('2000-12-31'), DATE('2001-12-31'))")
                .WithSql(SqlWriterType.Sqlite, "EXCEPTION: YEARSBETWEEN function not available on Sqlite")
                .WithSql(SqlWriterType.Postgres, "(EXTRACT(YEAR FROM DATE '2001-12-31') - EXTRACT(YEAR FROM DATE '2000-12-31'))")
                .WithSql(SqlWriterType.Db2, "(YEAR(DATE '2001-12-31') - YEAR(DATE '2000-12-31'))");

            CommonMother.AssertSql(c, expected);
        }
    }
}
