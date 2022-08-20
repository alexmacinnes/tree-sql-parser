using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TreeSqlParser.Model
{
    public abstract class SqlElement : ICloneable
    {
        [JsonIgnore]
        public virtual SqlElement Parent { get; set; }

        [JsonIgnore]
        public IEnumerable<SqlElement> Children => SqlElementConventions.GetChildren(this);

        public bool TryReplaceChild(SqlElement childToReplace, SqlElement replacement) => SqlElementConventions.TryReplaceChild(this, childToReplace, replacement);

        public void ReplaceChild(SqlElement childToReplace, SqlElement replacement)
        {
            if (!TryReplaceChild(childToReplace, replacement))
                throw new InvalidOperationException("SqlElement does not contain specified child");
            replacement.Parent = this;
        }

        public virtual void ReplaceSelf(SqlElement replacement) => Parent.ReplaceChild(this, replacement);

        public IEnumerable<SqlElement> Flatten()
        {
            yield return this;

            foreach (var c in Children)
                foreach (var x in c.Flatten())
                    yield return x;
        }

        public object Clone() => SqlElementConventions.CloneElement(this);
    }
}
