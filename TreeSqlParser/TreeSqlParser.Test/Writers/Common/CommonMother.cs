using System;
using TreeSqlParser.Model;
using TreeSqlParser.Writers;

namespace TreeSqlParser.Test.Writers.Common
{
    internal class CommonMother
    {
        public static ISqlWriter[] AllCommonWriters => new[]
        {
            SqlWriterFactory.CommonSqlWriter(SqlWriterType.SqlServer),
            SqlWriterFactory.CommonSqlWriter(SqlWriterType.Oracle),
            SqlWriterFactory.CommonSqlWriter(SqlWriterType.MySql),
            SqlWriterFactory.CommonSqlWriter(SqlWriterType.Sqlite)
        };

        public static string Sql(SqlElement element, SqlWriterType writer)
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
    }
}
