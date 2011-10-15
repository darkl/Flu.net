using System;

namespace Flunet.Attributes
{
    /// <summary>
    /// Represents an alias for a group of methods.
    /// This provides a way to make sure that all methods with the same
    /// alias are unique in a scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class AliasAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="AliasAttribute"/>.
        /// </summary>
        /// <param name="alias">The given alias.</param>
        public AliasAttribute(string alias)
        {
            this.Alias = alias;
        }

        /// <summary>
        /// The alias of the method.
        /// </summary>
        public string Alias
        {
            get; 
            private set;
        }
    }
}
