using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Enums;

namespace TreeSqlParser.Model.Analytics
{
    public class Over : SqlElement
    { 
        public List<Column> PartitionBy { get; set; }
        public List<OrderByColumn> OrderBy { get; set; }
        public OverExtent Extent { get; set; }
    }

    public class OverExtent : SqlElement
    {
        public ExtentType ExtentType { get; set; }

        /// <summary>
        /// null indicates UNBOUNDED
        /// 0 indicates CURRENT ROW
        /// Negative indicates rows PRECEDING
        /// Positive indicates rows FOLLOWING
        /// </summary>
        public int? LowerBound { get; set; }

        /// <summary>
        /// null indicates UNBOUNDED
        /// 0 indicates CURRENT ROW
        /// Negative indicates rows PRECEDING
        /// Positive indicates rows FOLLOWING
        /// </summary>
        public int? UpperBound { get; set; }
    }
}
