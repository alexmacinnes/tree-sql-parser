using System.Collections.Generic;
using System.Linq;
using TSQL.Tokens;

namespace TreeSqlParser.Parsing.Errors
{
    public class ErrorGenerator
    {
        private IList<TSQLWhitespace> whitespaceTokens;

        public ErrorGenerator(IList<TSQLWhitespace> whitespaceTokens)
        {
            this.whitespaceTokens = whitespaceTokens;
        }

        public TreeSqlParseException ParseException(string message, TSQLToken token)
        {
            if (token == null)
                return new TreeSqlParseException(message, null, null, null);

            int startIndex = token.BeginPosition;

            int lineIndex = 0;

            TSQLWhitespace lastWhitespaceWithNewLine = null;
            foreach (var x in whitespaceTokens)
            {
                if (x.BeginPosition >= token.BeginPosition)
                    break;

                int newLines = x.Text.Count(c => c == '\n');
                if (newLines != 0)
                {
                    lastWhitespaceWithNewLine = x;
                    lineIndex += newLines;
                }
            }

            if (lastWhitespaceWithNewLine == null)
            {
                throw new TreeSqlParseException(message, lineIndex, token.BeginPosition, token.Text);
            }

            //0 is the first space after the last whitespace
            int positionAfterWhitespace = token.BeginPosition - lastWhitespaceWithNewLine.EndPosition - 1;
            // the number of whitespace characters at the start of the current line
            int whitespaceStartingCurrentLine = lastWhitespaceWithNewLine.Text.Length - lastWhitespaceWithNewLine.Text.LastIndexOf('\n') - 1;

            int positionOnLine = whitespaceStartingCurrentLine + positionAfterWhitespace;

            return new TreeSqlParseException(message, lineIndex, positionOnLine, token.Text);
        }
    }
}