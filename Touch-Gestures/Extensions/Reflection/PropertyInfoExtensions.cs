using System.Reflection;
using OpenTabletDriver.External.Common.Serializables.Properties;

namespace TouchGestures.Extensions.Reflection
{
    public static class PropertyInfoExtensions
    {
        public static SerializableProperty ToSerializable(this PropertyInfo property) => SerializablePropertyExtension.SerializableFrom(property);
    }
}