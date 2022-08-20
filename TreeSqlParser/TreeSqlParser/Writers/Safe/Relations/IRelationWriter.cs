using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Relations;

namespace TreeSqlParser.Writers.Safe.Relations
{
    public interface IRelationWriter
    {
        public string RelationSql(Relation r);
    }
}
