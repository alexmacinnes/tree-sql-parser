using System;
using TreeSqlParser.Model;
using TreeSqlParser.Writers;

namespace TreeSqlParser.Test.Writers.Common
{
    internal class CommonMother
    {
        public static ISqlWriter[] AllCommonWriters => new[]
        {
            SqlWriterFactory.CommonSqlServerWriter(SqlWriterType.SqlServer),
            SqlWriterFactory.CommonSqlServerWriter(SqlWriterType.Oracle),
            SqlWriterFactory.CommonSqlServerWriter(SqlWriterType.MySql),
            SqlWriterFactory.CommonSqlServerWriter(SqlWriterType.Sqlite)
        };

        public static string Sql(SqlElement element, SqlWriterType writer)
        {
            try
            {
                return SqlWriterFactory.CommonSqlServerWriter(writer).GenerateSql(element);
            }
            catch (Exception e)
            {
                return "EXCEPTION: " + e.Message;
            }
        }       
    }
}
