﻿using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public interface IAggregationWriter
    {
        string AggregationSql(AggregatedColumn column);
    }
}
