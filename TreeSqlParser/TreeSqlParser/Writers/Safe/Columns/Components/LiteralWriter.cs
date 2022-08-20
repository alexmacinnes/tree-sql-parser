using System;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public abstract class LiteralWriter : ILiteralWriter
    {
        public string LiteralSql(LiteralColumnBase c) => c switch
        {
            IntegerColumn x => IntegerSql(x),
            DecimalColumn x => DecimalSql(x),
            StringColumn x => StringSql(x),
            DateColumn x => DateSql(x),
            DateTimeColumn x => DateTimeSql(x),
            _ => throw new NotSupportedException("Unknown sql literal: " + c.GetType().Name)
        };

        protected abstract string DateTimeSql(DateTimeColumn x);

        protected abstract string DateSql(DateColumn x);

        protected abstract string StringSql(StringColumn x);

        protected abstract string DecimalSql(DecimalColumn x);

        protected abstract string IntegerSql(IntegerColumn x);

    }
}
