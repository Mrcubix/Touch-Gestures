using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenTabletDriver.External.Common.Serializables;
using OpenTabletDriver.External.Common.Serializables.Properties;

namespace TouchGestures.Lib.Converters.Json
{
    public class SerializablePropertyConverter : JsonConverter<SerializableProperty>
    {
        public override SerializableProperty? ReadJson(JsonReader reader, Type objectType, SerializableProperty? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = JObject.Load(reader);

            if (value == null)
                return null;

            SerializableProperty? property;

            if (value.ContainsKey("Values"))
                property = new SerializableValidatedProperty(string.Empty, JTokenType.Null, Array.Empty<object>(), Array.Empty<SerializableAttributeModifier>());
            else if (value.ContainsKey("Maximum"))
                property = new SerializableSliderProperty(string.Empty, JTokenType.Null, Array.Empty<SerializableAttributeModifier>());
            else
                property = new SerializableProperty(string.Empty, JTokenType.Null, Array.Empty<SerializableAttributeModifier>());

            if (property == null)
                return null;

            serializer.Populate(value.CreateReader(), property);
            return property;
        }

        public override void WriteJson(JsonWriter writer, SerializableProperty? value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanWrite => false;
    }
}