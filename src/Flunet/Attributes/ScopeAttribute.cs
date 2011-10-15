using System;

namespace Flunet.Attributes
{
    /// <summary>
    /// Defines a named scope for a given interface.
    /// </summary>
    /// <returns>
    /// Scopes names are used for readability.
    /// Without it, interfaces have their name as the default scope name.
    /// </returns>
    [AttributeUsage(AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
    public sealed class ScopeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of a <see cref="ScopeAttribute"/>
        /// with the given name.
        /// </summary>
        /// <param name="name">The given scope's name.</param>
        public ScopeAttribute(string name)
        {
            this.Name = name;
        }

        /// <summary>
        /// The scope's name.
        /// </summary>
        public string Name
        {
            get; 
            private set;
        }
    }
}
