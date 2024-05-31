using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using swt = TreeSqlParser.Writers.SqlWriterType;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    class FunctionWriterTests
    {
        private static SelectParser SelectParser = new SelectParser();

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
                .WithSql("DATEADD(d, 365, {d '2000-12-31'})", swt.SqlServer)
                .WithSql("(DATE '2000-12-31' + 365)", swt.Oracle)
                .WithSql("ADDDATE(DATE('2000-12-31'), INTERVAL (365) DAY)", swt.MySql, swt.MariaDb)
                .WithSql("DATETIME(DATE('2000-12-31'), ''||(365)||' DAYS')", swt.Sqlite)
                .WithSql("DATE '2000-12-31' + (INTERVAL '1d' * 365)", swt.Postgres)
                .WithSql("ADD_DAYS(DATE '2000-12-31', 365)", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void AddMonths()
        {
            var c = ParseColumn("ADDMONTHS({d '2000-12-31'}, 3)");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEADD(m, 3, {d '2000-12-31'})", swt.SqlServer)
                .WithSql("ADD_MONTHS(DATE '2000-12-31', 3)", swt.Oracle, swt.Db2)
                .WithSql("ADDDATE(DATE('2000-12-31'), INTERVAL (3) MONTH)", swt.MySql, swt.MariaDb)
                .WithSql("DATETIME(DATE('2000-12-31'), ''||(3)||' MONTHS')", swt.Sqlite)
                .WithSql("DATE '2000-12-31' + (INTERVAL '1 month' * 3)", swt.Postgres);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void AddYears()
        {
            var c = ParseColumn("ADDYEARS({d '2000-12-31'}, 3)");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEADD(y, 3, {d '2000-12-31'})", swt.SqlServer)
                .WithSql("ADD_MONTHS(DATE '2000-12-31', 12 * (3))", swt.Oracle)
                .WithSql("ADDDATE(DATE('2000-12-31'), INTERVAL (3) YEAR)", swt.MySql, swt.MariaDb)
                .WithSql("DATETIME(DATE('2000-12-31'), ''||(3)||' YEARS')", swt.Sqlite)
                .WithSql("((DATE '2000-12-31') + (INTERVAL '1y' * 3))", swt.Postgres)
                .WithSql("ADD_YEARS(DATE '2000-12-31', 3)", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Ceiling()
        {
            var c = ParseColumn("CEILING(1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("CEIL(1.23)")
                .WithSql("CEILING(1.23)", swt.SqlServer, swt.Postgres)
                .WithSql("(CAST(1.23 AS INT) + (1.23 > CAST(1.23 AS INT)))", swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CharIndex()
        {
            var c = ParseColumn("CHARINDEX('d', 'abcde')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("INSTR('abcde', 'd')")
                .WithSql("CHARINDEX('d', 'abcde')", swt.SqlServer)
                .WithSql("STRPOS('abcde', 'd')", swt.Postgres);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CharIndexWithStartIndex()
        {
            var c = ParseColumn("CHARINDEX('d', 'abcde', 2)");

            var expected = new ExpectedSqlResult()
                .WithSql("CHARINDEX('d', 'abcde', 2)", swt.SqlServer)
                .WithSql("INSTR('abcde', 'd', 2)", swt.Oracle, swt.Db2)
                .WithSql("IFNULL((NULLIF(INSTR(SUBSTR('abcde', 2), 'd'), 0)) + 2 - 1, 0)", swt.MySql, swt.MariaDb, swt.Sqlite)
                .WithSql("COALESCE((NULLIF(STRPOS(SUBSTR('abcde', 2), 'd'), 0)) + 2 - 1, 0)", swt.Postgres);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Choose()
        {
            var c = ParseColumn("CHOOSE(2, 'one', 'two', 'three')");

            var expected = new ExpectedSqlResult()
                .WithSql("CHOOSE(2, 'one', 'two', 'three')", swt.SqlServer)
                .WithSql("DECODE(2, 1, 'one', 2, 'two', 3, 'three')", swt.Oracle, swt.Db2)
                .WithSql("ELT(2, 'one', 'two', 'three')", swt.MySql, swt.MariaDb)
                .WithSql("CASE  WHEN 2 = 1 THEN 'one' WHEN 2 = 2 THEN 'two' WHEN 2 = 3 THEN 'three' END", swt.Sqlite, swt.Postgres);

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
                .WithDefaultSql("(('a') || ('b') || ('c'))")
                .WithSql("CONCAT('a', 'b', 'c')", swt.SqlServer, swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ConcatWithSeperator()
        {
            var c = ParseColumn("CONCAT_WS('___', 'a','b','c')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("(('a') || ('___') || ('b') || ('___') || ('c'))")
                .WithSql("CONCAT_WS('___', 'a', 'b', 'c')", swt.SqlServer, swt.MySql, swt.MariaDb);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DateFromParts()
        {
            var c = ParseColumn("DATEFROMPARTS(2020, 12, 31)");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEFROMPARTS(2020, 12, 31)", swt.SqlServer)
                .WithSql("TO_DATE((2020) || (12) || (31), 'yyyyMMdd')", swt.Oracle)
                .WithSql("DATE(CONCAT_WS('-', 2020, 12, 31)))", swt.MySql, swt.MariaDb)
                .WithSql("DATE(SUBSTR('0000' || 2020, -4, 4) || '-' || SUBSTR('00' || 12, -2, 2) || '-' || SUBSTR('00' || 31, -2, 2))", swt.Sqlite)
                .WithSql("MAKE_DATE(2020, 12, 31)", swt.Postgres)
                .WithSql("DATE((2020) || '-' || (12) || '-' || (31))", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Day()
        {
            var c = ParseColumn("DAY({d '2000-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql("DAY({d '2000-12-31'})", swt.SqlServer)
                .WithSql("EXTRACT(day FROM DATE '2000-12-31')", swt.Oracle)
                .WithSql("DAY(DATE('2000-12-31'))", swt.MySql, swt.MariaDb)
                .WithSql("CAST(STRFTIME('%d', DATE('2000-12-31')) AS INT)", swt.Sqlite)
                .WithSql("EXTRACT(DAY FROM DATE '2000-12-31')", swt.Postgres)
                .WithSql("DAYOFMONTH(DATE '2000-12-31')", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DaysBetween()
        {
            var c = ParseColumn("DAYSBETWEEN({d '2000-12-31'}, {d '2001-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEDIFF(d, {d '2000-12-31'}, {d '2001-12-31'})", swt.SqlServer)
                .WithSql("(DATE '2001-12-31' - DATE '2000-12-31')", swt.Oracle)
                .WithSql("DATEDIFF(DATE('2001-12-31'), DATE('2000-12-31'))", swt.MySql, swt.MariaDb)
                .WithSql("CAST(JULIANDAY(DATE('2001-12-31') AS INT)) - CAST(JULIANDAY(DATE('2000-12-31') AS INT))", SqlWriterType.Sqlite)
                .WithSql("((DATE '2001-12-31')::date - (DATE '2000-12-31')::date)", swt.Postgres)
                .WithSql("DAYS_BETWEEN(DATE '2001-12-31', DATE '2000-12-31')", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Exp()
        {
            var c = ParseColumn("EXP(1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("EXP(1.23)")
                .WithSql("EXCEPTION: EXP function not available on Sqlite", swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Floor()
        {
            var c = ParseColumn("FLOOR(1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("FLOOR(1.23)")
                .WithSql("(CAST(1.23 AS INT) - (1.23 < CAST(1.23 AS INT)))", swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void GetDate()
        {
            var c = ParseColumn("GETDATE()");

            var expected = new ExpectedSqlResult()
                .WithSql("CONVERT(date, GETDATE())", swt.SqlServer)
                .WithSql("CURRENT_DATE", swt.Oracle, swt.Postgres)
                .WithSql("DATE(CURRENT_DATE())", swt.MySql, swt.MariaDb)
                .WithSql("DATE()", swt.Sqlite)
                .WithSql("(CURRENT DATE)", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void GetTimestamp()
        {
            var c = ParseColumn("GETTIMESTAMP()");

            var expected = new ExpectedSqlResult()
                .WithSql("GETDATE()", swt.SqlServer)
                .WithSql("CURRENT_TIMESTAMP", swt.Oracle, swt.Postgres)
                .WithSql("TIMESTAMP(CURRENT_TIMESTAMP())", swt.MySql, swt.MariaDb)
                .WithSql("DATETIME()", swt.Sqlite)
                .WithSql("(CURRENT TIMESTAMP)", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void IsNull()
        {
            var c = ParseColumn("ISNULL(x.y, 0)");

            var expected = new ExpectedSqlResult()
                .WithSql("ISNULL([x].[y], 0)", swt.SqlServer)
                .WithSql("NVL(\"x\".\"y\", 0)", swt.Oracle, swt.Db2)
                .WithSql("ISNULL(`x`.`y`, 0)", swt.MySql, swt.MariaDb)
                .WithSql("COALESCE(\"x\".\"y\", 0)", swt.Postgres)
                .WithSql("IFNULL([x].[y], 0)", swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Left()
        {
            var c = ParseColumn("LEFT('abcde', 2)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LEFT('abcde', 2)")
                .WithSql("SUBSTR('abcde', 1, 2)", swt.Oracle, swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Len()
        {
            var c = ParseColumn("LEN('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LENGTH('abc')")
                .WithSql("LEN('abc')", swt.SqlServer)
                .WithSql("LENGTH(RTRIM('abc'))", swt.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void LogDefaultBase()
        {
            var c = ParseColumn("LOG(1000)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LOG(1000)")
                .WithSql("EXCEPTION: LOG function not available on Sqlite", swt.Sqlite)
                .WithSql("LN(1000)", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void LogBase10()
        {
            var c = ParseColumn("LOG(1000, 10)");

            var expected = new ExpectedSqlResult()
                .WithSql("LOG(1000, 10)", swt.SqlServer, swt.Postgres)
                .WithSql("LOG(10, 1000)", swt.Oracle, swt.MySql, swt.MariaDb)
                .WithSql("EXCEPTION: LOG function not available on Sqlite", swt.Sqlite)
                .WithSql("LOG10(1000)", swt.Db2);

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
                .WithSql("MONTH({d '2000-12-31'})", swt.SqlServer)
                .WithSql("EXTRACT(month FROM DATE '2000-12-31')", swt.Oracle)
                .WithSql("MONTH(DATE('2000-12-31'))", swt.MySql, swt.MariaDb)
                .WithSql("CAST(STRFTIME('%m', DATE('2000-12-31')) AS INT)", swt.Sqlite)
                .WithSql("EXTRACT(MONTH FROM DATE '2000-12-31')", swt.Postgres)
                .WithSql("MONTH(DATE '2000-12-31')", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void MonthsBetween()
        {
            var c = ParseColumn("MONTHSBETWEEN({d '2000-12-30'}, {d '2001-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEDIFF(m, {d '2000-12-30'}, {d '2001-12-31'})", swt.SqlServer)
                .WithSql("FLOOR(MONTHS_BETWEEN(DATE '2001-12-31', DATE '2000-12-30'))", swt.Oracle)
                .WithSql("TIMESTAMPDIFF(MONTH, DATE('2000-12-30'), DATE('2001-12-31'))", swt.MySql, swt.MariaDb)
                .WithSql("((12 * (CAST(STRFTIME('%Y', DATE('2001-12-31')) AS INT) - CAST(STRFTIME('%Y', DATE('2000-12-30')) AS INT))) + CAST(STRFTIME('%m', DATE('2001-12-31')) AS INT) - CAST(STRFTIME('%m', DATE('2000-12-30')) AS INT))", swt.Sqlite)
                .WithSql("((12 * (EXTRACT(YEAR FROM DATE '2001-12-31') - EXTRACT(YEAR FROM DATE '2000-12-30'))) + EXTRACT(MONTH FROM DATE '2001-12-31') - EXTRACT(MONTH FROM DATE '2000-12-30'))", swt.Postgres)
                .WithSql("((12 * (YEAR(DATE '2001-12-31') - YEAR(DATE '2000-12-30'))) + MONTH(DATE '2001-12-31') - MONTH(DATE '2000-12-30'))", swt.Db2);

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
                .WithSql("POW(10, 2)", swt.Oracle, swt.MySql, swt.MariaDb)
                .WithSql("EXCEPTION: POWER function not available on Sqlite", SqlWriterType.Sqlite);

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
                .WithSql("REPLICATE('abc', 5)", swt.SqlServer)
                .WithSql("RPAD('', LENGTH(RTRIM('abc')) * 5, 'abc')", swt.Oracle)
                .WithSql("REPLACE(PRINTF('%.' || (5) || 'c', '/'),'/', 'abc')", swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Reverse()
        {
            var c = ParseColumn("REVERSE('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("REVERSE('abc')")
                .WithSql("EXCEPTION: REVERSE function not available on Sqlite", swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void RightFixedWidth()
        {
            var c = ParseColumn("RIGHT('abcde', 2)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("RIGHT('abcde', 2)")
                .WithSql("SUBSTR('abcde', -2, 2)", swt.Oracle, swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void RightVariableWidth()
        {
            var c = ParseColumn("RIGHT('abcde', 2*3)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("RIGHT('abcde', 2*3)")
                .WithSql("SUBSTR('abcde', 0 - (2*3), 2*3)", swt.Oracle, swt.Sqlite);

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
                .WithSql("RPAD('', 5, ' ')", swt.Oracle)
                .WithSql("REPLACE(PRINTF('%.' || (5) || 'c', '/'),'/', ' ')", swt.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Substring()
        {
            var c = ParseColumn("SUBSTRING('.bcd..', 2, 3)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SUBSTR('.bcd..', 2, 3)")
                .WithSql("SUBSTRING('.bcd..', 2, 3)", swt.SqlServer);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ToChar()
        {
            var c = ParseColumn("TOCHAR(123)");

            var expected = new ExpectedSqlResult()
                .WithSql("STR(123)", swt.SqlServer)
                .WithSql("TO_CHAR(123)", swt.Oracle)
                .WithSql("CONVERT(123, NCHAR)", swt.MySql, swt.MariaDb)
                .WithSql("CAST(123 AS NVARCHAR)", swt.Sqlite)
                .WithSql("(123)::varchar", swt.Postgres)
                .WithSql("NVARCHAR(123)", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ToTimestamp()
        {
            var c = ParseColumn("TOTIMESTAMP('2020-12-31 23:59.59.123')");

            var expected = new ExpectedSqlResult()
                .WithSql("CONVERT(datetime, '2020-12-31 23:59.59.123')", swt.SqlServer)
                .WithSql("TO_DATE('2020-12-31 23:59.59.123')", swt.Oracle)
                .WithSql("CONVERT('2020-12-31 23:59.59.123', DATETIME)", swt.MySql, swt.MariaDb)
                .WithSql("DATETIME('2020-12-31 23:59.59.123')", swt.Sqlite)
                .WithSql("('2020-12-31 23:59.59.123')::timestamp", swt.Postgres)
                .WithSql("TIMESTAMP('2020-12-31 23:59.59.123')", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ToNumber()
        {
            var c = ParseColumn("TONUMBER('123')");

            var expected = new ExpectedSqlResult()
                .WithSql("CONVERT(real, '123')", swt.SqlServer)
                .WithSql("TO_NUMBER('123')", swt.Oracle)
                .WithSql("CONVERT('123', DECIMAL)", swt.MySql, swt.MariaDb)
                .WithSql("CAST('123' AS REAL)", swt.Sqlite)
                .WithSql("('123')::decimal", swt.Postgres)
                .WithSql("DOUBLE('123')", swt.Db2);

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
                .WithSql("CONVERT(date, {ts '2000-12-31 23:59:59.123'})", swt.SqlServer)
                .WithSql("TRUNC(TIMESTAMP '2000-12-31 23:59:59.123')", swt.Oracle)
                .WithSql("DATE(TIMESTAMP('2000-12-31 23:59:59.123'))", swt.MySql, swt.MariaDb)
                .WithSql("DATE(DATETIME('2000-12-31 23:59:59'))", swt.Sqlite)
                .WithSql("DATE(TIMESTAMP '2000-12-31 23:59:59.123')", swt.Postgres, swt.Db2);

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
                .WithSql("YEAR({d '2000-12-31'})", swt.SqlServer)
                .WithSql("EXTRACT(year FROM DATE '2000-12-31')", swt.Oracle)
                .WithSql("YEAR(DATE('2000-12-31'))", swt.MySql, swt.MariaDb)
                .WithSql("CAST(STRFTIME('%Y', DATE('2000-12-31')) AS INT)", swt.Sqlite)
                .WithSql("EXTRACT(YEAR FROM DATE '2000-12-31')", swt.Postgres)
                .WithSql("YEAR(DATE '2000-12-31')", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void YearsBetween()
        {
            var c = ParseColumn("YEARSBETWEEN({d '2000-12-31'}, {d '2001-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEDIFF(year, {d '2000-12-31'}, {d '2001-12-31'})", swt.SqlServer)
                .WithSql("FLOOR(MONTHS_BETWEEN(DATE '2001-12-31', DATE '2000-12-31') / 12)", swt.Oracle)
                .WithSql("TIMESTAMPDIFF(YEAR, DATE('2000-12-31'), DATE('2001-12-31'))", swt.MySql, swt.MariaDb)
                .WithSql("(CAST(STRFTIME('%Y', DATE('2001-12-31')) AS INT) - CAST(STRFTIME('%Y', DATE('2000-12-31')) AS INT))", swt.Sqlite)
                .WithSql("(EXTRACT(YEAR FROM DATE '2001-12-31') - EXTRACT(YEAR FROM DATE '2000-12-31'))", swt.Postgres)
                .WithSql("(YEAR(DATE '2001-12-31') - YEAR(DATE '2000-12-31'))", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }
    }
}
