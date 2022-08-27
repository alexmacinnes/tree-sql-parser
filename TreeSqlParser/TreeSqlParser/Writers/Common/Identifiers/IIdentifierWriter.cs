using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;

namespace TreeSqlParser.Writers.Common.Identifiers
{
    public interface IIdentifierWriter
    {
        string Delimit(SqlIdentifier i);
    }
}
