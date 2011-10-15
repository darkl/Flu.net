using System;
using System.Collections.Generic;

namespace Flunet.Automata.Language
{
    /// <summary>
    /// Compares two <see cref="Tuple{T,T}"/> pairs without order relevance.
    /// </summary>
    public class SymmetricTupleComparer<T> : IEqualityComparer<Tuple<T, T>>
    {
        #region IEqualityComparer<Tuple<T,T>> Members

        /// <summary>
        /// <see cref="IEqualityComparer{T}.Equals(T,T)"/>
        /// </summary>
        public bool Equals(Tuple<T, T> x, Tuple<T, T> y)
        {
            return
                (EqualityComparer<T>.Default.Equals(x.Item1, y.Item1) &&
                EqualityComparer<T>.Default.Equals(x.Item2, y.Item2)) ||
                (EqualityComparer<T>.Default.Equals(x.Item1, y.Item2) &&
                EqualityComparer<T>.Default.Equals(x.Item2, y.Item1));
        }

        /// <summary>
        /// <see cref="IEqualityComparer{T}.GetHashCode(T)"/>
        /// </summary>
        public int GetHashCode(Tuple<T, T> obj)
        {
            return EqualityComparer<T>.Default.GetHashCode(obj.Item1) ^
                EqualityComparer<T>.Default.GetHashCode(obj.Item2);
        }

        #endregion
    }
}
