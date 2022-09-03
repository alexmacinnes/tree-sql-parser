using TreeSqlParser.Writers.Common.Columns.Sqlite;
using TreeSqlParser.Writers.Common.Conditions;
using TreeSqlParser.Writers.Common.Identifiers;
using TreeSqlParser.Writers.Common.Relations;
using TreeSqlParser.Writers.Common.Selects;

namespace TreeSqlParser.Writers.Common
{
    public class CommonSqliteSqlWriter : CommonSqlWriter
    {
        public CommonSqliteSqlWriter() : base("SQLite")
        {
            this.ColumnWriter = new SqliteColumnWriter(this);
            this.ConditionWriter = new ConditionWriter(this);
            this.RelationWriter = new RelationWriter(this, supportsRightJoin: false, supportsFullJoin: false);
            this.SelectWriter = new SqliteSelectWriter(this);
            this.IdentifierWriter = new IdentifierWriter('[', ']');
        }
    }
}
