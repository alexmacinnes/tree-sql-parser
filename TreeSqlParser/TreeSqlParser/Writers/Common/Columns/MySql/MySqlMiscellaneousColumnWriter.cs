using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Writers.Common.Columns.Components;

namespace TreeSqlParser.Writers.Common.Columns.MySql
{
    public class MySqlMiscellaneousColumnWriter : MiscellaneousColumnWriter
    {
        public MySqlMiscellaneousColumnWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string IifColumnSql(IifColumn c) =>
            $"IF({Sql(c.Condition)}, {Sql(c.TrueColumn)}, {Sql(c.FalseColumn)})";
    }
}
