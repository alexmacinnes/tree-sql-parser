using System;
using TreeSqlParser.Model;

namespace TreeSqlParser.Writers
{
    public interface ISqlWriter
    {
        string GenerateSql(SqlElement e);
    }
}
