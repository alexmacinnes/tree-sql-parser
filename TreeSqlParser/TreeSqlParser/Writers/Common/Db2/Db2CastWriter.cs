namespace TreeSqlParser.Writers.Common.Db2
{
    public class Db2CastWriter : CastWriter
    {
        public Db2CastWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Int() => "int";

        protected override string NVarChar() => "nvarchar";

        protected override string Real() => "double";

        protected override string Timestamp() => "timestamp";

        protected override string VarChar() => "varchar";
    }
}
