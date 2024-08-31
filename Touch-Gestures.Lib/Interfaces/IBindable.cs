using Newtonsoft.Json;
using TouchGestures.Lib.Reflection;

namespace TouchGestures.Lib.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IBindable
    {
        /// <inheritdoc />  
        [JsonProperty]
        BindingSettingStore? Store { get; set; }

        /// <summary>
        ///   The binding associated with the gesture.
        /// </summary>
        ISharedBinding? Binding { get; set; }
    }
}