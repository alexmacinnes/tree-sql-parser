namespace TreeSqlParser.Writers.Common.Postgres
{
    public class CommonPostgresSqlWriter : CommonSqlWriter
    {
        public CommonPostgresSqlWriter() : base("Postgres")
        {
            ColumnWriter = new PostgresColumnWriter(this);
            ConditionWriter = new ConditionWriter(this);
            RelationWriter = new RelationWriter(this, supportsFullJoin: true);
            SelectWriter = new PostgresSelectWriter(this);
            IdentifierWriter = new IdentifierWriter('"', '"');
        }
    }
}
