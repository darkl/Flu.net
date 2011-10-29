using System;
using System.Collections;
using System.Collections.Generic;
using Flunet.Automata.Interfaces;

namespace Flunet.Automata
{
    /// <summary>
    /// A default implementation of <see cref="IAutomataState{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of the alphabet of the <see cref="IAutomata{T}"/>
    /// </typeparam>
    /// <remarks>
    /// Important implementation detail: when this state is transited to an 
    /// input that is not mapped, it returns itself!
    /// </remarks>
    public class AutomataState<T> : IExtendableAutomataState<T>
    {
        #region Members

        private readonly IDictionary<T, IAutomataState<T>> mInputToState;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="AutomataState{T}"/> given
        /// its <see cref="Id"/> and <see cref="IsValid"/> values.
        /// </summary>
        /// <param name="id">The given <see cref="Id"/>.</param>
        /// <param name="isValid">A value indicating whether the
        /// state <see cref="IsValid"/>.</param>
        public AutomataState(string id, bool isValid) : 
            this(id, isValid, EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Creates a new <see cref="AutomataState{T}"/> given
        /// its <see cref="Id"/> and <see cref="IsValid"/> values,
        /// and a <see cref="IEqualityComparer{T}"/> to compare the inputs on transitions.
        /// </summary>
        /// <param name="id">The given <see cref="Id"/>.</param>
        /// <param name="isValid">A value indicating whether the
        /// state <see cref="IsValid"/>.</param>
        /// <param name="comparer">A <see cref="IEqualityComparer{T}"/> to
        /// compare inputs on transitions.</param>
        public AutomataState(string id, bool isValid, IEqualityComparer<T> comparer)
        {
            Id = id;
            IsValid = isValid;
            mInputToState = new Dictionary<T, IAutomataState<T>>(comparer);
        }

        #endregion

        #region IAutomataState<T> Members

        /// <summary>
        /// <see cref="IAutomataState{T}.IsValid"/>
        /// </summary>
        public bool IsValid
        {
            get; 
            private set;
        }

        /// <summary>
        /// <see cref="IAutomataState{T}.Id"/>
        /// </summary>
        public string Id
        {
            get; 
            private set;
        }

        /// <summary>
        /// <see cref="IAutomataState{T}.Transit"/>
        /// </summary>
        /// <remarks>
        /// If the corresponding input isn't mapped,
        /// the transited state is the the current state!
        /// </remarks>
        public IAutomataState<T> Transit(T input)
        {
            IAutomataState<T> result;

            if (mInputToState.TryGetValue(input, out result))
            {
                return result;
            }

            return this;
        }
        
        #endregion

        #region IExtendableAutomataState<T> Members

        /// <summary>
        /// <see cref="IExtendableAutomataState{T}.Add"/>
        /// </summary>
        public void Add(T input, IAutomataState<T> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state",
                                                "Can't map a state to null.");
            }

            mInputToState.Add(input, state);
        }

        #endregion

        #region IEnumerable<KeyValuePair<T, IAutomataState<T>>> Members

        /// <summary>
        /// <see cref="IEnumerable{T}.GetEnumerator"/>
        /// </summary>
        public IEnumerator<KeyValuePair<T, IAutomataState<T>>> GetEnumerator()
        {
            return mInputToState.GetEnumerator();
        }
        
        #endregion

        #region IEnumerable Members

        /// <summary>
        /// <see cref="System.Collections.IEnumerable.GetEnumerator"/>
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return mInputToState.GetEnumerator();
        }

        #endregion
    }
}