using System;
using System.Reflection;

namespace TouchGestures.Lib.Extensions
{
    public static class PropertyInfoExtensions
    {
        public static bool HasAttribute<T>(this PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttribute<T>() != null;
        }

        public static T? GetAttribute<T>(this PropertyInfo property) where T : Attribute
        {
            return property.GetCustomAttribute<T>();
        }

        public static Attribute[] GetAttributes(this PropertyInfo property)
        {
            return Attribute.GetCustomAttributes(property);
        }

        public static PropertyInfo? FindPropertyWithAttribute<T>(this Type type) where T : Attribute
        {
            foreach (var property in type.GetProperties())
            {
                if (property.HasAttribute<T>())
                    return property;
            }

            return null;
        }
    }
}