using System;

namespace TouchGestures.UX.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class DescriptionAttribute(string name) : Attribute
{
    public string Description { get; } = name;
}
