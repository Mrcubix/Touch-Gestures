using System.Collections.Generic;
using Newtonsoft.Json;
using TouchGestures.Lib.Entities.Gestures.Bases;

namespace TouchGestures.Lib.Interfaces
{
    public interface IGesturesProfile : IEnumerable<Gesture> 
    {
        /// <summary>
        ///   The name of the tablet associated with the profile.
        /// </summary>
        /// <remarks>
        ///   "(Pen Only)" may be append to the name of the profile if it is a pen (Single Touch) profile. <br/>
        ///   See <see cref="IsMultiTouch"/> for more information.
        /// </remarks>
        [JsonProperty]
        string Name { get; }

        /// <summary>
        ///   Whether or not the profile is a multi-touch profile.
        /// </summary>
        /// <remarks>
        ///   May also indicate that the profile is a pen (Single Touch) profile when false.
        /// </remarks>
        [JsonProperty]
        bool IsMultiTouch { get; }

        /// <summary>
        ///   Add a gesture to the profile.
        /// </summary>
        void Add(Gesture gesture);

        /// <summary>
        ///   Remove a gesture from the profile.
        /// </summary>
        void Remove(Gesture gesture);
    }
}