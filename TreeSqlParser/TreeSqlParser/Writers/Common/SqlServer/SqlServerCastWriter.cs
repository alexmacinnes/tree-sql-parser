namespace TreeSqlParser.Writers.Common.SqlServer
{
    public class SqlServerCastWriter : CastWriter
    {
        public SqlServerCastWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Int() => "int";

        protected override string NVarChar() => "nvarchar";

        protected override string Real() => "real";

        protected override string Timestamp() => "timestamp";

        protected override string VarChar() => "varchar";
    }
}
