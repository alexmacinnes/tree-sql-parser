using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.MySql
{
    public class MySqlMiscellaneousColumnWriter : MiscellaneousColumnWriter
    {
        public MySqlMiscellaneousColumnWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string IifColumnSql(IifColumn c) =>
            $"IF({Sql(c.Condition)}, {Sql(c.TrueColumn)}, {Sql(c.FalseColumn)})";
    }
}
