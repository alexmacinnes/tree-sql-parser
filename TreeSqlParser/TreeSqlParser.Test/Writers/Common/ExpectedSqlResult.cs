using System;
using System.Collections.Generic;
using System.Text;
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

        public ExpectedSqlResult WithSql(SqlWriterType sqlType, string sql)
        {
            if (ExpectedSqlByType.ContainsKey(sqlType))
                throw new InvalidOperationException();

            ExpectedSqlByType[sqlType] = sql;
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
