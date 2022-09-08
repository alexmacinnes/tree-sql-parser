# tree-sql-parser
C# library for parsing SQL select statements into an Abstract Syntax Tree (AST).
The AST can be analysed, transformed and translated into various SQL dialects (SQL Server, Oraclem MySql and Sqlite currently supported).

Available on Nuget, [tree-sql-parser](https://www.nuget.org/packages/Tree-Sql-Parser//).

    Install-Package tree-sql-parser

[![License](https://img.shields.io/badge/License-Apache%202.0-blue.svg)](https://opensource.org/licenses/Apache-2.0)

### Code Samples
Parse SQL
```cs
string sql = "select id, surname from dbo.people";
SqlRootElement root = SelectParser.ParseSelectStatement(sql);
```
Traverse AST explicitly
```cs
var statement = (SelectStatement)root.Child;
List<Column> columns = statement.Selects[0].Columns;
```
Traverse AST with Linq
```cs
var allElements = root.Flatten();
List<PrimitiveColumn> columns2 = allElements.OfType<PrimitiveColumn>().ToList();
```
Modify AST
```cs
// modify statement to: "select id, UPPER(surname) from dbo.people"
var nameColumn = root.Flatten().OfType<PrimitiveColumn>().Single(x => x.Name.Name == "surname");
var toUpperColumn = SelectParser.ParseColumn("UPPER(surname)");

// remove nameColumn from the AST and replace it with toUpperColumn
nameColumn.ReplaceSelf(toUpperColumn);
```
Translate AST to SQL
```cs
// FullSqlServerWriter has full support for SQL Server syntax
string fullSqlServerSql = new FullSqlServerWriter().GenerateSql(root);

Translate AST to SQL
// CommonSqlWriters have more limited support, which can be translated to a variety of dialects
// Currently SQL Server and Oracle are provided. More to follow.
string commonSqlServerSql = new CommonSqlServerSqlWriter().GenerateSql(root);
string commonOracleSql = new CommonOracleSqlWriter().GenerateSql(root);
```
