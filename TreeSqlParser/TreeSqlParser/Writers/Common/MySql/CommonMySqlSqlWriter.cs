namespace TreeSqlParser.Writers.Common.MySql
{
    public class CommonMySqlSqlWriter : CommonSqlWriter
    {

        public CommonMySqlSqlWriter() : base("MySQL")
        {
            ColumnWriter = new MySqlColumnWriter(this);
            ConditionWriter = new ConditionWriter(this);
            RelationWriter = new RelationWriter(this, supportsFullJoin: false);
            SelectWriter = new MySqlSelectWriter(this);
            IdentifierWriter = new IdentifierWriter('`', '`');
        }
    }
}
