using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TreeSqlParser.Model;

namespace TreeSqlParser.Writers.Safe.Identifiers
{
    public class IdentifierWriter : IIdentifierWriter
    {
        private readonly char openingDelimiter;

        private readonly char closingDelimiter;

        private readonly char[] illegalCharacters;

        public IdentifierWriter(char openingDelimiter, char closingDelimiter)
        {
            this.openingDelimiter = openingDelimiter;
            this.closingDelimiter = closingDelimiter;

            illegalCharacters = new[] { ';', openingDelimiter, closingDelimiter };
        }

        public string Delimit(SqlIdentifier i)
        {
            foreach (var c in illegalCharacters)
                if (i.Name.Contains(c))
                    throw new InvalidOperationException("Sql Identifier cannot contain: " + c);

            return openingDelimiter + i.Name + closingDelimiter;
        }
    }
}
