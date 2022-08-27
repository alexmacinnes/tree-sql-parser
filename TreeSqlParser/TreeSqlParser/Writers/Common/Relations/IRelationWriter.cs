using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Relations;

namespace TreeSqlParser.Writers.Common.Relations
{
    public interface IRelationWriter
    {
        string RelationSql(Relation r);
    }
}
