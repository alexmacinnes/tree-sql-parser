using TreeSqlParser.Writers.Common.Columns.Oracle;
using TreeSqlParser.Writers.Common.Conditions;
using TreeSqlParser.Writers.Common.Identifiers;
using TreeSqlParser.Writers.Common.Relations;
using TreeSqlParser.Writers.Common.Selects;

namespace TreeSqlParser.Writers.Common
{
    public class CommonOracleSqlWriter : CommonSqlWriter
    {
        public CommonOracleSqlWriter()
        {
            this.ColumnWriter = new OracleColumnWriter(this);
            this.ConditionWriter = new ConditionWriter(this);
            this.RelationWriter = new RelationWriter(this, supportsFullJoin: true);
            this.SelectWriter = new OracleSelectWriter(this);
            this.IdentifierWriter = new IdentifierWriter('"', '"');
        }
    }
}
