using System;
using System.Collections.Generic;
using TreeSqlParser.Writers;

namespace TreeSqlParser.Test.Writers.Common
{
    internal class ExpectedSqlResult
    {
        public bool SkipUnlistedConversions { get; private set; }
        public string DefaultSql { get; private set; }
        public Dictionary<SqlWriterType, string> ExpectedSqlByType { get; } = new Dictionary<SqlWriterType, string>();

        public ExpectedSqlResult(bool skipUnlistedConversions = false) => 
            SkipUnlistedConversions = skipUnlistedConversions;

        public ExpectedSqlResult WithDefaultSql(string sql)
        {
            if (DefaultSql != null)
                throw new InvalidOperationException();

            DefaultSql = sql;
            return this;
        }

        public ExpectedSqlResult WithSql(string sql, params SqlWriterType[] sqlTypes)
        {
            foreach (var x in sqlTypes)
            {
                if (ExpectedSqlByType.ContainsKey(x))
                    throw new InvalidOperationException();

                ExpectedSqlByType[x] = sql;
            }
            return this;
        }

        public bool HasSqlType(SqlWriterType sqlType) => 
            DefaultSql != null || ExpectedSqlByType.ContainsKey(sqlType);

        public string Sql(SqlWriterType sqlType) =>
            ExpectedSqlByType.TryGetValue(sqlType, out string result) 
            ? result
            : DefaultSql;
    }
}
