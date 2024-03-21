using System;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TouchGestures.Lib.Entities;

namespace TouchGestures.Lib.Converters
{
    public class SharedAreaConverter : JsonConverter<SharedArea>
    {
        public override SharedArea? ReadJson(JsonReader reader, Type objectType, SharedArea? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            // build a PluginSettingsStore using value.Path as argument
            var value = JObject.Load(reader);

            if (value == null)
                return null!;

            value.TryGetValue("Width", out var width);
            value.TryGetValue("Height", out var height);
            value.TryGetValue("Position", out var position);
            value.TryGetValue("Rotation", out var rotation);

            if (width == null || height == null || position == null || rotation == null)
                return null!;

            return new SharedArea((float)width, (float)height, position.ToObject<Vector2>(), (float)rotation);
        }

        public override void WriteJson(JsonWriter writer, SharedArea? value, JsonSerializer serializer)
        {
            // write a JObject with Path, Settings and Enable properties

            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var obj = new JObject
            {
                ["Width"] = value.Width,
                ["Height"] = value.Height,
                ["Position"] = JToken.FromObject(value.Position),
                ["Rotation"] = value.Rotation
            };

            obj.WriteTo(writer);
        }
    }
}