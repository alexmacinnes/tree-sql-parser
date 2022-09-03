using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Writers.Common.Columns.Components;

namespace TreeSqlParser.Writers.Common.Columns.Sqlite
{
    public class SqliteMiscellaneousColumnWriter : MiscellaneousColumnWriter
    {
        public SqliteMiscellaneousColumnWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string IifColumnSql(IifColumn c) =>
            $"CASE WHEN {Sql(c.Condition)} THEN {Sql(c.TrueColumn)} ELSE {Sql(c.FalseColumn)} END";
    }
}
