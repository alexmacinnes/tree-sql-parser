﻿using System;
using System.Collections.Generic;
using System.Linq;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Enums;

namespace TreeSqlParser.Writers.Common
{
    public class AggregationWriter : IAggregationWriter
    {
        private readonly CommonSqlWriter sqlWriter;

        private static readonly ISet<Aggregation> validAggregations = new HashSet<Aggregation>
        {
            Aggregation.Sum,
            Aggregation.Max,
            Aggregation.Min,
            Aggregation.Avg,
            Aggregation.Count
        };

        public AggregationWriter(CommonSqlWriter sqlWriter)
        {
            this.sqlWriter = sqlWriter;
        }

        public string AggregationSql(AggregatedColumn column)
        {
            if (!validAggregations.Contains(column.Aggregation))
                throw new InvalidOperationException("Limited Aggregation Writer does not support " + column.Aggregation.ToString());

            string agg = column.Aggregation.ToString().ToUpperInvariant();
            string distinct = column.Distinct ? "DISTINCT " : string.Empty;
            string columns = string.Join(", ", column.InnerColumns.Select(sqlWriter.ColumnSql));

            return $"{agg}({distinct}{columns})";
        }
    }
}
