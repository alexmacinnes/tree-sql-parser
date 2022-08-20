using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;

namespace TreeSqlParser.Writers.Safe.Identifiers
{
    public interface IIdentifierWriter
    {
        public string Delimit(SqlIdentifier i);
    }
}
