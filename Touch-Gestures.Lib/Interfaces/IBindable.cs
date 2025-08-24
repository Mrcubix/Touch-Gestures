using Newtonsoft.Json;
using OpenTabletDriver.Desktop.Reflection;

namespace TouchGestures.Lib.Interfaces
{
    [JsonObject(MemberSerialization.OptIn)]
    public interface IBindable
    {
        /// <summary>
        ///   The plugin settings store used for building the bindings.
        /// </summary>
        [JsonProperty]
        PluginSettingStore? Store { get; set; }
    }
}