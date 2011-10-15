using System;
using System.Linq;
using System.Reflection;
using Flunet.Attributes;

namespace Flunet.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="Attribute"/>s handling.
    /// </summary>
    public static class AttributeExtensions
    {
        /// <summary>
        /// Gets a value indicating whether a given <see cref="MemberInfo"/> has
        /// the requested <see cref="Attribute"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The requested <see cref="Attribute"/> type.
        /// </typeparam>
        /// <param name="info">The given <see cref="MemberInfo"/>.</param>
        /// <returns>A value indicating whether a given <see cref="MemberInfo"/> has
        /// the requested <see cref="Attribute"/>.</returns>
        public static bool HasAttribute<TAttribute>(this MemberInfo info) where TAttribute : Attribute
        {
            return info.GetCustomAttributes(typeof(TAttribute), true).Any();
        }

        /// <summary>
        /// Gets the <see cref="Attribute"/> of a given type from 
        /// on a given <see cref="MemberInfo"/>.
        /// </summary>
        /// <typeparam name="TAttribute">The given <see cref="Attribute"/> type.</typeparam>
        /// <param name="info">The given <see cref="MemberInfo"/>.</param>
        /// <returns>The <see cref="Attribute"/> of a given type from 
        /// on a given <see cref="MemberInfo"/>.</returns>
        public static TAttribute GetAttribute<TAttribute>(this MemberInfo info) where TAttribute : Attribute
        {
            return (TAttribute)info.GetCustomAttributes(typeof(TAttribute), true).FirstOrDefault();
        }

        /// <summary>
        /// Gets the scope declared by a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The given <see cref="Type"/>.</param>
        /// <returns>The scope declared by a given <see cref="Type"/>.</returns>
        public static string GetScopeName(this Type type)
        {
            ScopeAttribute scope = type.GetAttribute<ScopeAttribute>();
            
            if (scope != null)
            {
                return scope.Name;
            }

            return type.Name;
        }

    }
}
