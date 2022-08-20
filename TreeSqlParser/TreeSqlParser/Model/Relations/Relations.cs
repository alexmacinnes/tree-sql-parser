using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Enums;
using TreeSqlParser.Model.Selects;

namespace TreeSqlParser.Model.Relations
{
    public abstract class Relation : SqlElement { }

    public class Table : Relation
    {
        public SqlIdentifier[] Name { get; set; }
        public SqlIdentifier Alias { get; set; }
    }

    public class JoinChain : Relation
    {
        public Relation LeftRelation { get; set; }
        public List<Join> Joins { get; set; }
    }

    public class Join : SqlElement
    {
        public JoinType JoinType { get; set; }
        public Relation RightRelation { get; set; }
        public Condition Condition { get; set; }
    }

    public class BracketedRelation : Relation
    {
        public Relation InnerRelation { get; set; }
    }

    public class SubselectRelation : Relation
    {
        public SelectStatement InnerSelect { get; set; }     
        public SqlIdentifier Alias { get; set; }
    }
}
