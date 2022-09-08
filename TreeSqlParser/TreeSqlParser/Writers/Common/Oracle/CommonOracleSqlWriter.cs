namespace TreeSqlParser.Writers.Common.Oracle
{
    public class CommonOracleSqlWriter : CommonSqlWriter
    {
        public CommonOracleSqlWriter() : base("Oracle")
        {
            ColumnWriter = new OracleColumnWriter(this);
            ConditionWriter = new ConditionWriter(this);
            RelationWriter = new RelationWriter(this, supportsFullJoin: true);
            SelectWriter = new OracleSelectWriter(this);
            IdentifierWriter = new IdentifierWriter('"', '"');
        }
    }
}
