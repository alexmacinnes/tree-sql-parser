using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Writers.Safe.Identifiers;

namespace TreeSqlParser.Writers.Safe.Selects
{
    public class OracleSelectWriter : SelectWriter
    {
        public OracleSelectWriter(ISqlWriter sqlWriter, IIdentifierWriter identifierWriter) : base(sqlWriter, identifierWriter)
        {
        }

        protected override string FromSql(Select s) => s.From?.Any() == true ? base.FromSql(s) : "FROM dual";
    }
}
