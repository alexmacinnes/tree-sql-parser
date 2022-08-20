using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TreeSqlParser.Parsing.Enums
{
    internal class EnumUtilities
    {
        public static IReadOnlyDictionary<string, T> ToDictionary<T>(bool caseSensitive = false)
            where T : Enum
        {
            var vals = Enum.GetValues(typeof(T)).Cast<T>();
            var comparer = caseSensitive ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase;
            var result = vals.ToDictionary(x => x.ToString(), x => x, comparer);
            return result;           
        }

        public static IReadOnlyDictionary<string, TEnum> ToDictionary<TEnum, TAttribute>(bool caseSensitive = false)
            where TEnum : Enum
            where TAttribute : Attribute
        {
            var fields = typeof(TEnum).GetFields().Where(x => x.Name != "value__").ToArray();
            var comparer = caseSensitive ? StringComparer.InvariantCulture : StringComparer.InvariantCultureIgnoreCase;
            var result = fields.ToDictionary(x => EnumText<TAttribute>(x), x => (TEnum) Enum.Parse(typeof(TEnum), x.Name));
            return result;
        }

        private static string EnumText<TAttribute>(FieldInfo f)
            where TAttribute : Attribute
            => f.GetCustomAttribute<TAttribute>().ToString();
    }
}
