using System.Collections.Generic;

namespace Flunet.Automata.Interfaces
{
    /// <summary>
    /// Represents a state of an <see cref="IAutomata{T}"/>.
    /// </summary>
    /// <typeparam name="T">The input type of the alphabet of the automata.</typeparam>
    public interface IAutomataState<T> : IEnumerable<KeyValuePair<T, IAutomataState<T>>>
    {
        /// <summary>
        /// Returns the state that is transited by the given input.
        /// </summary>
        /// <param name="input">The given input.</param>
        /// <returns>The state that is transited by the given input.</returns>
        IAutomataState<T> Transit(T input);
        
        /// <summary>
        /// Gets an unique name that represents the state.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a value indicating whether the current state is valid.
        /// </summary>
        bool IsValid { get; }
    }
}
