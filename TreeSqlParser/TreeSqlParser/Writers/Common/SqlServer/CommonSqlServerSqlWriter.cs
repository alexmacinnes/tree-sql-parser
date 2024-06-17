namespace TreeSqlParser.Writers.Common.SqlServer
{
    public class CommonSqlServerSqlWriter : CommonSqlWriter
    {
        public CommonSqlServerSqlWriter() : base("SQL Server")
        {
            ColumnWriter = new SqlServerColumnWriter(this);
            ConditionWriter = new ConditionWriter(this);
            RelationWriter = new RelationWriter(this, supportsFullJoin: true);
            SelectWriter = new SqlServerSelectWriter(this);
            IdentifierWriter = new IdentifierWriter('[', ']');
        }
    }
}
