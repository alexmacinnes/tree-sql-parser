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
        #region Type Info
        private class SqlElementTypeInfo
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

            public SqlElementTypeInfo(Type t)
            {
                Name = t.Name;

                var readWriteProperties =
                    t.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(x => x.CanRead && x.CanWrite && x.GetGetMethod() != null && x.GetSetMethod() != null)
                    .Where(x => x.Name != "Parent")
                    .ToArray();

                PropertyAdaptors = readWriteProperties.Select(PropertyAdaptor).ToArray();
            }

            private PropertyAdaptorBase PropertyAdaptor(PropertyInfo p)
            {
                Type t = p.PropertyType;

                if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>))
                {
                    Type typeArg = t.GenericTypeArguments[0];
                    if (IsSqlElement(typeArg))
                        return new SqlElementListPropertyAdaptor(p);
                    if (IsValueType(typeArg))
                        return new ValueListPropertyAdaptor(p);
                    if (IsCloneable(typeArg))
                        return new CloneableListPropertyAdaptor(p);
                }
                else
                {
                    if (IsSqlElement(t))
                        return new SqlElementPropertyAdaptor(p);
                    if (IsValueType(t))
                        return new ValuePropertyAdaptor(p);
                    if (IsCloneable(t))
                        return new CloneablePropertyAdaptor(p);
                }

                 throw new InvalidOperationException("Unknown property type");
            }

            private static bool IsCloneable(Type x) => 
                typeof(ICloneable).IsAssignableFrom(x);

            private static bool IsValueType(Type x) =>
                ValueTypes.Contains(x) || isEnum(x);

            private static bool isEnum(Type x) =>
                x.IsEnum ||
                (x.IsGenericType && x.GetGenericTypeDefinition() == typeof(Nullable<>) && x.GetGenericArguments()[0].IsEnum);

            private static bool IsSqlElement(Type x) =>
                typeof(SqlElement).IsAssignableFrom(x);

            public string Name { get; set; }

            public IList<PropertyAdaptorBase> PropertyAdaptors { get; }

            public override string ToString() => Name;
        }

        #endregion

        #region Property Adaptors

        private abstract class PropertyAdaptorBase
        {
            protected PropertyInfo PropertyInfo { get; }
            protected PropertyAdaptorBase(PropertyInfo propertyInfo)
            {
                PropertyInfo = propertyInfo;
            }
            public override string ToString() => GetType().Name + " :  " + PropertyInfo.Name;

            public abstract void CloneProperty(SqlElement source, SqlElement target);

            public virtual IEnumerable<SqlElement> GetChildren(SqlElement e) => Enumerable.Empty<SqlElement>();

            public virtual bool TryReplaceChild(SqlElement containingElement, SqlElement childToReplace, SqlElement replacement) => false;
        }

        private class SqlElementPropertyAdaptor : PropertyAdaptorBase
        {
            public SqlElementPropertyAdaptor(PropertyInfo propertyInfo) : base(propertyInfo) { }

            public override void CloneProperty(SqlElement source, SqlElement target)
            {
                if (source == null)
                    return;

                var e = PropertyInfo.GetValue(source) as SqlElement;
                if (e == null)
                    return;

                var cloned = (SqlElement)CloneElement(e);
                cloned.Parent = target;

                PropertyInfo.SetValue(target, cloned);
            }

            public override IEnumerable<SqlElement> GetChildren(SqlElement e)
            {
                var child = (SqlElement)PropertyInfo.GetValue(e);
                if (child != null)
                    yield return child;
            }

            public override bool TryReplaceChild(SqlElement containingElement, SqlElement childToReplace, SqlElement replacement)
            {
                var e = (SqlElement)PropertyInfo.GetValue(containingElement);
                if (object.ReferenceEquals(e, childToReplace))
                {
                    replacement.Parent = containingElement;
                    PropertyInfo.SetValue(containingElement, replacement);

                    return true;
                }

                return false;
            }
        }

        private class SqlElementListPropertyAdaptor : PropertyAdaptorBase
        {
            public SqlElementListPropertyAdaptor(PropertyInfo propertyInfo) : base(propertyInfo) { }

            public override void CloneProperty(SqlElement source, SqlElement target)
            {
                if (source == null)
                    return;

                var srcList = PropertyInfo.GetValue(source) as IList;
                if (srcList != null)
                {
                    var clonedList = (IList)Activator.CreateInstance(srcList.GetType());
                    foreach (var x in srcList)
                    {
                        var sourceElement = x as SqlElement;
                        var cloned = Clone(sourceElement, target);
                        clonedList.Add(cloned);
                    }

                    PropertyInfo.SetValue(target, clonedList);
                }
            }

            private SqlElement Clone(SqlElement source, SqlElement newParent)
            {
                var cloned = CloneElement(source);
                if (cloned != null)
                    cloned.Parent = newParent;
                return cloned;
            }

            public override IEnumerable<SqlElement> GetChildren(SqlElement e)
            {
                var list = PropertyInfo.GetValue(e) as IList;
                if (list != null)
                {
                    foreach (var x in list)
                    {
                        if (x != null)
                            yield return (SqlElement)x;
                    }
                }
            }

            public override bool TryReplaceChild(SqlElement containingElement, SqlElement childToReplace, SqlElement replacement)
            {
                var list = PropertyInfo.GetValue(containingElement) as IList;
                if (list == null)
                    return false;

                for (int i = 0; i < list.Count; i++)
                {
                    var x = list[i];
                    if (x == null)
                        continue;

                    if (object.ReferenceEquals(x, childToReplace))
                    {
                        replacement.Parent = containingElement;
                        list[i] = replacement;

                        return true;
                    }
                }

                return false;
            }
        }

        private class ValuePropertyAdaptor : PropertyAdaptorBase
        {
            public ValuePropertyAdaptor(PropertyInfo propertyInfo) : base(propertyInfo) { }

            public override void CloneProperty(SqlElement source, SqlElement target)
            {
                var x = PropertyInfo.GetValue(source);
                PropertyInfo.SetValue(target, x);
            }
        }

        private class ValueListPropertyAdaptor : PropertyAdaptorBase
        {
            public ValueListPropertyAdaptor(PropertyInfo propertyInfo) : base(propertyInfo) { }

            public override void CloneProperty(SqlElement source, SqlElement target)
            {
                if (source == null)
                    return;

                var sourceList = PropertyInfo.GetValue(source) as IList;
                if (sourceList != null)
                {
                    var clonedList = (IList)Activator.CreateInstance(sourceList.GetType());
                    foreach (var x in sourceList)
                    {
                        var cloned = x == null ? null : ((ICloneable)x).Clone();
                        clonedList.Add(cloned);
                    }

                    PropertyInfo.SetValue(target, clonedList);
                }
            }
        }

        private class CloneablePropertyAdaptor : PropertyAdaptorBase
        {
            public CloneablePropertyAdaptor(PropertyInfo propertyInfo) : base(propertyInfo) { }

            public override void CloneProperty(SqlElement source, SqlElement target)
            {
                if (source == null)
                    return;

                var x = PropertyInfo.GetValue(source) as ICloneable;

                if (x != null)
                {
                    var cloned = x.Clone();
                    PropertyInfo.SetValue(target, cloned);
                }
            }
        }

        private class CloneableListPropertyAdaptor : PropertyAdaptorBase
        {
            public CloneableListPropertyAdaptor(PropertyInfo propertyInfo) : base(propertyInfo) { }

            public override void CloneProperty(SqlElement source, SqlElement target)
            {
                if (source == null)
                    return;

                var sourceList = PropertyInfo.GetValue(source) as IList;
                if (sourceList != null)
                {
                    var clonedList = (IList)Activator.CreateInstance(sourceList.GetType());
                    foreach (var x in sourceList)
                        clonedList.Add(x);

                    PropertyInfo.SetValue(target, clonedList);
                }
            }
        }

        #endregion

        private static readonly ConcurrentDictionary<Type, SqlElementTypeInfo> typeInfos = new ConcurrentDictionary<Type, SqlElementTypeInfo>();

        private static SqlElementTypeInfo GetTypeInfo(Type t)
        {
            var result = typeInfos.GetOrAdd(t, x => new SqlElementTypeInfo(x));
            return result;
        }

        public static SqlElement CloneElement(SqlElement e)
        {
            if (e == null)
                return null;

            Type t = e.GetType();
            var elementTypeInfo = GetTypeInfo(t);
            var result = (SqlElement)Activator.CreateInstance(t);

            foreach (var p in elementTypeInfo.PropertyAdaptors)
                p.CloneProperty(e, result);

            return result;
        }

        public static IEnumerable<SqlElement> GetChildren(SqlElement e)
        {
            var elementTypeInfo = GetTypeInfo(e.GetType());

            foreach (var p in elementTypeInfo.PropertyAdaptors)
            {
                foreach (var c in p.GetChildren(e))
                    yield return c;
            }
        }

        public static bool TryReplaceChild(SqlElement containingElement, SqlElement childToReplace, SqlElement replacement)
        {
            if (containingElement is null) throw new ArgumentNullException(nameof(containingElement));
            if (childToReplace is null) throw new ArgumentNullException(nameof(childToReplace));

            var elementTypeInfo = GetTypeInfo(containingElement.GetType());

            foreach (var p in elementTypeInfo.PropertyAdaptors)
            {
                if (p.TryReplaceChild(containingElement, childToReplace, replacement))
                    return true;
            }

            return false;
        }
    }
}