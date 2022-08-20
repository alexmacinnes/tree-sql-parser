﻿using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model.Conditions;

namespace TreeSqlParser.Writers.Safe.Conditions
{
    public interface IConditionWriter
    {
        string ConditionSql(Condition c);
    }
}