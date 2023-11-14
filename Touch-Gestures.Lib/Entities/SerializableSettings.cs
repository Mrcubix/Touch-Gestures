using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using TouchGestures.Lib.Entities.Gestures.Bases;
using TouchGestures.Lib.Serializables.Gestures;

namespace TouchGestures.Lib.Entities
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SerializableSettings : IEnumerable<Gesture>
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

        public IEnumerator<Gesture> GetEnumerator()
        {
            foreach (var tapGesture in TapGestures)
            {
                yield return tapGesture;
            }

            foreach (var swipeGesture in SwipeGestures)
            {
                yield return swipeGesture;
            }

            // do the same for other gestures
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    }
}