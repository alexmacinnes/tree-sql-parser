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
 
        public string GenerateSql(SqlElement e) => e switch
        {
            Column x => ColumnWriter.ColumnSql(x),
            Condition x => ConditionWriter.ConditionSql(x),
            Relation x => RelationWriter.RelationSql(x),
            SelectStatement x => SelectWriter.SelectStatementSql(x),
            Select x => SelectWriter.SelectSql(x),
            _ => throw new InvalidOperationException($"SqlElement of type {e.GetType().Name} not supported")
        };       
    }
}
