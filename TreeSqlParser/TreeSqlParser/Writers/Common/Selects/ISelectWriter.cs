using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Writers.Common.Selects
{
    public interface ISelectWriter
    {
        string SelectStatementSql(SelectStatement s);

        string SelectSql(Select s);
    }
}
