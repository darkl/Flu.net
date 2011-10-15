using System;
using System.Collections.Generic;
using Flunet.Extensions;

namespace Flunet.Automata.Language
{
    /// <summary>
    /// Compares two values by a mapping that indicates which pairs
    /// aren't equivalent.
    /// </summary>
    public class NonEquivalentComparer<T> : IEqualityComparer<T>
    {
        #region Members

        private readonly HashSet<Tuple<T, T>> mNonEquivalent;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="NonEquivalentComparer{T}"/> that compares
        /// values by the given <see cref="nonEquivalentPairs"/>.
        /// </summary>
        /// <param name="nonEquivalentPairs">A mapping that indicates what
        /// pairs are not equivalent.</param>
        public NonEquivalentComparer(HashSet<Tuple<T, T>> nonEquivalentPairs)
        {
            mNonEquivalent = nonEquivalentPairs;
        }

        #endregion

        #region IEqualityComparer<T> Members

        /// <summary>
        /// <see cref="IEqualityComparer{T}.Equals(T,T)"/>
        /// </summary>
        /// <remarks>Two values are considered to be equal
        /// if they are not not equivalent.</remarks>
        public bool Equals(T x, T y)
        {
            return !mNonEquivalent.Contains(x, y);
        }

        /// <summary>
        /// <see cref="IEqualityComparer{T}.GetHashCode(T)"/>
        /// </summary>
        /// <remarks>This just returns 0. 
        /// This is the best I can do...</remarks>
        public int GetHashCode(T obj)
        {
            return 0;
        }

        #endregion
    }
}
