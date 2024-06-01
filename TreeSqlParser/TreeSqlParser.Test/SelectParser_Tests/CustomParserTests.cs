using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TreeSqlParser.Model;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Conditions;
using TreeSqlParser.Model.Selects;
using TreeSqlParser.Parsing;

namespace TreeSqlParser.Test.SelectParser_Tests
{
    internal class CustomParserTests
    {
        private class CustomColumnParser : ColumnParser
        {
            public override Column ParseNextColumnSegment(SqlElement parent, TokenList tokenList)
            {
                var t = tokenList.Peek();
                if (t.Text == "$baz")
                {
                    tokenList.Advance();
                    return new FooColumn() { Parent = parent };
                }
                return base.ParseNextColumnSegment(parent, tokenList);
            }
        }
        private class CustomConditionParser : ConditionParser
        {
            public override Condition ParseNextCondition(SqlElement parent, TokenList tokenList)
            {
                var t = tokenList.Peek();
                if (t.Text == "$foo")
                {
                    tokenList.Advance();
                    return new FooCondition() { Parent = parent };
                }

                return base.ParseNextCondition(parent, tokenList);
            }
        }

        private class CustomSelectParser : SelectParser
        {
            public CustomSelectParser() : base()
            {
                ConditionParser = new CustomConditionParser() { SelectParser = this };
                ColumnParser = new CustomColumnParser() { SelectParser = this };
            }
        }

        private class FooCondition : Condition
        {
            public override string DebuggerDisplay => "FOO = BAR";
        }

        private class FooColumn : Column
        {
            public override string DebuggerDisplay => "BAZBAZBAZ()";
        }

        [Test]
        public void ParseCustomCondition()
        {
            string sql = "select 1 where $foo";
            var root = new CustomSelectParser().ParseSelectStatement(sql);
            var statement = (SelectStatement) root.Child;
            var condition = statement.Selects[0].WhereCondition;

            Assert.IsInstanceOf<FooCondition>(condition);
            Assert.AreEqual("FOO = BAR", condition.DebuggerDisplay);
        }

        [Test]
        public void ParseEmbeddedCustomCondition()
        {
            string sql = "select 1 where a=b and $foo or c=d";
            var root = new CustomSelectParser().ParseSelectStatement(sql);
            var statement = (SelectStatement)root.Child;
            var condition = statement.Selects[0].WhereCondition;

            Assert.AreEqual("a Equals b And FOO = BAR Or c Equals d", condition.DebuggerDisplay);
        }

        [Test]
        public void ParseCustomColumn()
        {
            string sql = "select $baz";
            var root = new CustomSelectParser().ParseSelectStatement(sql);
            var statement = (SelectStatement)root.Child;
            var column = statement.Selects[0].Columns[0];

            Assert.IsInstanceOf<FooColumn>(column);
            Assert.AreEqual("BAZBAZBAZ()", column.DebuggerDisplay);
        }

        [Test]
        public void ParseEmbeddedCustomColumn()
        {
            string sql = "select myfunc(1, $baz, 3)";
            var root = new CustomSelectParser().ParseSelectStatement(sql);
            var statement = (SelectStatement)root.Child;
            var funcColumn = (FunctionColumn)statement.Selects[0].Columns[0];

            Assert.IsInstanceOf<FooColumn>(funcColumn.Parameters[1]);
            Assert.AreEqual("myfunc(1, BAZBAZBAZ(), 3)", funcColumn.DebuggerDisplay);
        }
    }
}
