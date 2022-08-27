using System;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public abstract class LiteralWriter : ILiteralWriter
    {
        public string LiteralSql(LiteralColumnBase c) 
        {
            if (c is IntegerColumn i)
                return IntegerSql(i);
            if (c is DecimalColumn dc)
                return DecimalSql(dc);
            if (c is StringColumn s)
                return StringSql(s);
            if (c is DateColumn d)
                return DateSql(d);
            if (c is DateTimeColumn dt)
                return DateTimeSql(dt);

            throw new NotSupportedException("Unknown sql literal: " + c.GetType().Name);
        }

        protected abstract string DateTimeSql(DateTimeColumn x);

        protected abstract string DateSql(DateColumn x);

        protected abstract string StringSql(StringColumn x);

        protected abstract string DecimalSql(DecimalColumn x);

        protected abstract string IntegerSql(IntegerColumn x);

    }
}
