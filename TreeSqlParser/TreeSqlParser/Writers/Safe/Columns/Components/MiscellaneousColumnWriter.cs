﻿using System;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Writers.Safe.Identifiers;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public class MiscellaneousColumnWriter : IMiscellaneousColumnWriter
    {
        private readonly string iifFunctionName;

        private readonly ISqlWriter sqlWriter;

        private readonly IIdentifierWriter identifierWriter;

        public MiscellaneousColumnWriter(ISqlWriter sqlWriter, IIdentifierWriter identifierWriter, string iifFunctionName)
        {
            this.sqlWriter = sqlWriter;
            this.identifierWriter = identifierWriter;
            this.iifFunctionName = iifFunctionName;
        }

        private string Sql(SqlElement x) => sqlWriter.GenerateSql(x);

        private string Delimit(SqlIdentifier i) => identifierWriter.Delimit(i);

        public string ColumnSql(Column c) 
        {
            if (c is PrimitiveColumn p)
                return PrimitiveColumnSql(p);
            if (c is AliasColumn a)
                return AliasColumnSql(a);
            if (c is BracketedColumn b)
                return BracketedColumnSql(b);
            if (c is IifColumn i)
                return IifColumnSql(i);
            if (c is CaseColumn cs)
                return CaseColumnSql(cs);
            if (c is SubselectColumn s)
                return SubselectColumnSql(s);
            if (c is StarColumn)
                return StarColumn();
            if (c is NullColumn)
                return NullColumn();

            throw new NotSupportedException("Unsupported column type");
        }

        protected virtual string NullColumn() => "NULL";

        protected virtual string StarColumn() => "*";

        protected virtual string SubselectColumnSql(SubselectColumn c) =>
            "(" + Sql(c.InnerSelect) + ")";

        protected virtual string CaseColumnSql(CaseColumn c)
        {
            var sb = new StringBuilder("CASE");
            foreach (var b in c.Branches)
            {
                sb.Append($" WHEN {Sql(b.Condition)} THEN {Sql(b.Column)}");
            }
            if (c.DefaultColumn != null)
                sb.Append($" ELSE {Sql(c.DefaultColumn)}");
            sb.Append(" END");

            return sb.ToString();
        }

        protected virtual string IifColumnSql(IifColumn c) =>
            $"{iifFunctionName}({Sql(c.Condition)}, {Sql(c.TrueColumn)}, {Sql(c.FalseColumn)})";

        private string BracketedColumnSql(BracketedColumn c) =>
            "(" + Sql(c.InnerColumn) + ")";

        private string AliasColumnSql(AliasColumn c)
        {
            string alias = c.Alias?.Name;
            if (alias == null)
                return Sql(c.InnerColumn);
            else
            {
                string delimited = Delimit(c.Alias);
                return Sql(c.InnerColumn) + " AS " + delimited;
            }
        }

        protected virtual string PrimitiveColumnSql(PrimitiveColumn c) =>
            c.TableAlias?.Name == null ?
            Delimit(c.Name) :
            Delimit(c.TableAlias) + "." + Delimit(c.Name);
    }
}