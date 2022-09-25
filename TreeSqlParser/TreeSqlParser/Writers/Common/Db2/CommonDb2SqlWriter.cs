namespace TreeSqlParser.Writers.Common.Db2
{
    public class CommonDb2SqlWriter : CommonSqlWriter
    {
        public CommonDb2SqlWriter() : base("DB2")
        {
            ColumnWriter = new Db2ColumnWriter(this);
            ConditionWriter = new ConditionWriter(this);
            RelationWriter = new RelationWriter(this, supportsRightJoin: true, supportsFullJoin: true);
            SelectWriter = new SelectWriter(this);
            IdentifierWriter = new IdentifierWriter('"', '"');
        }
    }
}
