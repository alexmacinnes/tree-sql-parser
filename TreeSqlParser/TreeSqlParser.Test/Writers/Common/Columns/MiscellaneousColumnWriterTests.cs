﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Writers.Safe;

namespace TreeSqlParser.Writers.Test.Safe.Columns
{
    public class MiscellaneousColumnWriterTests
    {
        private readonly IReadOnlyDictionary<DbFamily, ISqlWriter> writers = new Dictionary<DbFamily, ISqlWriter>
        {
            { DbFamily.SqlServer, new SafeSqlServerWriter() },
            { DbFamily.Oracle, new SafeOracleSqlWriter() }
        };

        private string Sql(Column c, DbFamily db) => writers[db].GenerateSql(c);

        private Column ParseColumn(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select " + sql).Child).Selects[0].Columns[0];

        [TestCase("*")]
        [TestCase("(1)")]
        [TestCase("IIF(1 = 2, 3, 4)")]
        [TestCase("CASE WHEN 1 = 2 THEN 3 WHEN 4 = 5 THEN 6 END")]
        [TestCase("CASE WHEN 1 = 2 THEN 3 WHEN 4 = 5 THEN 6 ELSE 7 END")]
        public void SqlSameOnAllDbs(string sql)
        {
            var column = ParseColumn(sql);

            foreach (var x in writers.Values)
            {
                string generatedSql = x.GenerateSql(column);
                Assert.AreEqual(sql, generatedSql);
            }
        }

        [Test]
        public void SubselectColumn()
        {
            var c = ParseColumn("(SELECT 1)");

            Assert.AreEqual("(SELECT 1)", Sql(c, DbFamily.SqlServer));
            Assert.AreEqual("(SELECT 1 FROM dual)", Sql(c, DbFamily.Oracle));
        }

        [Test]
        public void AliasColumn()
        {
            var c = ParseColumn("1 AS foo");

            Assert.AreEqual("1 AS [foo]", Sql(c, DbFamily.SqlServer));
            Assert.AreEqual("1 AS \"foo\"", Sql(c, DbFamily.Oracle));
        }

        [Test]
        public void PrimitiveColumnWithTable()
        {
            var c = ParseColumn("x.y");

            Assert.AreEqual("[x].[y]", Sql(c, DbFamily.SqlServer));
            Assert.AreEqual("\"x\".\"y\"", Sql(c, DbFamily.Oracle));
        }

        [Test]
        public void PrimitiveColumnWithoutTable()
        {
            var c = ParseColumn("x");

            Assert.AreEqual("[x]", Sql(c, DbFamily.SqlServer));
            Assert.AreEqual("\"x\"", Sql(c, DbFamily.Oracle));
        }
    }
}