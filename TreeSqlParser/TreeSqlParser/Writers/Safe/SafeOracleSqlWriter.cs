using TreeSqlParser.Writers.Safe.Columns.Oracle;
using TreeSqlParser.Writers.Safe.Conditions;
using TreeSqlParser.Writers.Safe.Identifiers;
using TreeSqlParser.Writers.Safe.Relations;
using TreeSqlParser.Writers.Safe.Selects;

namespace TreeSqlParser.Writers.Safe
{
    public class SafeOracleSqlWriter : SafeSqlWriter
    {
        public SafeOracleSqlWriter()
        {
            this.ColumnWriter = new OracleColumnWriter(this);
            this.ConditionWriter = new ConditionWriter(this);
            this.RelationWriter = new RelationWriter(this, supportsFullJoin: true);
            this.SelectWriter = new OracleSelectWriter(this);
            this.IdentifierWriter = new IdentifierWriter('"', '"');
        }
    }
}
