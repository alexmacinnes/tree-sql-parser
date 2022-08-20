﻿using System;
using System.Diagnostics;
using System.Linq;
using TreeSqlParser.Model.Columns;
using TreeSqlParser.Model.Enums;

namespace TreeSqlParser.Writers.Safe.Columns.Components
{
    public abstract class ArithmeticWriter : IArithmeticWriter
    {
        private readonly IColumnWriter columnWriter;

        protected ArithmeticWriter(IColumnWriter columnWriter)
        {
            this.columnWriter = columnWriter;
        }

        protected abstract string ModuloReplacementFunction { get; }

        public string ArithmeticSql(ArithmeticChain a)
        {
            Column element = ModuloReplacementFunction == null ? a : ReplaceModulo(a);
            string sql = ColumnSql(element);

            return sql;

            // see operator precedence
            // https://docs.microsoft.com/en-us/sql/t-sql/language-elements/operator-precedence-transact-sql?view=sql-server-ver15

            // translate '1 + 2 * 3 / 4 % 5 * 6'
            // into      '1 + {modfunction}(2 * 3 / 4, 5) * 6
        }

        private Column ReplaceModulo(Column e)
        {
            if (!(e is ArithmeticChain a))
                return e;

            int lastIndex = a.Operations.FindLastIndex(x => x.Operator == ArithmeticOperator.Modulo);
            if (lastIndex == -1)
                return a;

            int firstChainedElement = lastIndex;
            for (int i = lastIndex - 1; i >= 0; i--)
            {
                var op = a.Operations[i].Operator;
                if (op == ArithmeticOperator.Multiply || op == ArithmeticOperator.Divide || op == ArithmeticOperator.Modulo)
                    firstChainedElement = i;
                else
                    break;
            }

            if (firstChainedElement == 0)
            {
                // everything upto and including the last modulo operation
                var customModulo = new CustomModuloFunction()
                {
                    LeftColumn = new ArithmeticChain()
                    {
                        LeftColumn = a.LeftColumn,
                        Operations = a.Operations.Take(lastIndex).ToList()
                    },
                    RightColumn = a.Operations[lastIndex].RightColumn
                };
                customModulo.LeftColumn = ReplaceModulo(customModulo.LeftColumn);

                if (lastIndex == a.Operations.Count - 1)
                    // modulo is final operation and all preceding operations are of same precedence
                    // 1 * 2 / 3 % 4  =>  MOD(1 * 2 / 3, 4)
                    return customModulo;
                else
                {
                    // modulo is notfinal operation and all preceding operations are of same precedence
                    // 1 * 2 / 3 % 4 + 5  =>  MOD(1 * 2 / 3, 4) + 5
                    var result = new ArithmeticChain
                    {
                        LeftColumn = customModulo,
                        Operations = a.Operations.Skip(lastIndex + 1).ToList()
                    };
                    return result;
                }

            }
            else
            {
                var arithChain = new ArithmeticChain
                {
                    LeftColumn = a.LeftColumn,
                    Operations = a.Operations.Take(firstChainedElement - 1).ToList()
                };
                var customModulo = new CustomModuloFunction
                {
                    LeftColumn = new ArithmeticChain()
                    {
                        LeftColumn = a.Operations[firstChainedElement - 1].RightColumn,
                        Operations = a.Operations.Skip(firstChainedElement).Take(lastIndex - firstChainedElement).ToList()
                    },
                    RightColumn = a.Operations[lastIndex].RightColumn
                };
                customModulo.LeftColumn = ReplaceModulo(customModulo.LeftColumn);

                var op = new ArithmeticOperation
                {
                    Operator = a.Operations[firstChainedElement - 1].Operator,
                    RightColumn = customModulo
                };
                arithChain.Operations.Add(op);
                arithChain.Operations.AddRange(a.Operations.Skip(lastIndex + 1));

                var result = ReplaceModulo(arithChain);
                return result;
            }
        }

        private string OperatorText(ArithmeticOperator x) => x switch
        {
            ArithmeticOperator.Plus => "+",
            ArithmeticOperator.Minus => "-",
            ArithmeticOperator.Multiply => "*",
            ArithmeticOperator.Divide => "/",
            ArithmeticOperator.Modulo => "%",
            _ => throw new NotSupportedException("Unknwn ArithmeticOperator: " + x)
        };

        private string ColumnSql(Column c) => c switch
        {
            CustomModuloFunction x => $"{ModuloReplacementFunction}({ColumnSql(x.LeftColumn)}, {ColumnSql(x.RightColumn)})",
            ArithmeticChain x =>
                ColumnSql(x.LeftColumn) +
                string.Join(
                    string.Empty,
                    x.Operations.Select(o => OperatorText(o.Operator) + ColumnSql(o.RightColumn))),
            _ => columnWriter.ColumnSql(c)
        };

        [DebuggerDisplay("{DebuggerDisplay}")]
        private class CustomModuloFunction : Column
        {
            public Column LeftColumn { get; set; }

            public Column RightColumn { get; set; }

            public override string DebuggerDisplay =>
                $"CUSTOM_MODULO({LeftColumn.DebuggerDisplay}, {RightColumn.DebuggerDisplay})";
        }
    }
}
