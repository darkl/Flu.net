using System;

namespace Flunet.Attributes
{
    /// <summary>
    /// Indicates that a method can be called once in a scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class UniqueInScopeAttribute : Attribute
    {
        /// <summary>
        /// Creates a new instance of the <see cref="UniqueInScopeAttribute"/>.
        /// </summary>
        /// <param name="scope">The scope where the method is unique.</param>
        public UniqueInScopeAttribute(string scope)
        {
            this.Scope = scope;
        }

        /// <summary>
        /// The scope where the method can be called once.
        /// </summary>
        public string Scope 
        { 
            get; 
            private set; 
        }
    }
}
