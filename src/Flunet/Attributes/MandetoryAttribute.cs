using System;

namespace Flunet.Attributes
{
    /// <summary>
    /// Indicates that a method call is mandatory in scope - i.e.
    /// can't call other methods before calling it.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class MandatoryAttribute : Attribute
    {
    }
}
