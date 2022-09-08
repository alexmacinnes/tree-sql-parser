namespace TreeSqlParser.Writers.Common.MySql
{
    public class MySqlCastWriter : CastWriter
    {
        public MySqlCastWriter(CommonSqlWriter sqlWriter) : base(sqlWriter)
        {
        }

        protected override string Int() => "signed";

        protected override string NVarChar() => "nchar";

        protected override string Real() => "decimal";

        protected override string Timestamp() => "datetime";

        protected override string VarChar() => "char";
    }
}
