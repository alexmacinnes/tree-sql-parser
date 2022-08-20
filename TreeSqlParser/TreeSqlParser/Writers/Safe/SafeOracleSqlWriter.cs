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
            var identifierWriter = new IdentifierWriter('"', '"');

            this.ColumnWriter = new OracleColumnWriter(this, identifierWriter);
            this.ConditionWriter = new ConditionWriter(this);
            this.RelationWriter = new RelationWriter(this, identifierWriter, true);
            this.SelectWriter = new OracleSelectWriter(this, identifierWriter);
        }
    }
}
