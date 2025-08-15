using System;

namespace TouchGestures.UX.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class MultiTouchOnlyAttribute(bool isMultiTouchOnly) : Attribute
{
    public bool IsMultiTouchOnly { get; } = isMultiTouchOnly;
}
