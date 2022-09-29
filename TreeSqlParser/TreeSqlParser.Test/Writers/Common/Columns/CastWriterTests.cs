using NUnit.Framework;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Parsing;
using TreeSqlParser.Test.Writers.Common;
using swt = TreeSqlParser.Writers.SqlWriterType;

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
                .WithSql("CAST(NULL AS nvarchar)", swt.SqlServer, swt.Sqlite, swt.Db2)
                .WithSql("CAST(NULL AS nvarchar2(255))", swt.Oracle)
                .WithSql("CAST(NULL AS nchar)", swt.MySql, swt.MariaDb)
                .WithSql("(NULL)::varchar", swt.Postgres); ;

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsVarChar()
        {
            var c = ParseColumn("CAST(NULL AS varchar)");

            var expected = new ExpectedSqlResult()
                .WithSql("CAST(NULL AS varchar)", swt.SqlServer, swt.Sqlite, swt.Db2)
                .WithSql("CAST(NULL AS varchar2(255))", swt.Oracle)
                .WithSql("CAST(NULL AS char)", swt.MySql, swt.MariaDb)
                .WithSql("(NULL)::varchar", swt.Postgres);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsInt()
        {
            var c = ParseColumn("CAST(NULL AS int)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("CAST(NULL AS int)")
                .WithSql("CAST(NULL AS signed)", swt.MySql, swt.MariaDb)
                .WithSql("(NULL)::integer", SqlWriterType.Postgres);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsReal()
        {
            var c = ParseColumn("CAST(NULL AS real)");

            var expected = new ExpectedSqlResult()
                .WithSql("CAST(NULL AS real)", swt.SqlServer, swt.Sqlite)
                .WithSql("CAST(NULL AS number)", swt.Oracle)
                .WithSql("CAST(NULL AS decimal)", swt.MySql, swt.MariaDb)
                .WithSql("(NULL)::decimal", swt.Postgres)
                .WithSql("CAST(NULL AS double)", swt.Db2);

            CommonMother.AssertSql(c, expected);
        }

        [Test]
        public void CastAsTimestamp()
        {
            var c = ParseColumn("CAST(NULL AS timestamp)");

            var expected = new ExpectedSqlResult()
                .WithDefaultSql("CAST(NULL AS timestamp)")
                .WithSql("CAST(NULL AS datetime)", swt.Oracle, swt.MySql, swt.MariaDb)
                .WithSql("DATETIME(NULL)", swt.Sqlite)
                .WithSql("(NULL)::timestamp", swt.Postgres);

            CommonMother.AssertSql(c, expected);
        }
    }
}
