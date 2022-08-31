using TreeSqlParser.Writers.Common.Columns.MySql;
using TreeSqlParser.Writers.Common.Conditions;
using TreeSqlParser.Writers.Common.Identifiers;
using TreeSqlParser.Writers.Common.Relations;
using TreeSqlParser.Writers.Common.Selects;

namespace TreeSqlParser.Writers.Common
{
    public class CommonMySqlSqlWriter : CommonSqlWriter
    { 

        public CommonMySqlSqlWriter() : base("MySQL")
        {
            this.ColumnWriter = new MySqlColumnWriter(this);
            this.ConditionWriter = new ConditionWriter(this);
            this.RelationWriter = new RelationWriter(this, supportsFullJoin: false);
            this.SelectWriter = new MySqlSelectWriter(this);
            this.IdentifierWriter = new IdentifierWriter('`', '`');
        }
    }
}
