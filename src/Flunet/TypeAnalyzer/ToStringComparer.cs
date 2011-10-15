using System.Collections.Generic;
using System.Reflection;

namespace Flunet.TypeAnalyzer
{
    /// <summary>
    /// Compares two instances by their <see cref="object.ToString"/> value.
    /// </summary>
    /// <remarks>
    /// This class is a patch because it is hard to compare two <see cref="MethodInfo"/>s.
    /// </remarks>
    internal class ToStringComparer<T> : IEqualityComparer<T> where T : class
    {
        /// <summary>
        /// <see cref="IEqualityComparer{T}.Equals(T,T)"/>
        /// </summary>
        public bool Equals(T x, T y)
        {
            if ((x == null) || (y == null))
            {
                return (x == null) && (y == null);
            }

            return x.ToString() == y.ToString();
        }

        /// <summary>
        /// <see cref="IEqualityComparer{T}.GetHashCode(T)"/>
        /// </summary>
        public int GetHashCode(T obj)
        {
            return obj.ToString().GetHashCode();
        }
    }
}