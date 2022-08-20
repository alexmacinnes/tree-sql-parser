using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;

namespace TreeSqlParser.Test.Model.SqlElementConventions_Tests
{
    internal class FooSimpleSqlElement : SqlElement
    {
        public string Name { get; set; }

        public FooSimpleSqlElement() { }

        public FooSimpleSqlElement(string name) { this.Name = name; }

        public override string ToString() => Name;
    }

    internal class DerivedSimpleSqlElement : FooSimpleSqlElement
    {
        public DerivedSimpleSqlElement() { }

        public DerivedSimpleSqlElement(string name) : base(name)
        { }
    }

    internal class FooComplexSqlElement : SqlElement
    {
        public SqlElement Element1 { get; set; }

        public SqlElement Element2 { get; set; }

        public List<SqlElement> List1 { get; set; }

        public List<SqlElement> List2 { get; set; }
    }

    internal class DerivedComplexSqlElement : FooComplexSqlElement
    { }

    internal class PrivateMembersSqlElement : SqlElement
    {
        public SqlElement ReadOnly { get; } = new FooSimpleSqlElement("a");

        public SqlElement PrivateSet { get; private set; } = new FooSimpleSqlElement("b");

        public SqlElement PrivateGet { private get; set; } = new FooSimpleSqlElement("c");

#pragma warning disable IDE0051 // Remove unused private members
        private SqlElement Private { get; set; } = new FooSimpleSqlElement("d");
#pragma warning restore IDE0051 // Remove unused private members

        protected SqlElement Protected { get; set; } = new FooSimpleSqlElement("e");

        public SqlElement ProtectedSet { get; protected set; } = new FooSimpleSqlElement("f");
    }

    internal class StringMembersSqlElement : SqlElement
    {
        public string String1 { get; set; }

        public List<string> List1 { get; set; }
    }

    internal class DerivedElementContainer : SqlElement
    {
        public DerivedSimpleSqlElement DerivedElement1 { get; set; }

        public List<DerivedSimpleSqlElement> DerivedList1 { get; set; }
    }

    internal class CloneableMembersContainer : SqlElement
    {
        public FooCloneable Item1 { get; set; }

        public FooCloneable Item2 { get; set; }

        public List<FooCloneable> List1 { get; set; }

        public List<FooCloneable> List2 { get; set; }
    }

    internal class FooCloneable : ICloneable
    {
        public string Name { get; set; }

        public object Clone() => new FooCloneable { Name = Name };
    }
}
