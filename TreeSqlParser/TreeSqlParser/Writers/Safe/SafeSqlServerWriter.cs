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
            var identifierWriter = new IdentifierWriter('[', ']');

            this.ColumnWriter = new SqlServerColumnWriter(this, identifierWriter);
            this.ConditionWriter = new ConditionWriter(this);
            this.RelationWriter = new RelationWriter(this, identifierWriter, true);
            this.SelectWriter = new SelectWriter(this, identifierWriter);
        }
    }
}
