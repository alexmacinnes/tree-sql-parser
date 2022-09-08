using TreeSqlParser.Model.Relations;

namespace TreeSqlParser.Writers.Common
{
    public interface IRelationWriter
    {
        string RelationSql(Relation r);
    }
}
