using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Common.Sqlite
{ 
    public class SqliteMiscellaneousColumnWriter : MiscellaneousColumnWriter
    {
        public SqliteMiscellaneousColumnWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        { }

        protected override string IifColumnSql(IifColumn c) =>
            $"CASE WHEN {Sql(c.Condition)} THEN {Sql(c.TrueColumn)} ELSE {Sql(c.FalseColumn)} END";
    }
}
