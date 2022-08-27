using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Conditions;

namespace TreeSqlParser.Writers.Common.Conditions
{
    public interface IConditionWriter
    {
        string ConditionSql(Condition c);
    }
}
