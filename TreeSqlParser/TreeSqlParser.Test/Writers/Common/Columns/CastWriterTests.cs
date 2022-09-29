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
                .WithSql("CAST(NULL AS nvarchar)", SqlWriterType.SqlServer)
                .WithSql("CAST(NULL AS nvarchar2(255))", SqlWriterType.Oracle)
                .WithSql("CAST(NULL AS nchar)", SqlWriterType.MySql)
                .WithSql("CAST(NULL AS nchar)", SqlWriterType.MariaDb)
                .WithSql("CAST(NULL AS nvarchar)", SqlWriterType.Sqlite)
                .WithSql("(NULL)::varchar", SqlWriterType.Postgres)
                .WithSql("CAST(NULL AS nvarchar)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsVarChar()
        {
            var c = ParseColumn("CAST(NULL AS varchar)");

            var expected = new ExpectedSqlResult()
                .WithSql("CAST(NULL AS varchar)", SqlWriterType.SqlServer)
                .WithSql("CAST(NULL AS varchar2(255))", SqlWriterType.Oracle)
                .WithSql("CAST(NULL AS char)", SqlWriterType.MySql)
                .WithSql("CAST(NULL AS char)", SqlWriterType.MariaDb)
                .WithSql("CAST(NULL AS varchar)", SqlWriterType.Sqlite)
                .WithSql("(NULL)::varchar", SqlWriterType.Postgres)
                .WithSql("CAST(NULL AS varchar)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsInt()
        {
            var c = ParseColumn("CAST(NULL AS int)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("CAST(NULL AS int)")
                .WithSql("CAST(NULL AS signed)", SqlWriterType.MySql)
                .WithSql("CAST(NULL AS signed)", SqlWriterType.MariaDb)
                .WithSql("(NULL)::integer", SqlWriterType.Postgres);


            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsReal()
        {
            var c = ParseColumn("CAST(NULL AS real)");

            var expected = new ExpectedSqlResult()
                .WithSql("CAST(NULL AS real)", SqlWriterType.SqlServer)
                .WithSql("CAST(NULL AS number)", SqlWriterType.Oracle)
                .WithSql("CAST(NULL AS decimal)", SqlWriterType.MySql)
                .WithSql("CAST(NULL AS decimal)", SqlWriterType.MariaDb)
                .WithSql("CAST(NULL AS real)", SqlWriterType.Sqlite)
                .WithSql("(NULL)::decimal", SqlWriterType.Postgres)
                .WithSql("CAST(NULL AS double)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsTimestamp()
        {
            var c = ParseColumn("CAST(NULL AS timestamp)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("CAST(NULL AS timestamp)")
                .WithSql("CAST(NULL AS datetime)", SqlWriterType.Oracle)
                .WithSql("CAST(NULL AS datetime)", SqlWriterType.MySql)
                .WithSql("CAST(NULL AS datetime)", SqlWriterType.MariaDb)
                .WithSql("DATETIME(NULL)", SqlWriterType.Sqlite)
                .WithSql("(NULL)::timestamp", SqlWriterType.Postgres)
                .WithSql("CAST(NULL AS timestamp)", SqlWriterType.Db2);

            CommonMother.AssertSql(c, expected);
        }
    }
}
