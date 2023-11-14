using Newtonsoft.Json;
using OpenTabletDriver.Desktop.Reflection;
using OpenTabletDriver.Plugin;

namespace TouchGestures.Lib.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IBindable
    {
        /// <inheritdoc />  
        [JsonProperty]
        PluginSettingStore? Store { get; set; }

        /// <summary>
        ///   The binding associated with the gesture.
        /// </summary>
        IBinding? Binding { get; }
    }
}