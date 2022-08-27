using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using TreeSqlParser.Model.Columns;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public abstract class CastWriter : ICastWriter
    {
        private readonly SafeSqlWriter sqlWriter;

        protected CastWriter(SafeSqlWriter sqlWriter)
        {
            this.sqlWriter = sqlWriter;
        }

        private string ColumnSql(Column c) => sqlWriter.ColumnSql(c);

        public virtual string CastSql(CastColumn c)
        {
            if (c.TryCast)
                throw new InvalidOperationException("TRY_CAST not supported");

            return $"CAST({ColumnSql(c.Column)} AS {MapDataType(c.DataType)})";
        }

        protected virtual string MapDataType(ColumnDataType dataType)
        {
            switch (dataType.Value.ToUpperInvariant())
            {
                case "VARCHAR": return VarChar();
                case "NVARCHAR": return NVarChar();
                case "INT": return Int();
                case "REAL": return Real();
                case "TIMESTAMP": return Timestamp();
                default: throw new InvalidOperationException("Inavlid CAST. Valid types are VARCHAR, NVARCHAR, INT, REAL, TIMESTAMP");
            }
        }

        protected abstract string Int();

        protected abstract string NVarChar();

        protected abstract string Real();

        protected abstract string Timestamp();

        protected abstract string VarChar();
    }
}
