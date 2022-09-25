using System;
using System.Collections.Generic;
using TreeSqlParser.Writers.Common.Db2;
using TreeSqlParser.Writers.Common.MySql;
using TreeSqlParser.Writers.Common.Oracle;
using TreeSqlParser.Writers.Common.Postgres;
using TreeSqlParser.Writers.Common.Sqlite;
using TreeSqlParser.Writers.Common.SqlServer;
using TreeSqlParser.Writers.Full;

namespace TreeSqlParser.Writers
{
    public static class SqlWriterFactory
    {
        private static readonly IReadOnlyDictionary<SqlWriterType, Func<ISqlWriter>> commonWriters =
            new Dictionary<SqlWriterType, Func<ISqlWriter>>
            {
                { SqlWriterType.SqlServer, () => new CommonSqlServerSqlWriter() },
                { SqlWriterType.Oracle, () => new CommonOracleSqlWriter() },
                { SqlWriterType.MySql, () => new CommonMySqlSqlWriter() },
                { SqlWriterType.Sqlite, () => new CommonSqliteSqlWriter() },
                { SqlWriterType.Postgres, () => new CommonPostgresSqlWriter() },
                { SqlWriterType.Db2, () => new CommonDb2SqlWriter() },
                { SqlWriterType.MariaDb, () => new CommonMySqlSqlWriter() }               // MariaDb syntax is MySql
            };

        /// <summary>A full implementation allowing any SqlElement to be converted into SQL Server SQL </summary>
        public static ISqlWriter FullSqlServerWriter() => new FullSqlServerWriter();

        /// <summary>A limited implementation allowing the most common SQL features
        /// to be translated to other SQL dialects</summary>
        public static ISqlWriter CommonSqlWriter(SqlWriterType sqlWriterType) =>
            commonWriters.TryGetValue(sqlWriterType, out Func<ISqlWriter> func)
            ? func()
            : throw new NotSupportedException("SqlWriterType not recognised: " + sqlWriterType);
    }
}
