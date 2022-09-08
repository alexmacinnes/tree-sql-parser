namespace TreeSqlParser.Writers.Common.SqlServer
{
    public class CommonSqlServerSqlWriter : CommonSqlWriter
    {
        public CommonSqlServerSqlWriter() : base("SQL Server")
        {
            ColumnWriter = new SqlServerColumnWriter(this);
            ConditionWriter = new ConditionWriter(this);
            RelationWriter = new RelationWriter(this, supportsFullJoin: true);
            SelectWriter = new SelectWriter(this);
            IdentifierWriter = new IdentifierWriter('[', ']');
        }
    }
}
