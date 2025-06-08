using System.Collections.Generic;
using Newtonsoft.Json;

namespace TouchGestures.Lib.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializableSettings
    {
        [JsonProperty]
        public int Version { get; set; } = 1;

        [JsonProperty]
        public List<SerializableProfile> Profiles { get; set; } = new();
    }
}