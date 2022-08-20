using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TreeSqlParser.Model
{
    public static class SqlElementConventions
    {
        private class SqlElementPropertyInfo
        {
            private static readonly HashSet<Type> ValueTypes = new HashSet<Type>
            {
                typeof(string), 
                typeof(long), typeof(long?), typeof(int), typeof(int?), typeof(short), typeof(short?),
                typeof(ulong), typeof(ulong?), typeof(uint), typeof(uint?), typeof(ushort), typeof(ushort?),
                typeof(byte), typeof(char),
                typeof(decimal), typeof(decimal?), typeof(double), typeof(double?), typeof(float), typeof(float?),
                typeof(DateTime), typeof(DateTime?), typeof(TimeSpan), typeof(TimeSpan?),
                typeof(bool), typeof(bool?)
            };

            public PropertyInfo[] SingleElementProperties { get; }

            public PropertyInfo[] ListProperties { get; }

            public PropertyInfo[] ValueProperties { get; }

            public PropertyInfo[] ValueListProperties { get; }

            public PropertyInfo[] CloneableProperties { get; }

            public PropertyInfo[] CloneableListProperties { get; }

            public SqlElementPropertyInfo(Type t)
            {
                static bool isSqlElement(Type x) => 
                    typeof(SqlElement).IsAssignableFrom(x);

                static bool isListSqlElement(Type x) => 
                    x.IsGenericType && x.GetGenericTypeDefinition() == typeof(List<>) && isSqlElement(x.GenericTypeArguments[0]);

                static bool isValueType(Type x) =>
                    ValueTypes.Contains(x) || x.IsEnum;

                static bool isValueListType(Type x) =>
                    x.IsGenericType && x.GetGenericTypeDefinition() == typeof(List<>) && isValueType(x.GenericTypeArguments[0]);

                static bool isCloneable(Type x) =>
                    typeof(ICloneable).IsAssignableFrom(x);

                static bool isCloneableList(Type x) =>
                    x.IsGenericType && x.GetGenericTypeDefinition() == typeof(List<>) && isCloneable(x.GenericTypeArguments[0]);

                var readWriteProperties = t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanRead && x.CanWrite && x.GetGetMethod() != null && x.GetSetMethod() != null)
                    .Where(x => x.Name != "Parent")
                    .ToArray();

                SingleElementProperties = readWriteProperties.Where(x => isSqlElement(x.PropertyType)).ToArray();
                ListProperties = readWriteProperties.Where(x => isListSqlElement(x.PropertyType)).ToArray();
                ValueProperties = readWriteProperties.Where(x => isValueType(x.PropertyType)).ToArray();
                ValueListProperties = readWriteProperties.Where(x => isValueListType(x.PropertyType)).ToArray();
                CloneableProperties = readWriteProperties.Except(SingleElementProperties).Where(x => isCloneable(x.PropertyType)).ToArray();
                CloneableListProperties = readWriteProperties.Except(ListProperties).Where(x => isCloneableList(x.PropertyType)).ToArray();

                var remaining = readWriteProperties
                    .Except(SingleElementProperties)
                    .Except(ListProperties)
                    .Except(ValueProperties)
                    .Except(ValueListProperties)
                    .Except(CloneableProperties)
                    .Except(CloneableListProperties)
                    .ToArray();

                if (remaining.Any())
                    throw new NotSupportedException("Can't clone");
            }
        }

        internal static object CloneElement(SqlElement e)
        {
            var elementTypeInfo = GetPropertyInfo(e);
            var result = (SqlElement) Activator.CreateInstance(e.GetType());
            
            // clone each child element, and reparent
            foreach (var p in elementTypeInfo.SingleElementProperties)
            {
                if (p.GetValue(e) is SqlElement propertyVal)
                {
                    var clonedChild = CloneChildElement(propertyVal, result);
                    p.SetValue(result, clonedChild);
                }
            }

            // clone each List of child elements, and reparent
            foreach (var p in elementTypeInfo.ListProperties)
            {
                if (p.GetValue(e) is IList propertyVal)
                {
                    var clonedList = CloneChildElementList(propertyVal, result);
                    p.SetValue(result, clonedList);
                }
            }

            // value types can simply be copied across
            foreach (var p in elementTypeInfo.ValueProperties)
            {
                object propertyVal = p.GetValue(e);
                p.SetValue(result, propertyVal);
            }

            // value lists can be copied to a new list
            foreach (var p in elementTypeInfo.ValueListProperties)
            {
                var propertyVal = p.GetValue(e);
                if (propertyVal != null)
                {
                    // call the list copy constructor
                    var cloned = Activator.CreateInstance(propertyVal.GetType(), propertyVal);
                    p.SetValue(result, cloned);
                }
            }

            // ICloneables can be cloned
            foreach (var p in elementTypeInfo.CloneableProperties)
            {
                if (p.GetValue(e) is ICloneable propertyVal)
                {
                    var cloned = propertyVal.Clone();
                    p.SetValue(result, cloned);
                }
            }

            // List<ICloneables> can be deep copied
            foreach (var p in elementTypeInfo.CloneableListProperties)
            {
                if (p.GetValue(e) is IList propertyVal)
                {
                    var cloned = CloneCloneableList(propertyVal);
                    p.SetValue(result, cloned);
                }
            }

            return result;
        }

        private static SqlElement CloneChildElement(SqlElement toClone, SqlElement newParent)
        {
            if (toClone == null)
                return null;

            var cloned = (SqlElement) toClone.Clone();
            cloned.Parent = newParent;
            return cloned;
        }

        private static IList CloneChildElementList(IList toClone, SqlElement newParent)
        {
            if (toClone == null)
                return null;

            var result = (IList) Activator.CreateInstance(toClone.GetType());
            foreach (var x in toClone)
            {
                var cloned = CloneChildElement((SqlElement)x, newParent);
                result.Add(cloned);
            }

            return result;
        }

        private static IList CloneCloneableList(IList toClone)
        {
            if (toClone == null)
                return null;

            var result = (IList)Activator.CreateInstance(toClone.GetType());
            foreach (var x in toClone)
            {
                var c = (x as ICloneable)?.Clone();
                result.Add(c);
            }

            return result;
        }

        private static readonly ConcurrentDictionary<Type, SqlElementPropertyInfo> typeInfos = new ConcurrentDictionary<Type, SqlElementPropertyInfo>();

        public static IEnumerable<SqlElement> GetChildren(SqlElement e)
        {
            var elementTypeInfo = GetPropertyInfo(e);

            foreach (var p in elementTypeInfo.SingleElementProperties)
            {
                if (p.GetValue(e) is SqlElement propertyVal)
                    yield return propertyVal;
            }

            foreach (var p in elementTypeInfo.ListProperties)
            {
                if (p.GetValue(e) is IList list)
                    foreach (SqlElement x in list)
                        if (x != null)
                            yield return x;
            }
        }

        public static bool TryReplaceChild(SqlElement containingElement, SqlElement childToReplace, SqlElement replacement)
        {
            if (containingElement is null) throw new ArgumentNullException(nameof(containingElement));
            if (childToReplace is null) throw new ArgumentNullException(nameof(childToReplace));

            var elementTypeInfo = GetPropertyInfo(containingElement);

            foreach (var p in elementTypeInfo.SingleElementProperties)
            {
                var propertyVal = (SqlElement)p.GetValue(containingElement);
                if (ReferenceEquals(propertyVal, childToReplace))
                {
                    p.SetValue(containingElement, replacement);
                    return true;
                }
            }

            foreach (var p in elementTypeInfo.ListProperties)
            {
                if (p.GetValue(containingElement) is IList list)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (ReferenceEquals(list[i], childToReplace))
                        {
                            list[i] = replacement;
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static SqlElementPropertyInfo GetPropertyInfo(SqlElement e)
        {
            Type t = e.GetType();
            var result = typeInfos.GetOrAdd(t, x => new SqlElementPropertyInfo(x));
            return result;
        }
    }
}
