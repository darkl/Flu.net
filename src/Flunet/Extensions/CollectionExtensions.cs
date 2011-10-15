using System.Collections.Generic;

namespace Flunet.Extensions
{
    /// <summary>
    /// Provides extension methods for collections.
    /// </summary>
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds a range of elements to a given collection.
        /// </summary>
        /// <typeparam name="T">The type of the collections.</typeparam>
        /// <param name="source">The collection to add the elements to.</param>
        /// <param name="range">The range of elements to add.</param>
        public static void AddRange<T>(this ICollection<T> source, IEnumerable<T> range)
        {
            foreach (T current in range)
            {
                source.Add(current);
            }
        }
    }
}