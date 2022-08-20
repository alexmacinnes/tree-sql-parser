using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Writers.Safe.Selects
{
    public interface ISelectWriter
    {
        public string SelectStatementSql(SelectStatement s);

        public string SelectSql(Select s);
    }
}
