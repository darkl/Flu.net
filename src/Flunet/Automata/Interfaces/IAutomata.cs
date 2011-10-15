using System.Collections.Generic;

namespace Flunet.Automata.Interfaces
{
    /// <summary>
    /// Represents an automata.
    /// </summary>
    /// <typeparam name="T">The alphabet's type.</typeparam>
    public interface IAutomata<T>
    {
        /// <summary>
        /// Transits the automata to the required state, due to the
        /// given input.
        /// </summary>
        /// <param name="input">The given input.</param>
        void Read(T input);

        /// <summary>
        /// Resets the automata, allowing it to start reading another word.
        /// </summary>
        void Reset();

        /// <summary>
        /// Gets a value indicating whether the automata is currently in
        /// a valid state.
        /// </summary>
        bool IsValid {get;}
        
        /// <summary>
        /// Gets a comparer that is used by the automata to compare
        /// inputs.
        /// </summary>
        IEqualityComparer<T> Comparer { get; }
        
        /// <summary>
        /// Gets the valid alphabet of the current automata.
        /// </summary>
        ICollection<T> Alphabet { get; }
    }
}
