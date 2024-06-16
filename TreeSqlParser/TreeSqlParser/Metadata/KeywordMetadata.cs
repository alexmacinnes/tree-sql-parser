using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TreeSqlParser.Parsing;
using TSQL;
using TSQL.Tokens;

namespace TreeSqlParser.Metadata
{
    internal class KeywordMetadata
    {
        internal static void CheckKeywordBlacklist(List<TSQLToken> tokens, ErrorGenerator errorGenerator)
        {
            var keywords = tokens.OfType<TSQLKeyword>().Select(x => x.Keyword).Distinct().ToArray();
            var black = keywords
                .Intersect(KeywordMetadata.BlackList)
                .Select(KeywordName)
                .OrderBy(x => x)
                .ToArray();

            if (black.Any())
                throw new InvalidOperationException("The following keywords are not supported: " + string.Join(", ", black));
        }

        private static string KeywordName(TSQLKeywords k)
        {
            Type t = typeof(TSQLKeywords);
            var field = t.GetField("Keyword", BindingFlags.NonPublic | BindingFlags.Instance);
            string keyword = (string)field.GetValue(k);
            return keyword;
        }

        public static HashSet<TSQLKeywords> WhiteList = new HashSet<TSQLKeywords>
        {
            TSQLKeywords.ALL,
            TSQLKeywords.AND,
            TSQLKeywords.ANY,
            TSQLKeywords.AS,
            TSQLKeywords.ASC,
            TSQLKeywords.BETWEEN,
            TSQLKeywords.BY,
            TSQLKeywords.CASE,
            TSQLKeywords.FETCH,
            TSQLKeywords.FOR,
            TSQLKeywords.FROM,
            TSQLKeywords.FULL,
            TSQLKeywords.GROUP,
            TSQLKeywords.HAVING,
            TSQLKeywords.IN,
            TSQLKeywords.INNER,
            TSQLKeywords.INTERSECT,
            TSQLKeywords.IS,
            TSQLKeywords.JOIN,
            TSQLKeywords.LIKE,
            TSQLKeywords.NOT,
            TSQLKeywords.NULL,
            TSQLKeywords.ON,
            TSQLKeywords.OPTION,
            TSQLKeywords.OR,
            TSQLKeywords.ORDER,
            TSQLKeywords.OUTER,
            TSQLKeywords.OVER,
            TSQLKeywords.PERCENT,
            TSQLKeywords.PIVOT,
            TSQLKeywords.WITH,
        };

        public static HashSet<TSQLKeywords> BlackList = new HashSet<TSQLKeywords>
        {
            TSQLKeywords.ADD,               // ALTER TABLE
            TSQLKeywords.ALTER,             //         
            TSQLKeywords.AUTHORIZATION,     // ALTER AUTHORIZATION
            TSQLKeywords.BACKUP,            // BACKUP DB
            TSQLKeywords.BEGIN,             // PROC  
            TSQLKeywords.BREAK,             // PROC
            TSQLKeywords.BROWSE,            // ?
            TSQLKeywords.BULK,              // INSERT
            TSQLKeywords.CASCADE,           // UPDATE/DELETE
            TSQLKeywords.CHECK,             // CONSTRAINTS    
            TSQLKeywords.CHECKPOINT,        // CHECKPOINT
            TSQLKeywords.FILE,              // ?
            TSQLKeywords.FILLFACTOR,        // INDEX   
            TSQLKeywords.FOREIGN,           // INDEX
            TSQLKeywords.FUNCTION,          // CREATE FUNCTION
            TSQLKeywords.GET,               // ?
            TSQLKeywords.GO,                // PROC  
            TSQLKeywords.GOTO,              // PROC
            TSQLKeywords.GRANT,             // PERMISSIONS
            TSQLKeywords.HOLDLOCK,          // TABLE HINT
            TSQLKeywords.IDENTITY,          // CREATE COLUMN
            TSQLKeywords.IDENTITYCOL,       // ?
            TSQLKeywords.IDENTITY_INSERT,   // ?
            TSQLKeywords.IF,                // PROC
            TSQLKeywords.INDEX,             // CREATE INDEX
            TSQLKeywords.INSERT,            // INSERT
            TSQLKeywords.INTO,              // INTO new table
            TSQLKeywords.KEY,               // CREATE KEY
            TSQLKeywords.LINENO,            // ?
            TSQLKeywords.LOAD,              // ?
            TSQLKeywords.MERGE,             // INSERT/UPDATE/DELETE
            TSQLKeywords.MOVE,              // ?
            TSQLKeywords.NOCHECK,           // ALTER TABLE
            TSQLKeywords.NONCLUSTERED,      // INDEX
            TSQLKeywords.OFF,               // SETTINGS
            TSQLKeywords.OFFSETS,           // SETTINGS
            TSQLKeywords.OPEN,              // CURSOR
            TSQLKeywords.PLAN,              // ?
            TSQLKeywords.PRECISION,         // ?    
            TSQLKeywords.PRIMARY,           // INDEX
            TSQLKeywords.PRINT,             // PROC
            TSQLKeywords.PROCEDURE,         // PROC
            TSQLKeywords.PUBLIC,            // ?
            TSQLKeywords.READ,              // PROC
            TSQLKeywords.READTEXT,          // OBSOLETE
            TSQLKeywords.RECEIVE,           // ?
            TSQLKeywords.RECONFIGURE,       // PROC
            TSQLKeywords.REFERENCES,        // ?
            TSQLKeywords.REPEATABLE,        // PROC
            TSQLKeywords.REPLICATION,       // ?
            TSQLKeywords.RESTORE,           // RESTORE DB
            TSQLKeywords.RETURN,            // PROC  
            TSQLKeywords.RETURNS,           // ?
            TSQLKeywords.REVERT,            // PROC
            TSQLKeywords.REVOKE,            // PERMISSIONS
            TSQLKeywords.WHILE,             // PROC
            TSQLKeywords.WITHIN,            // ?
            TSQLKeywords.WRITETEXT          // UPDATE
        };
    }
}
