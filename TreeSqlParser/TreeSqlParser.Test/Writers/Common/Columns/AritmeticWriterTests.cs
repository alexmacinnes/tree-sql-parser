using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class AritmeticWriterTests
    {
        private Column ParseColumn(string sql) =>
            ((SelectStatement)SelectParser.ParseSelectStatement("select " + sql).Child).Selects[0].Columns[0];

        private string Sql(Column c, SqlWriterType db) => CommonMother.Sql(c, db);

        private void AssertSql(Column c, SqlWriterType db, string expected) =>
            Assert.AreEqual(expected, Sql(c, db));

        [Test]
        public void SimpleModulo()
        {
            var c = ParseColumn("1 % 2");

            AssertSql(c, SqlWriterType.SqlServer, "1%2");
            AssertSql(c, SqlWriterType.Oracle, "MOD(1, 2)");
            AssertSql(c, SqlWriterType.MySql, "1%2");
            AssertSql(c, SqlWriterType.Sqlite, "1%2");
            AssertSql(c, SqlWriterType.Postgres, "1%2");
        }

        [Test]
        public void SimpleModuloAtEnd()
        {
            var c = ParseColumn("1 + 2 % 3");

            AssertSql(c, SqlWriterType.SqlServer, "1+2%3");
            AssertSql(c, SqlWriterType.Oracle, "1+MOD(2, 3)");
            AssertSql(c, SqlWriterType.MySql, "1+2%3");
            AssertSql(c, SqlWriterType.Sqlite, "1+2%3");
            AssertSql(c, SqlWriterType.Postgres, "1+2%3");
        }

        [Test]
        public void ChainedModulo()
        {
            var c = ParseColumn("1 % 2 % 3 % 4");

            AssertSql(c, SqlWriterType.SqlServer, "1%2%3%4");
            AssertSql(c, SqlWriterType.Oracle, "MOD(MOD(MOD(1, 2), 3), 4)");
            AssertSql(c, SqlWriterType.MySql, "1%2%3%4");
            AssertSql(c, SqlWriterType.Sqlite, "1%2%3%4");
            AssertSql(c, SqlWriterType.Postgres, "1%2%3%4");
        }

        [Test]
        public void MultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 * 4");

            AssertSql(c, SqlWriterType.SqlServer, "1*2%3*4");
            AssertSql(c, SqlWriterType.Oracle, "MOD(1*2, 3)*4");
            AssertSql(c, SqlWriterType.MySql, "1*2%3*4");
            AssertSql(c, SqlWriterType.Sqlite, "1*2%3*4");
            AssertSql(c, SqlWriterType.Postgres, "1*2%3*4");
        }

        [Test]
        public void DoubleMultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 * 4 % 5");

            AssertSql(c, SqlWriterType.SqlServer, "1*2%3*4%5");
            AssertSql(c, SqlWriterType.Oracle, "MOD(MOD(1*2, 3)*4, 5)");
            AssertSql(c, SqlWriterType.MySql, "1*2%3*4%5");
            AssertSql(c, SqlWriterType.Sqlite, "1*2%3*4%5");
            AssertSql(c, SqlWriterType.Postgres, "1*2%3*4%5");
        }

        [Test]
        public void DoubleSplitMultiplyChainedModulo()
        {
            var c = ParseColumn("1 * 2 % 3 + 4 % 5");

            AssertSql(c, SqlWriterType.SqlServer, "1*2%3+4%5");
            AssertSql(c, SqlWriterType.Oracle, "MOD(1*2, 3)+MOD(4, 5)");
            AssertSql(c, SqlWriterType.MySql, "1*2%3+4%5");
            AssertSql(c, SqlWriterType.Sqlite, "1*2%3+4%5");
            AssertSql(c, SqlWriterType.Postgres, "1*2%3+4%5");
        }

        [Test]
        public void MultiplyChainedModuloWithManyFollowing()
        {
            var c = ParseColumn("1 * 2 % 3 * 4 + 5 * 6");

            AssertSql(c, SqlWriterType.SqlServer, "1*2%3*4+5*6");
            AssertSql(c, SqlWriterType.Oracle, "MOD(1*2, 3)*4+5*6");
            AssertSql(c, SqlWriterType.MySql, "1*2%3*4+5*6");
            AssertSql(c, SqlWriterType.Sqlite, "1*2%3*4+5*6");
            AssertSql(c, SqlWriterType.Postgres, "1*2%3*4+5*6");
        }


        [Test]
        public void EmbeddedSingleModulo()
        {
            var c = ParseColumn("1 + 2 % 3 - 4");

            AssertSql(c, SqlWriterType.SqlServer, "1+2%3-4");
            AssertSql(c, SqlWriterType.Oracle, "1+MOD(2, 3)-4");
            AssertSql(c, SqlWriterType.MySql, "1+2%3-4");
            AssertSql(c, SqlWriterType.Sqlite, "1+2%3-4");
            AssertSql(c, SqlWriterType.Postgres, "1+2%3-4");
        }

        [Test]
        public void EmbeddedChainedModulo()
        {
            var c = ParseColumn("1 + 2 % 3 % 4 - 5");

            AssertSql(c, SqlWriterType.SqlServer, "1+2%3%4-5");
            AssertSql(c, SqlWriterType.Oracle, "1+MOD(MOD(2, 3), 4)-5");
            AssertSql(c, SqlWriterType.MySql, "1+2%3%4-5");
            AssertSql(c, SqlWriterType.Sqlite, "1+2%3%4-5");
            AssertSql(c, SqlWriterType.Postgres, "1+2%3%4-5");
        }
    }
}
