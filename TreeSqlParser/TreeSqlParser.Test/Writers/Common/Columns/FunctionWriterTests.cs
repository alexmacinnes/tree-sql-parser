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
                .WithSql("DATEADD(d, 365, {d '2000-12-31'})", SqlWriterType.SqlServer)
                .WithSql("(DATE '2000-12-31' + 365)", SqlWriterType.Oracle)
                .WithSql("ADDDATE(DATE('2000-12-31'), INTERVAL (365) DAY)", SqlWriterType.MySql)
                .WithSql("ADDDATE(DATE('2000-12-31'), INTERVAL (365) DAY)", SqlWriterType.MariaDb)
                .WithSql("DATETIME(DATE('2000-12-31'), ''||(365)||' DAYS')", SqlWriterType.Sqlite)
                .WithSql("DATE '2000-12-31' + (INTERVAL '1d' * 365)", SqlWriterType.Postgres)
                .WithSql("ADD_DAYS(DATE '2000-12-31', 365)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void AddMonths()
        {
            var c = ParseColumn("ADDMONTHS({d '2000-12-31'}, 3)");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEADD(m, 3, {d '2000-12-31'})", SqlWriterType.SqlServer)
                .WithSql("ADD_MONTHS(DATE '2000-12-31', 3)", SqlWriterType.Oracle)
                .WithSql("ADDDATE(DATE('2000-12-31'), INTERVAL (3) MONTH)", SqlWriterType.MySql)
                .WithSql("ADDDATE(DATE('2000-12-31'), INTERVAL (3) MONTH)", SqlWriterType.MariaDb)
                .WithSql("DATETIME(DATE('2000-12-31'), ''||(3)||' MONTHS')", SqlWriterType.Sqlite)
                .WithSql("DATE '2000-12-31' + (INTERVAL '1 month' * 3)", SqlWriterType.Postgres)
                .WithSql("ADD_MONTHS(DATE '2000-12-31', 3)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void AddYears()
        {
            var c = ParseColumn("ADDYEARS({d '2000-12-31'}, 3)");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEADD(y, 3, {d '2000-12-31'})", SqlWriterType.SqlServer)
                .WithSql("ADD_MONTHS(DATE '2000-12-31', 12 * (3)", SqlWriterType.Oracle)
                .WithSql("ADDDATE(DATE('2000-12-31'), INTERVAL (3) YEAR)", SqlWriterType.MySql)
                .WithSql("ADDDATE(DATE('2000-12-31'), INTERVAL (3) YEAR)", SqlWriterType.MariaDb)
                .WithSql("DATETIME(DATE('2000-12-31'), ''||(3)||' YEARS')", SqlWriterType.Sqlite)
                .WithSql("DATE '2000-12-31' + (INTERVAL '1y' * 3)", SqlWriterType.Postgres)
                .WithSql("ADD_YEARS(DATE '2000-12-31', 3)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Ceiling()
        {
            var c = ParseColumn("CEILING(1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("CEIL(1.23)")
                .WithSql("CEILING(1.23)", SqlWriterType.SqlServer)
                .WithSql("(CAST(1.23 AS INT) + (1.23 > CAST(1.23 AS INT)))", SqlWriterType.Sqlite)
                .WithSql("CEILING(1.23)", SqlWriterType.Postgres);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CharIndex()
        {
            var c = ParseColumn("CHARINDEX('d', 'abcde')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("INSTR('abcde', 'd')")
                .WithSql("CHARINDEX('d', 'abcde')", SqlWriterType.SqlServer)
                .WithSql("STRPOS('abcde', 'd')", SqlWriterType.Postgres);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CharIndexWithStartIndex()
        {
            var c = ParseColumn("CHARINDEX('d', 'abcde', 2)");

            var expected = new ExpectedSqlResult()
                .WithSql("CHARINDEX('d', 'abcde', 2)", SqlWriterType.SqlServer)
                .WithSql("INSTR('abcde', 'd', 2)", SqlWriterType.Oracle)
                .WithSql("IFNULL((NULLIF(INSTR(SUBSTR('abcde', 2), 'd'), 0)) + 2 - 1, 0)", SqlWriterType.MySql)
                .WithSql("IFNULL((NULLIF(INSTR(SUBSTR('abcde', 2), 'd'), 0)) + 2 - 1, 0)", SqlWriterType.MariaDb)
                .WithSql("IFNULL((NULLIF(INSTR(SUBSTR('abcde', 2), 'd'), 0)) + 2 - 1, 0)", SqlWriterType.Sqlite)
                .WithSql("COALESCE((NULLIF(STRPOS(SUBSTR('abcde', 2), 'd'), 0)) + 2 - 1, 0)", SqlWriterType.Postgres)
                .WithSql("INSTR('abcde', 'd', 2)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Choose()
        {
            var c = ParseColumn("CHOOSE(2, 'one', 'two', 'three')");

            var expected = new ExpectedSqlResult()
                .WithSql("CHOOSE(2, 'one', 'two', 'three')", SqlWriterType.SqlServer)
                .WithSql("DECODE(2, 1, 'one', 2, 'two', 3, 'three')", SqlWriterType.Oracle)
                .WithSql("ELT(2, 'one', 'two', 'three')", SqlWriterType.MySql)
                .WithSql("ELT(2, 'one', 'two', 'three')", SqlWriterType.MariaDb)
                .WithSql("CASE  WHEN 2 = 1 THEN 'one' WHEN 2 = 2 THEN 'two' WHEN 2 = 3 THEN 'three' END", SqlWriterType.Sqlite)
                .WithSql("CASE  WHEN 2 = 1 THEN 'one' WHEN 2 = 2 THEN 'two' WHEN 2 = 3 THEN 'three' END", SqlWriterType.Postgres)
                .WithSql("DECODE(2, 1, 'one', 2, 'two', 3, 'three')", SqlWriterType.Db2);

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
                .WithSql("CONCAT('a', 'b', 'c')", SqlWriterType.SqlServer)
                .WithSql("CONCAT('a', 'b', 'c')", SqlWriterType.MySql)
                .WithSql("CONCAT('a', 'b', 'c')", SqlWriterType.MariaDb);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ConcatWithSeperator()
        {
            var c = ParseColumn("CONCAT_WS('___', 'a','b','c')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("'a' || '___' || 'b' || '___' || 'c'")
                .WithSql("CONCAT_WS('___', 'a', 'b', 'c')", SqlWriterType.SqlServer)
                .WithSql("CONCAT_WS('___', 'a', 'b', 'c')", SqlWriterType.MySql)
                .WithSql("CONCAT_WS('___', 'a', 'b', 'c')", SqlWriterType.MariaDb);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DateFromParts()
        {
            var c = ParseColumn("DATEFROMPARTS(2020, 12, 31)");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEFROMPARTS(2020, 12, 31)", SqlWriterType.SqlServer)
                .WithSql("TO_DATE((2020) || (12) || (31), 'yyyyMMdd')", SqlWriterType.Oracle)
                .WithSql("DATE(CONCAT_WS('-', 2020, 12, 31)))", SqlWriterType.MySql)
                .WithSql("DATE(CONCAT_WS('-', 2020, 12, 31)))", SqlWriterType.MariaDb)
                .WithSql("DATE(SUBSTR('0000' || 2020, -4, 4) || '-' || SUBSTR('00' || 12, -2, 2) || '-' || SUBSTR('00' || 31, -2, 2))", SqlWriterType.Sqlite)
                .WithSql("MAKE_DATE(2020, 12, 31)", SqlWriterType.Postgres)
                .WithSql("DATE(2020 || '-' || 12  || '-' || 31 )", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Day()
        {
            var c = ParseColumn("DAY({d '2000-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql("DAY({d '2000-12-31'})", SqlWriterType.SqlServer)
                .WithSql("EXTRACT(day FROM DATE '2000-12-31')", SqlWriterType.Oracle)
                .WithSql("DAY(DATE('2000-12-31'))", SqlWriterType.MySql)
                .WithSql("DAY(DATE('2000-12-31'))", SqlWriterType.MariaDb)
                .WithSql("CAST(STRFTIME('%d', DATE('2000-12-31')) AS INT)", SqlWriterType.Sqlite)
                .WithSql("EXTRACT(DAY FROM DATE '2000-12-31')", SqlWriterType.Postgres)
                .WithSql("DAYOFMONTH(DATE '2000-12-31')", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void DaysBetween()
        {
            var c = ParseColumn("DAYSBETWEEN({d '2000-12-31'}, {d '2001-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEDIFF(d, {d '2000-12-31'}, {d '2001-12-31'})", SqlWriterType.SqlServer)
                .WithSql("(DATE '2001-12-31' - DATE '2000-12-31')", SqlWriterType.Oracle)
                .WithSql("DATEDIFF(DATE('2001-12-31'), DATE('2000-12-31'))", SqlWriterType.MySql)
                .WithSql("DATEDIFF(DATE('2001-12-31'), DATE('2000-12-31'))", SqlWriterType.MariaDb)
                .WithSql("CAST(JULIANDAY(DATE('2001-12-31') AS INT)) - CAST(JULIANDAY(DATE('2000-12-31') AS INT))", SqlWriterType.Sqlite)
                .WithSql("((DATE '2001-12-31')::date - (DATE '2000-12-31')::date)", SqlWriterType.Postgres)
                .WithSql("DAYS_BETWEEN(DATE '2001-12-31', DATE '2000-12-31')", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Exp()
        {
            var c = ParseColumn("EXP(1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("EXP(1.23)")
                .WithSql("EXCEPTION: EXP function not available on Sqlite", SqlWriterType.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Floor()
        {
            var c = ParseColumn("FLOOR(1.23)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("FLOOR(1.23)")
                .WithSql("(CAST(1.23 AS INT) - (1.23 < CAST(1.23 AS INT)))", SqlWriterType.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void GetDate()
        {
            var c = ParseColumn("GETDATE()");

            var expected = new ExpectedSqlResult()
                .WithSql("CONVERT(date, GETDATE())", SqlWriterType.SqlServer)
                .WithSql("CURRENT_DATE", SqlWriterType.Oracle)
                .WithSql("DATE(CURRENT_DATE())", SqlWriterType.MySql)
                .WithSql("DATE(CURRENT_DATE())", SqlWriterType.MariaDb)
                .WithSql("DATE()", SqlWriterType.Sqlite)
                .WithSql("CURRENT_DATE", SqlWriterType.Postgres)
                .WithSql("(CURRENT DATE)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void GetTimestamp()
        {
            var c = ParseColumn("GETTIMESTAMP()");

            var expected = new ExpectedSqlResult()
                .WithSql("GETDATE()", SqlWriterType.SqlServer)
                .WithSql("CURRENT_TIMESTAMP", SqlWriterType.Oracle)
                .WithSql("TIMESTAMP(CURRENT_TIMESTAMP())", SqlWriterType.MySql)
                .WithSql("TIMESTAMP(CURRENT_TIMESTAMP())", SqlWriterType.MariaDb)
                .WithSql("DATETIME()", SqlWriterType.Sqlite)
                .WithSql("CURRENT_TIMESTAMP", SqlWriterType.Postgres)
                .WithSql("(CURRENT TIMESTAMP)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void IsNull()
        {
            var c = ParseColumn("ISNULL(x.y, 0)");

            var expected = new ExpectedSqlResult()
                .WithSql("ISNULL([x].[y], 0)", SqlWriterType.SqlServer)
                .WithSql("NVL(\"x\".\"y\", 0)", SqlWriterType.Oracle)
                .WithSql("ISNULL(`x`.`y`, 0)", SqlWriterType.MySql)
                .WithSql("ISNULL(`x`.`y`, 0)", SqlWriterType.MariaDb)
                .WithSql("IFNULL([x].[y], 0)", SqlWriterType.Sqlite)
                .WithSql("COALESCE(\"x\".\"y\", 0)", SqlWriterType.Postgres)
                .WithSql("NVL(\"x\".\"y\", 0)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Left()
        {
            var c = ParseColumn("LEFT('abcde', 2)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LEFT('abcde', 2)")
                .WithSql("SUBSTR('abcde', 1, 2)", SqlWriterType.Oracle)
                .WithSql("SUBSTR('abcde', 1, 2)", SqlWriterType.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Len()
        {
            var c = ParseColumn("LEN('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LENGTH('abc')")
                .WithSql("LEN('abc')", SqlWriterType.SqlServer)
                .WithSql("LENGTH(RTRIM('abc'))", SqlWriterType.Oracle);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void LogDefaultBase()
        {
            var c = ParseColumn("LOG(1000)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("LOG(1000)")
                .WithSql("EXCEPTION: LOG function not available on Sqlite", SqlWriterType.Sqlite)
                .WithSql("LN(1000)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void LogBase10()
        {
            var c = ParseColumn("LOG(1000, 10)");

            var expected = new ExpectedSqlResult()
                .WithSql("LOG(1000, 10)", SqlWriterType.SqlServer)
                .WithSql("LOG(10, 1000)", SqlWriterType.Oracle)
                .WithSql("LOG(10, 1000)", SqlWriterType.MySql)
                .WithSql("LOG(10, 1000)", SqlWriterType.MariaDb)
                .WithSql("EXCEPTION: LOG function not available on Sqlite", SqlWriterType.Sqlite)
                .WithSql("LOG(1000, 10)", SqlWriterType.Postgres)
                .WithSql("LOG10(1000)", SqlWriterType.Db2);

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
                .WithSql("MONTH({d '2000-12-31'})", SqlWriterType.SqlServer)
                .WithSql("EXTRACT(month FROM DATE '2000-12-31')", SqlWriterType.Oracle)
                .WithSql("MONTH(DATE('2000-12-31'))", SqlWriterType.MySql)
                .WithSql("MONTH(DATE('2000-12-31'))", SqlWriterType.MariaDb)
                .WithSql("CAST(STRFTIME('%m', DATE('2000-12-31')) AS INT)", SqlWriterType.Sqlite)
                .WithSql("EXTRACT(MONTH FROM DATE '2000-12-31')", SqlWriterType.Postgres)
                .WithSql("MONTH(DATE '2000-12-31')", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void MonthsBetween()
        {
            var c = ParseColumn("MONTHSBETWEEN({d '2000-12-30'}, {d '2001-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEDIFF(m, {d '2000-12-30'}, {d '2001-12-31'})", SqlWriterType.SqlServer)
                .WithSql("FLOOR(MONTHS_BETWEEN(DATE '2001-12-31', DATE '2000-12-30'))", SqlWriterType.Oracle)
                .WithSql("TIMESTAMPDIFF(MONTH, DATE('2000-12-30'), DATE('2001-12-31'))", SqlWriterType.MySql)
                .WithSql("TIMESTAMPDIFF(MONTH, DATE('2000-12-30'), DATE('2001-12-31'))", SqlWriterType.MariaDb)
                .WithSql("EXCEPTION: MONTHSBETWEEN function not available on Sqlite", SqlWriterType.Sqlite)
                .WithSql("(12*(EXTRACT(YEAR FROM DATE '2001-12-31') - EXTRACT(YEAR FROM DATE '2000-12-30'))) + (EXTRACT(MONTH FROM DATE '2001-12-31') - EXTRACT(MONTH FROM DATE '2000-12-30'))", SqlWriterType.Postgres)
                .WithSql("(12*((YEAR(DATE '2001-12-31') - YEAR(DATE '2000-12-30')))) + (MONTH(DATE '2001-12-31') - MONTH(DATE '2000-12-30'))", SqlWriterType.Db2);

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
                .WithSql("POW(10, 2)", SqlWriterType.Oracle)
                .WithSql("POW(10, 2)", SqlWriterType.MySql)
                .WithSql("POW(10, 2)", SqlWriterType.MariaDb)
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
                .WithSql("REPLICATE('abc', 5)", SqlWriterType.SqlServer)
                .WithSql("RPAD('', LENGTH(RTRIM('abc')) * 5, 'abc')", SqlWriterType.Oracle)
                .WithSql("REPLACE(PRINTF('%.' || (5) || 'c', '/'),'/', 'abc')", SqlWriterType.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Reverse()
        {
            var c = ParseColumn("REVERSE('abc')");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("REVERSE('abc')")
                .WithSql("EXCEPTION: REVERSE function not available on Sqlite", SqlWriterType.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void RightFixedWidth()
        {
            var c = ParseColumn("RIGHT('abcde', 2)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("RIGHT('abcde', 2)")
                .WithSql("SUBSTR('abcde', -2, 2)", SqlWriterType.Oracle)
                .WithSql("SUBSTR('abcde', -2, 2)", SqlWriterType.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void RightVariableWidth()
        {
            var c = ParseColumn("RIGHT('abcde', 2*3)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("RIGHT('abcde', 2*3)")
                .WithSql("SUBSTR('abcde', 0 - (2*3), 2*3)", SqlWriterType.Oracle)
                .WithSql("SUBSTR('abcde', 0 - (2*3), 2*3)", SqlWriterType.Sqlite);

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
                .WithSql("RPAD('', 5, ' ')", SqlWriterType.Oracle)
                .WithSql("REPLACE(PRINTF('%.' || (5) || 'c', '/'),'/', ' ')", SqlWriterType.Sqlite);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void Substring()
        {
            var c = ParseColumn("SUBSTRING('.bcd..', 2, 3)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("SUBSTR('.bcd..', 2, 3)")
                .WithSql("SUBSTRING('.bcd..', 2, 3)", SqlWriterType.SqlServer);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ToChar()
        {
            var c = ParseColumn("TOCHAR(123)");

            var expected = new ExpectedSqlResult()
                .WithSql("STR(123)", SqlWriterType.SqlServer)
                .WithSql("TO_CHAR(123)", SqlWriterType.Oracle)
                .WithSql("CONVERT(123, NCHAR)", SqlWriterType.MySql)
                .WithSql("CONVERT(123, NCHAR)", SqlWriterType.MariaDb)
                .WithSql("CAST(123 AS NVARCHAR)", SqlWriterType.Sqlite)
                .WithSql("(123)::varchar", SqlWriterType.Postgres)
                .WithSql("NVARCHAR(123)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ToTimestamp()
        {
            var c = ParseColumn("TOTIMESTAMP('2020-12-31 23:59.59.123')");

            var expected = new ExpectedSqlResult()
                .WithSql("CONVERT(datetime, '2020-12-31 23:59.59.123')", SqlWriterType.SqlServer)
                .WithSql("TO_DATE('2020-12-31 23:59.59.123')", SqlWriterType.Oracle)
                .WithSql("CONVERT('2020-12-31 23:59.59.123', DATETIME)", SqlWriterType.MySql)
                .WithSql("CONVERT('2020-12-31 23:59.59.123', DATETIME)", SqlWriterType.MariaDb)
                .WithSql("DATETIME('2020-12-31 23:59.59.123')", SqlWriterType.Sqlite)
                .WithSql("('2020-12-31 23:59.59.123')::timestamp", SqlWriterType.Postgres)
                .WithSql("TIMESTAMP('2020-12-31 23:59.59.123')", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void ToNumber()
        {
            var c = ParseColumn("TONUMBER('123')");

            var expected = new ExpectedSqlResult()
                .WithSql("CONVERT(real, '123')", SqlWriterType.SqlServer)
                .WithSql("TO_NUMBER('123')", SqlWriterType.Oracle)
                .WithSql("CONVERT('123', DECIMAL)", SqlWriterType.MySql)
                .WithSql("CONVERT('123', DECIMAL)", SqlWriterType.MariaDb)
                .WithSql("CAST('123' AS REAL)", SqlWriterType.Sqlite)
                .WithSql("('123')::decimal", SqlWriterType.Postgres)
                .WithSql("DOUBLE('123')", SqlWriterType.Db2);

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
                .WithSql("CONVERT(date, {ts '2000-12-31 23:59:59.123'})", SqlWriterType.SqlServer)
                .WithSql("TRUNC(TIMESTAMP '2000-12-31 23:59:59.123')", SqlWriterType.Oracle)
                .WithSql("DATE(TIMESTAMP('2000-12-31  23:59:59.123'))", SqlWriterType.MySql)
                .WithSql("DATE(TIMESTAMP('2000-12-31  23:59:59.123'))", SqlWriterType.MariaDb)
                .WithSql("DATE(DATETIME('2000-12-31 23:59:59'))", SqlWriterType.Sqlite)
                .WithSql("DATE(TIMESTAMP '2000-12-31 23:59:59.123')", SqlWriterType.Postgres)
                .WithSql("DATE(TIMESTAMP '2000-12-31 23:59:59.123')", SqlWriterType.Db2);

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
                .WithSql("YEAR({d '2000-12-31'})", SqlWriterType.SqlServer)
                .WithSql("EXTRACT(year FROM DATE '2000-12-31')", SqlWriterType.Oracle)
                .WithSql("YEAR(DATE('2000-12-31'))", SqlWriterType.MySql)
                .WithSql("YEAR(DATE('2000-12-31'))", SqlWriterType.MariaDb)
                .WithSql("CAST(STRFTIME('%Y', DATE('2000-12-31')) AS INT)", SqlWriterType.Sqlite)
                .WithSql("EXTRACT(YEAR FROM DATE '2000-12-31')", SqlWriterType.Postgres)
                .WithSql("YEAR(DATE '2000-12-31')", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void YearsBetween()
        {
            var c = ParseColumn("YEARSBETWEEN({d '2000-12-31'}, {d '2001-12-31'})");

            var expected = new ExpectedSqlResult()
                .WithSql("DATEDIFF(year, {d '2000-12-31'}, {d '2001-12-31'})", SqlWriterType.SqlServer)
                .WithSql("FLOOR(MONTHS_BETWEEN(DATE '2001-12-31', DATE '2000-12-31') / 12)", SqlWriterType.Oracle)
                .WithSql("TIMESTAMPDIFF(YEAR, DATE('2000-12-31'), DATE('2001-12-31'))", SqlWriterType.MySql)
                .WithSql("TIMESTAMPDIFF(YEAR, DATE('2000-12-31'), DATE('2001-12-31'))", SqlWriterType.MariaDb)
                .WithSql("EXCEPTION: YEARSBETWEEN function not available on Sqlite", SqlWriterType.Sqlite)
                .WithSql("(EXTRACT(YEAR FROM DATE '2001-12-31') - EXTRACT(YEAR FROM DATE '2000-12-31'))", SqlWriterType.Postgres)
                .WithSql("(YEAR(DATE '2001-12-31') - YEAR(DATE '2000-12-31'))", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }
    }
}
