using TreeSqlParser.Writers.Safe.Columns.SqlServer;
using TreeSqlParser.Writers.Safe.Conditions;
using TreeSqlParser.Writers.Safe.Identifiers;
using TreeSqlParser.Writers.Safe.Relations;
using TreeSqlParser.Writers.Safe.Selects;

namespace TreeSqlParser.Writers.Safe
{
    public class SafeSqlServerWriter : SafeSqlWriter
    {
        public SafeSqlServerWriter()
        {
            this.ColumnWriter = new SqlServerColumnWriter(this);
            this.ConditionWriter = new ConditionWriter(this);
            this.RelationWriter = new RelationWriter(this, supportsFullJoin: true);
            this.SelectWriter = new SelectWriter(this);
            this.IdentifierWriter = new IdentifierWriter('[', ']');
        }
    }
}
