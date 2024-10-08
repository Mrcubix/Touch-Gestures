using System.Collections.Generic;
using Newtonsoft.Json;
using TouchGestures.Lib.Entities.Gestures.Bases;

namespace TouchGestures.Lib.Interfaces
{
    public interface IGesturesProfile : IEnumerable<Gesture> 
    {
        [JsonProperty]
        string Name { get; }

        [JsonProperty]
        bool IsMultiTouch { get; }

        void Add(Gesture gesture);

        void Remove(Gesture gesture);
    }
}