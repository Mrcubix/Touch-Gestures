using System;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TouchGestures.Lib.Reflection
{
    public class BindingSetting
    {
        public BindingSetting(string property, object? value)
            : this()
        {
            Property = property;
            SetValue(value);
        }

        public BindingSetting(PropertyInfo property, object? value)
            : this(property.Name, value)
        {
        }

        public BindingSetting(PropertyInfo property)
            : this(property, null)
        {
        }

        [JsonConstructor]
        private BindingSetting()
        {
            Property = null!;
            Value = null!;
        }

        [JsonProperty]
        public string Property { set; get; }

        [JsonProperty]
        public JToken? Value { set; get; }

        [JsonIgnore]
        public bool HasValue => Value != null && Value.Type != JTokenType.Null;

        public void SetValue(object? value)
        {
            Value = value == null ? null : JToken.FromObject(value);
        }

        public T? GetValue<T>()
        {
            return Value == null ? default(T) : Value.Type != JTokenType.Null ? Value.ToObject<T>() : default(T);
        }

        public object? GetValue(Type asType)
        {
            return Value == null ? default : Value.Type != JTokenType.Null ? Value.ToObject(asType) : default;
        }

        public virtual T GetValueOrDefault<T>(PropertyInfo property)
            => throw new NotImplementedException();
    }
}
