using System;

namespace Flunet.Attributes
{
    /// <summary>
    /// Indicates that a scope's methods are inherited to its children.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class InheritedAttribute : Attribute
    {
    }
}
