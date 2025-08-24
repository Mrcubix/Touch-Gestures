using System.Reflection;
using System.Collections;
using Newtonsoft.Json.Linq;
using OpenTabletDriver.External.Common.Serializables.Properties;
using OpenTabletDriver.Plugin.Attributes;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.External.Common.Enums;
using System.Collections.Generic;
using System;
using System.Linq;

namespace TouchGestures.Extensions.Reflection
{
    public static class SerializablePropertyExtension
    {
        internal static readonly Dictionary<Type, JTokenType> TypeToJTokenType = new()
        {
            { typeof(bool), JTokenType.Boolean },
            { typeof(int), JTokenType.Integer },
            { typeof(uint), JTokenType.Integer },
            { typeof(short), JTokenType.Integer },
            { typeof(ushort), JTokenType.Integer },
            { typeof(long), JTokenType.Integer },
            { typeof(ulong), JTokenType.Integer },
            { typeof(byte), JTokenType.Integer },
            { typeof(sbyte), JTokenType.Integer },
            { typeof(char), JTokenType.Integer },
            { typeof(float), JTokenType.Float },
            { typeof(double), JTokenType.Float },
            { typeof(string), JTokenType.String },
            { typeof(IEnumerable), JTokenType.Array }
        };

        public static SerializableProperty SerializableFrom(PropertyInfo property)
        {
            ArgumentNullException.ThrowIfNull(property);

            if (TypeToJTokenType.TryGetValue(property.PropertyType, out var type) == false)
                throw new ArgumentException($"Property type {property.PropertyType} is not supported.");

            // ALL that extra reflection bs just to get valid keys
            var validatedPropertyAttribute = property.GetCustomAttribute<PropertyValidatedAttribute>();
            var sliderPropertyAttribute = property.GetCustomAttribute<SliderPropertyAttribute>();

            var modifiers = property.SerializeModifiers();

            if (validatedPropertyAttribute == null)
                return new SerializableProperty(property.Name, type, modifiers);
            else if (sliderPropertyAttribute != null)
                return new SerializableSliderProperty(property.Name, type, modifiers)
                {
                    Minimum = sliderPropertyAttribute.Min,
                    Maximum = sliderPropertyAttribute.Max
                };
            else
            {
                // Get the name of the property holding the valid keys
                var validKeysPropertyName = validatedPropertyAttribute.MemberName;
                var owningType = property.DeclaringType!;
                
                var validKeysProperty = owningType.GetProperty(validKeysPropertyName);
                var validKeys = validatedPropertyAttribute?.GetValue<IEnumerable>(validKeysProperty).Cast<object>().ToArray() ?? Array.Empty<object>();

                return new SerializableValidatedProperty(property.Name, JTokenType.Array, validKeys, modifiers);
            }
        }

        internal static IEnumerable<SerializableAttributeModifier> SerializeModifiers(this PropertyInfo property)
        {
            var modifiers = new List<SerializableAttributeModifier>();
            
            if (property.GetCustomAttribute<UnitAttribute>() is { } unitAttribute)
                modifiers.Add(new SerializableAttributeModifier(AttributeModifierType.Unit, unitAttribute.Unit));

            if (property.GetCustomAttribute<ToolTipAttribute>() is { } toolTipAttribute)
                modifiers.Add(new SerializableAttributeModifier(AttributeModifierType.Tooltip, toolTipAttribute.ToolTip));

            if (property.GetCustomAttribute<DefaultPropertyValueAttribute>() is { } defaultValueAttribute)
                modifiers.Add(new SerializableAttributeModifier(AttributeModifierType.DefaultValue, defaultValueAttribute.Value));

            return modifiers;
        }
    }
}