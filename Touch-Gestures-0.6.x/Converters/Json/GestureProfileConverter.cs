using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TouchGestures.Lib.Entities;

namespace TouchGestures.Converters
{
    public class GestureProfileConverter : JsonConverter<GestureProfile>
    {
        public override bool CanWrite => false;

        public override GestureProfile? ReadJson(JsonReader reader, Type objectType, GestureProfile? existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var value = JObject.Load(reader);

            if (value == null)
                return null!;

            BulletproofGestureProfile profile = new();
            serializer.Populate(value.CreateReader(), profile);
            return profile;
        }

        public override void WriteJson(JsonWriter writer, GestureProfile? value, JsonSerializer serializer)
            => throw new NotImplementedException();
    }
}