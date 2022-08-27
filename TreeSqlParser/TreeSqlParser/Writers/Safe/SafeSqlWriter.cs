using System;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Relations;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Writers.Safe.Columns;
using TreeSqlParser.Writers.Safe.Conditions;
using TreeSqlParser.Writers.Safe.Identifiers;
using TreeSqlParser.Writers.Safe.Relations;
using TreeSqlParser.Writers.Safe.Selects;

namespace TreeSqlParser.Writers.Safe
{
    public class SafeSqlWriter : ISqlWriter
    {
        protected IColumnWriter ColumnWriter { get; set; }

        protected IConditionWriter ConditionWriter { get; set; }

        protected IRelationWriter RelationWriter { get; set; }

        protected ISelectWriter SelectWriter { get; set; }

        protected IIdentifierWriter IdentifierWriter { get; set; }  

 
        public virtual string GenerateSql(SqlElement e)
        {
            if (e is Column col) return ColumnSql(col);
            if (e is Condition con) return ConditionSql(con);
            if (e is Relation r) return RelationSql(r);
            if (e is SelectStatement ss) return SelectStatementSql(ss);
            if (e is Select s) return SelectSql(s);

            throw new InvalidOperationException($"SqlElement of type {e.GetType().Name} not supported");
        }

        public virtual string ColumnSql(Column x) => ColumnWriter.ColumnSql(x);

        public virtual string ConditionSql(Condition x) => ConditionWriter.ConditionSql(x);

        public virtual string RelationSql(Relation x) => RelationWriter.RelationSql(x);

        public virtual string SelectStatementSql(SelectStatement x) => SelectWriter.SelectStatementSql(x);

        public virtual string SelectSql(Select x) => SelectWriter.SelectSql(x);

        public virtual string IdentifierSql(SqlIdentifier x) => IdentifierWriter.Delimit(x);
    }
}
