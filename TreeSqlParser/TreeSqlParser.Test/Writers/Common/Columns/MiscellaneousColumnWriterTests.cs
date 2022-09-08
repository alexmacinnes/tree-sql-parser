using NUnit.Framework;
using System.Collections.Generic;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class MiscellaneousColumnWriterTests
    {
        private string Sql(Column c, SqlWriterType db) => CommonMother.Sql(c, db);

        private Column ParseColumn(string sql) =>
            SelectParser.ParseColumn(sql);

        [TestCase("*")]
        [TestCase("(1)")]
        [TestCase("CASE WHEN 1 = 2 THEN 3 WHEN 4 = 5 THEN 6 END")]
        [TestCase("CASE WHEN 1 = 2 THEN 3 WHEN 4 = 5 THEN 6 ELSE 7 END")]
        public void SqlSameOnAllDbs(string sql)
        {
            var column = ParseColumn(sql);

            foreach (var x in CommonMother.AllCommonWriters)
            {
                string generatedSql = x.GenerateSql(column);
                Assert.AreEqual(sql, generatedSql);
            }
        }

        [Test]
        public void IifColumn()
        {
            var c = ParseColumn("IIF(1 = 2, 3, 4)");

            Assert.AreEqual("IIF(1 = 2, 3, 4)", Sql(c, SqlWriterType.SqlServer));
            Assert.AreEqual("IIF(1 = 2, 3, 4)", Sql(c, SqlWriterType.Oracle));
            Assert.AreEqual("IF(1 = 2, 3, 4)", Sql(c, SqlWriterType.MySql));
            Assert.AreEqual("CASE WHEN 1 = 2 THEN 3 ELSE 4 END", Sql(c, SqlWriterType.Sqlite));
        }

        [Test]
        public void SubselectColumn()
        {
            var c = ParseColumn("(SELECT 1)");

            Assert.AreEqual("(SELECT 1)", Sql(c, SqlWriterType.SqlServer));
            Assert.AreEqual("(SELECT 1 FROM dual)", Sql(c, SqlWriterType.Oracle));
            Assert.AreEqual("(SELECT 1)", Sql(c, SqlWriterType.MySql));
            Assert.AreEqual("(SELECT 1)", Sql(c, SqlWriterType.Sqlite));
        }

        [Test]
        public void AliasColumn()
        {
            var c = ParseColumn("1 AS foo");

            Assert.AreEqual("1 AS [foo]", Sql(c, SqlWriterType.SqlServer));
            Assert.AreEqual("1 AS \"foo\"", Sql(c, SqlWriterType.Oracle));
            Assert.AreEqual("1 AS `foo`", Sql(c, SqlWriterType.MySql));
            Assert.AreEqual("1 AS [foo]", Sql(c, SqlWriterType.Sqlite));
        }

        [Test]
        public void PrimitiveColumnWithTable()
        {
            var c = ParseColumn("x.y");

            Assert.AreEqual("[x].[y]", Sql(c, SqlWriterType.SqlServer));
            Assert.AreEqual("\"x\".\"y\"", Sql(c, SqlWriterType.Oracle));
            Assert.AreEqual("`x`.`y`", Sql(c, SqlWriterType.MySql));
            Assert.AreEqual("[x].[y]", Sql(c, SqlWriterType.Sqlite));
        }

        [Test]
        public void PrimitiveColumnWithoutTable()
        {
            var c = ParseColumn("x");

            Assert.AreEqual("[x]", Sql(c, SqlWriterType.SqlServer));
            Assert.AreEqual("\"x\"", Sql(c, SqlWriterType.Oracle));
            Assert.AreEqual("`x`", Sql(c, SqlWriterType.MySql));
            Assert.AreEqual("[x]", Sql(c, SqlWriterType.Sqlite));
        }
    }
}
