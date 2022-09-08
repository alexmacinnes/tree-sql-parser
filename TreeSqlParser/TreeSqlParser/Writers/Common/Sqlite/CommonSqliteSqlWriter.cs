namespace TreeSqlParser.Writers.Common.Sqlite
{
    public class CommonSqliteSqlWriter : CommonSqlWriter
    {
        public CommonSqliteSqlWriter() : base("SQLite")
        {
            ColumnWriter = new SqliteColumnWriter(this);
            ConditionWriter = new ConditionWriter(this);
            RelationWriter = new RelationWriter(this, supportsRightJoin: false, supportsFullJoin: false);
            SelectWriter = new SqliteSelectWriter(this);
            IdentifierWriter = new IdentifierWriter('[', ']');
        }
    }
}
