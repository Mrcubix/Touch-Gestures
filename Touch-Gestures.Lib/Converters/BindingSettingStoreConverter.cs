using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using TouchGestures.Lib.Reflection;

namespace TouchGestures.Lib.Converters
{
    public abstract class BindingSettingStoreConverter(IReadOnlyCollection<TypeInfo> pluginsTypes) : JsonConverter<BindingSettingStore>
    {
        protected readonly IReadOnlyCollection<TypeInfo> pluginsTypes = pluginsTypes;
    }
}