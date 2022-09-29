using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TreeSqlParser.Model;
using TreeSqlParser.Writers;

namespace TreeSqlParser.Test.Writers.Common
{
    internal class CommonMother
    {
        internal static IEnumerable<SqlWriterType> AllCommonWriterTypes =>
            Enum.GetValues(typeof(SqlWriterType)).Cast<SqlWriterType>();

        internal static string Sql(SqlElement element, SqlWriterType writer)
        {
            try
            {
                return SqlWriterFactory.CommonSqlWriter(writer).GenerateSql(element);
            }
            catch (Exception e)
            {
                return "EXCEPTION: " + e.Message;
            }
        }     
        
        public static void AssertSql(SqlElement element, ExpectedSqlResult expected)
        {
            var types = AllCommonWriterTypes;
            if (expected.SkipUnlistedConversions)
                types = types.Where(expected.HasSqlType);

            foreach (var x in types)
            {
                string actualSql = Sql(element, x);
                string expectedSql = expected.Sql(x);
                Assert.AreEqual(expectedSql, actualSql, "SqlType: " + x.ToString());
            }
        }
    }
}
