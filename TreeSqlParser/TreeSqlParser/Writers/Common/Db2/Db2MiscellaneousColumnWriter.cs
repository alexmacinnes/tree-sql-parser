using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Db2
{
    public class Db2MiscellaneousColumnWriter : MiscellaneousColumnWriter
    {
        public Db2MiscellaneousColumnWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string IifColumnSql(IifColumn c) =>
            $"IF({Sql(c.Condition)}, {Sql(c.TrueColumn)}, {Sql(c.FalseColumn)})";
    }
}
