using TreeSqlParser.Writers.Common.Columns.SqlServer;
using TreeSqlParser.Writers.Common.Conditions;
using TreeSqlParser.Writers.Common.Identifiers;
using TreeSqlParser.Writers.Common.Relations;
using TreeSqlParser.Writers.Common.Selects;

namespace TreeSqlParser.Writers.Common
{
    public class CommonSqlServerSqlWriter : CommonSqlWriter
    {
        public CommonSqlServerSqlWriter() : base("SQL Server")
        {
            this.ColumnWriter = new SqlServerColumnWriter(this);
            this.ConditionWriter = new ConditionWriter(this);
            this.RelationWriter = new RelationWriter(this, supportsFullJoin: true);
            this.SelectWriter = new SelectWriter(this);
            this.IdentifierWriter = new IdentifierWriter('[', ']');
        }
    }
}
