using System;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Relations;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Writers.Safe.Columns;
using TreeSqlParser.Writers.Safe.Conditions;
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
 
        public string GenerateSql(SqlElement e)
        {
            if (e is Column col) return ColumnWriter.ColumnSql(col);
            if (e is Condition con) return ConditionWriter.ConditionSql(con);
            if (e is Relation r) return RelationWriter.RelationSql(r);
            if (e is SelectStatement ss) return SelectWriter.SelectStatementSql(ss);
            if (e is Select s) return SelectWriter.SelectSql(s);

            throw new InvalidOperationException($"SqlElement of type {e.GetType().Name} not supported");
        }      
    }
}
