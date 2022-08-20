﻿using NUnit.Framework;
using System.Collections.Generic;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Writers.Safe;

namespace TreeSqlParser.Writers.Test.Safe.Columns
{
    public class AritmeticWriterTests
    {
        private readonly IReadOnlyDictionary<DbFamily, ISqlWriter> writers = new Dictionary<DbFamily, ISqlWriter>
        {
            { DbFamily.SqlServer, new SafeSqlServerWriter() },
            { DbFamily.Oracle, new SafeOracleSqlWriter() }
        };

        private Column ParseColumn(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select " + sql).Child).Selects[0].Columns[0];

        private string Sql(Column c, DbFamily db) => writers[db].GenerateSql(c);

        private void AssertSql(Column c, DbFamily db, string expected) =>
            Assert.AreEqual(expected, Sql(c, db));

        [Test]
        public void SimpleModulo()
        {
            var c = ParseColumn("1 % 2");

            AssertSql(c, DbFamily.SqlServer, "1%2");
            AssertSql(c, DbFamily.Oracle, "MOD(1, 2)");
        }

        [Test]
        public void SimpleModuloAtEnd()
        {
            var c = ParseColumn("1 + 2 % 3");

            AssertSql(c, DbFamily.SqlServer, "1+2%3");
            AssertSql(c, DbFamily.Oracle, "1+MOD(2, 3)");
        }

        [Test]
        public void ChainedModulo()
        {
            var c = ParseColumn("1 % 2 % 3 % 4");

            AssertSql(c, DbFamily.SqlServer, "1%2%3%4");
            AssertSql(c, DbFamily.Oracle, "MOD(MOD(MOD(1, 2), 3), 4)");
        }

        [Test]
        public void MultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 * 4");

            AssertSql(c, DbFamily.SqlServer, "1*2%3*4");
            AssertSql(c, DbFamily.Oracle, "MOD(1*2, 3)*4");
        }

        [Test]
        public void DoubleMultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 * 4 % 5");

            AssertSql(c, DbFamily.SqlServer, "1*2%3*4%5");
            AssertSql(c, DbFamily.Oracle, "MOD(MOD(1*2, 3)*4, 5)");
        }

        [Test]
        public void DoubleSplitMultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 + 4 % 5");

            AssertSql(c, DbFamily.SqlServer, "1*2%3+4%5");
            AssertSql(c, DbFamily.Oracle, "MOD(1*2, 3)+MOD(4, 5)");
        }

        [Test]
        public void MultiplyChainedModuloWithManyFollowing()
        {
            var c = ParseColumn("1 * 2 % 3 * 4 + 5 * 6");

            AssertSql(c, DbFamily.SqlServer, "1*2%3*4+5*6");
            AssertSql(c, DbFamily.Oracle, "MOD(1*2, 3)*4+5*6");
        }


        [Test]
        public void EmbeddedSingleModulo()
        {
            var c = ParseColumn("1 + 2 % 3 - 4");

            AssertSql(c, DbFamily.SqlServer, "1+2%3-4");
            AssertSql(c, DbFamily.Oracle, "1+MOD(2, 3)-4");
        }

        [Test]
        public void EmbeddedChainedModulo()
        {
            var c = ParseColumn("1 + 2 % 3 % 4 - 5");

            AssertSql(c, DbFamily.SqlServer, "1+2%3%4-5");
            AssertSql(c, DbFamily.Oracle, "1+MOD(MOD(2, 3), 4)-5");
        }
    }
}
