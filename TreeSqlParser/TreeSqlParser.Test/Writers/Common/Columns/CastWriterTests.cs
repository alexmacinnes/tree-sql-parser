using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;

namespace TreeSqlParser.Writers.Test.Common.Columns
{
    public class CastWriterTests
    {
        private Column ParseColumn(string sql) =>
            (Column) SelectParser.ParseColumn(sql).Child;

        [Test]
        public void CastAsNVarChar()
        {
            var c = ParseColumn("CAST(NULL AS nvarchar)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "CAST(NULL AS nvarchar)")
                .WithSql(SqlWriterType.Oracle, "CAST(NULL AS nvarchar2(255))")
                .WithSql(SqlWriterType.MySql, "CAST(NULL AS nchar)")
                .WithSql(SqlWriterType.MariaDb, "CAST(NULL AS nchar)")
                .WithSql(SqlWriterType.Sqlite, "CAST(NULL AS nvarchar)")
                .WithSql(SqlWriterType.Postgres, "(NULL)::varchar")
                .WithSql(SqlWriterType.Db2, "CAST(NULL AS nvarchar)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsVarChar()
        {
            var c = ParseColumn("CAST(NULL AS varchar)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "CAST(NULL AS varchar)")
                .WithSql(SqlWriterType.Oracle, "CAST(NULL AS varchar2(255))")
                .WithSql(SqlWriterType.MySql, "CAST(NULL AS char)")
                .WithSql(SqlWriterType.MariaDb, "CAST(NULL AS char)")
                .WithSql(SqlWriterType.Sqlite, "CAST(NULL AS varchar)")
                .WithSql(SqlWriterType.Postgres, "(NULL)::varchar")
                .WithSql(SqlWriterType.Db2, "CAST(NULL AS varchar)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsInt()
        {
            var c = ParseColumn("CAST(NULL AS int)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("CAST(NULL AS int)")
                .WithSql(SqlWriterType.MySql, "CAST(NULL AS signed)")
                .WithSql(SqlWriterType.MariaDb, "CAST(NULL AS signed)")
                .WithSql(SqlWriterType.Postgres, "(NULL)::integer");


            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsReal()
        {
            var c = ParseColumn("CAST(NULL AS real)");

            var expected = new ExpectedSqlResult()
                .WithSql(SqlWriterType.SqlServer, "CAST(NULL AS real)")
                .WithSql(SqlWriterType.Oracle, "CAST(NULL AS number)")
                .WithSql(SqlWriterType.MySql, "CAST(NULL AS decimal)")
                .WithSql(SqlWriterType.MariaDb, "CAST(NULL AS decimal)")
                .WithSql(SqlWriterType.Sqlite, "CAST(NULL AS real)")
                .WithSql(SqlWriterType.Postgres, "(NULL)::decimal")
                .WithSql(SqlWriterType.Db2, "CAST(NULL AS double)");

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsTimestamp()
        {
            var c = ParseColumn("CAST(NULL AS timestamp)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("CAST(NULL AS timestamp)")
                .WithSql(SqlWriterType.Oracle, "CAST(NULL AS datetime)")
                .WithSql(SqlWriterType.MySql, "CAST(NULL AS datetime)")
                .WithSql(SqlWriterType.MariaDb, "CAST(NULL AS datetime)")
                .WithSql(SqlWriterType.Sqlite, "DATETIME(NULL)")
                .WithSql(SqlWriterType.Postgres, "(NULL)::timestamp")
                .WithSql(SqlWriterType.Db2, "CAST(NULL AS timestamp)");

            CommonMother.AssertSql(c, expected);
        }
    }
}
