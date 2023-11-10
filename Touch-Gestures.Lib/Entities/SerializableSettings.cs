using System.Collections.Generic;
using Newtonsoft.Json;
using TouchGestures.Lib.Serializables.Gestures;

namespace TouchGestures.Lib.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializableSettings
    {
        #region Constructors

        public SerializableSettings()
        {
        }

        #endregion

        #region Properties

        [JsonProperty]
        public List<SerializableTapGesture> TapGestures { get; set; } = new();

        [JsonProperty]
        public List<SerializableSwipeGesture> SwipeGestures { get; set; } = new();

        #endregion
    }
}