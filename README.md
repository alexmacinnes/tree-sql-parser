# tree-sql-parser
C# library for parsing SQL select statements into an Abstract Syntax Tree (AST).
The AST can be analysed, transformed and translated into various SQL dialects (SQL Server, Oracle, MySql (MariaDb), Sqlite, Postgres amd DB2 currently supported).

Available on Nuget, [tree-sql-parser](https://www.nuget.org/packages/Tree-Sql-Parser//).

    Install-Package tree-sql-parser

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

### Code Samples
Parse SQL
```cs
using TreeSqlParser.Model;
using TreeSqlParser.Parsing;

string sql = "select id, surname from dbo.people";
SqlRootElement root = SelectParser.ParseSelectStatement(sql);
```
Traverse AST explicitly
```cs
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;

var statement = (SelectStatement)root.Child;
List<Column> columns = statement.Selects[0].Columns;
```
Traverse AST with Linq
```cs
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;

var allElements = root.Flatten();
List<PrimitiveColumn> columns2 = allElements.OfType<PrimitiveColumn>().ToList();
```
Modify AST
```cs
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;

// modify statement to: "select id, UPPER(surname) from dbo.people"
var nameColumn = root.Flatten().OfType<PrimitiveColumn>().Single(x => x.Name.Name == "surname");
var toUpperColumn = SelectParser.ParseColumn("UPPER(surname)");

// remove nameColumn from the AST and replace it with toUpperColumn
nameColumn.ReplaceSelf(toUpperColumn);
```
### Generate SQL

FullSqlServerWriter has full support for converting any AST back into SQL.
```cs
using TreeSqlParser.Model;
using TreeSqlParser.Writers;

string fullSqlServerSql = SqlWriterFactory.FullSqlServerWriter().GenerateSql(root);
```

### Generate SQL with CommonSqlWriters
CommonSqlWriters have more limited support, which can be translated to a variety of dialects.
More dialects to follow.
```cs
using TreeSqlParser.Model;
using TreeSqlParser.Writers;

string commonSqlServerSql = SqlWriterFactory.CommonSqlWriter(SqlWriterType.SqlServer).GenerateSql(root);
string commonOracleSql = SqlWriterFactory.CommonSqlWriter(SqlWriterType.Oracle).GenerateSql(root);
string commonMySqlSql = SqlWriterFactory.CommonSqlWriter(SqlWriterType.MySql).GenerateSql(root);
string commonSqliteSql = SqlWriterFactory.CommonSqlWriter(SqlWriterType.Sqlite).GenerateSql(root);
string commonPostgresSql = SqlWriterFactory.CommonSqlWriter(SqlWriterType.Postgres).GenerateSql(root);
string commonMariaDbSql = SqlWriterFactory.CommonSqlWriter(SqlWriterType.MariaDb).GenerateSql(root);
string commonDb2ql = SqlWriterFactory.CommonSqlWriter(SqlWriterType.Db2).GenerateSql(root);
```

### CommonSqlWriters support

| SQL Feature | SQL Server | Oracle | MySql (MariaDB) | Sqlite&nbsp; | Postgres | DB2&nbsp;&nbsp;&nbsp;&nbsp; | Notes
| -- | -- | -- | -- | -- | -- | -- | -- |
| Simple SELECT | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark:| :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | SELECT .. FROM .. WHERE .. GROUP BY .. HAVING .. ORDER BY
 SET operations | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark:| :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | UNION, UNION ALL, INTERSECT, EXCEPT
| SUBSELECT | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark:| :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: |
| CTE | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark:| :heavy_check_mark: | :heavy_check_mark: | non recursive only
| ARITHMETIC (+-*/%) | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark:| :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | + is interpreted as plus. If you need to concatenate string use CONCAT(...). Oracle - % is converted to MOD(x, y)
| JOINS: INNER, LEFT | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark:| :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | 
| JOINS: RIGHT | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark:| :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | Sqlite - rewrites as LEFT JOIN
| JOINS: FULL | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | | | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | 
| FUNCTIONS | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | Limited set of known functions - see below
| AGGREGATIONS | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | SUM, MIN, MAX, AVG, COUNT, COUNT DISTINCT only
| GROUP BY | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | Simple columns only, no GROUPING SETS
| FETCH, OFFSET | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | :heavy_check_mark::heavy_check_mark: | 
| CAST column | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | CAST as following types only: nvarchar, varchar, int, real, timestamp
| CONVERT column | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | TRY_CONVERT not supported

Function support is limited to the following list of known SQL Server functions. These get translated into native function calls on other SQL dialects where possible, or into complex statements where not.

| Function | SQL Server | Oracle | MySql<br/>(MariaDB) | Sqlite | Postgres | DB2 |
| -- | -- | -- | -- | -- | -- | -- |
Abs(num) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
AddDays(date, days) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
AddMonths(date, months) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
AddYears(date, years) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Ceiling(num) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
CharIndex(fullText, search) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
CharIndex(fullText, search, startIndex) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Choose(selector, items... ) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Coalesce(columns...) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Concat(columns...) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Concat_WS(seperator, columns...) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
DateFromParts(year, month, day) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Day(date) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
DaysBetween(date1, date2) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Exp(num) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | | :heavy_check_mark: | :heavy_check_mark: |
Floor(num) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
GetDate() | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
GetTimestamp() | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
IsNull(column) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Left(text) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Len(text) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Log(num) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | | :heavy_check_mark: | :heavy_check_mark: |
Log(num, base) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | | :heavy_check_mark: | :heavy_check_mark: |
Lower(text) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
LTrim(text) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Month(date) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
MonthsBetween(date1, date2)| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
NullIf(column) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Power(num, pwr) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | | :heavy_check_mark: | :heavy_check_mark: |
Replace(text)| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Replicate(text, n) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Reverse(text) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | | :heavy_check_mark: | :heavy_check_mark: |
Right(text, n) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Round(num) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
RTrim(text) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Sign(num)| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Space(n) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Substring(text, start, count)| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
ToChar(column) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
ToTimestamp(column)| :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
ToNumber(column) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Trim(text) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
TruncateDate(date) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Upper(text) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
Year(date) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
YearsBetween(date1, date2) | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: | :heavy_check_mark: |
