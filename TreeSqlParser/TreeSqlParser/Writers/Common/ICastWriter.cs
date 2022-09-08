using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common
{
    public interface ICastWriter
    {
        string CastSql(CastColumn c);
    }
}
