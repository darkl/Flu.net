using System;
using System.Collections.Generic;
using System.Linq;
using Flunet.Automata.Interfaces;

namespace Flunet.Automata
{
    /// <summary>
    /// A simple implementation of <see cref="IExtendableDeterministicAutomata{T}"/>
    /// using the <see cref="AutomataState{T}"/> type as states.
    /// </summary>
    /// <typeparam name="T">The given type of the alphabet of this <see cref="IAutomata{T}"/></typeparam>
    public class DeterministicAutomata<T> : IExtendableDeterministicAutomata<T>
    {
        #region Members

        private readonly ICollection<IAutomataState<T>> mStates = 
            new HashSet<IAutomataState<T>>();

        private readonly IEqualityComparer<T> mComparer;
        
        private ICollection<T> mAlphabet;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="DeterministicAutomata{T}"/> with
        /// the <see cref="EqualityComparer{T}.Default"/> comparer.
        /// </summary>
        public DeterministicAutomata()
            : this(EqualityComparer<T>.Default)
        {
        }

        /// <summary>
        /// Creates a new <see cref="DeterministicAutomata{T}"/> with
        /// a given <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">The given comparer.</param>
        public DeterministicAutomata(IEqualityComparer<T> comparer)
        {
            mComparer = comparer;
            Alphabet = new HashSet<T>(comparer);
        }

        #endregion

        #region IAutomata<T> Members

        /// <summary>
        /// <see cref="IAutomata{T}.Read"/>
        /// </summary>
        public void Read(T input)
        {
            if (this.CurrentState == null)
            {
                ThrowHelper.ThrowNoStatesAvailable();
            }

            CurrentState = CurrentState.Transit(input);
        }

        /// <summary>
        /// <see cref="IAutomata{T}.Reset"/>
        /// </summary>
        public void Reset()
        {
            if (this.Root == null)
            {
                ThrowHelper.ThrowNoStatesAvailable();
            }

            this.CurrentState = this.Root;
        }

        /// <summary>
        /// <see cref="IAutomata{T}.IsValid"/>
        /// </summary>
        public bool IsValid
        {
            get 
            {
                if (this.CurrentState == null)
                {
                    ThrowHelper.ThrowNoStatesAvailable();
                }

                return CurrentState.IsValid; 
            }
        }

        /// <summary>
        /// <see cref="IAutomata{T}.Comparer"/>
        /// </summary>
        public IEqualityComparer<T> Comparer
        {
            get
            {
                return mComparer;
            }
        }

        /// <summary>
        /// <see cref="IAutomata{T}.Alphabet"/>
        /// </summary>
        public ICollection<T> Alphabet
        {
            get
            {
                if ((mAlphabet == null) || !mAlphabet.Any())
                {
                    // If there is no alphabet available, we guess it...
                    IEnumerable<T> knownSymbols =
                        mStates
                            .SelectMany(x => x)
                            .Select(x => x.Key);

                    mAlphabet =
                        new HashSet<T>(knownSymbols, mComparer);
                }

                return mAlphabet;
            }
            private set
            {
                mAlphabet = value;
            }
        }

        #endregion

        #region IDeterministicAutomata<T> Members

        /// <summary>
        /// <see cref="IDeterministicAutomata{T}.Root"/>
        /// </summary>
        public IAutomataState<T> Root
        {
            get;
            private set;
        }

        /// <summary>
        /// <see cref="IDeterministicAutomata{T}.CurrentState"/>
        /// </summary>
        public IAutomataState<T> CurrentState
        {
            get;
            private set;
        }

        #endregion

        #region IExtendableStateAutomata<T> Members

        /// <summary>
        /// <see cref="IExtendableAutomataState{T}.Add"/>
        /// </summary>
        public void Add(IAutomataState<T> state)
        {
            if (state == null)
            {
                throw new ArgumentNullException("state",
                                                "Can't add a null state.");
            }

            if (Root == null)
            {
                Root = state;
            }

            if (CurrentState == null)
            {
                CurrentState = state;
            }

            mStates.Add(state);
        }

        #endregion

        #region IEnumerable<IAutomataState<T>> Members

        /// <summary>
        /// <see cref="IEnumerable{T}.GetEnumerator"/>
        /// </summary>
        public IEnumerator<IAutomataState<T>> GetEnumerator()
        {
            return mStates.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// <see cref="System.Collections.IEnumerable.GetEnumerator"/>
        /// </summary>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion
    }
}
