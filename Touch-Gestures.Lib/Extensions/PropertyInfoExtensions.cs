using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using OpenTabletDriver.Plugin;

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

        public static bool TryGetStaticValue<T>(this PropertyInfo property, out T? value)
        {
            value = default;

            var sourceType = property.ReflectedType;

            if (sourceType == null)
                return false;

            var member = sourceType.GetMember(property.Name).FirstOrDefault();

            if (member == null)
                return false;

            try
            {
                value = member.MemberType switch
                {
                    MemberTypes.Property => (T?)sourceType.GetProperty(property.Name)?.GetValue(null),
                    MemberTypes.Field => (T?)sourceType.GetField(property.Name)?.GetValue(null),
                    MemberTypes.Method => (T?)sourceType.GetMethod(property.Name)?.Invoke(null, null),
                    _ => default
                };
            }
            catch (Exception e)
            {
                Log.Write("Plugin", $"Failed to get valid binding values for '{property.Name}'", LogLevel.Error);

                var match = Regex.Match(e.Message, "Non-static (.*) requires a target\\.");

                if (e is TargetException && match.Success)
                {
                    Log.Debug("Plugin", $"Validation {match.Groups[1].Value} must be static");
                }
                else
                {
                    Log.Exception(e);
                }
            }

            return value != null;
        }
    }
}