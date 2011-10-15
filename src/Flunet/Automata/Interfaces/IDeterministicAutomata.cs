using System.Collections.Generic;

namespace Flunet.Automata.Interfaces
{
    /// <summary>
    /// Represents an state driven automata.
    /// </summary>
    /// <typeparam name="T">The type of the alphabet of the automata.</typeparam>
    public interface IDeterministicAutomata<T> : IAutomata<T>, IEnumerable<IAutomataState<T>>
    {
        /// <summary>
        /// Gets the root of the automata.
        /// </summary>
        IAutomataState<T> Root { get; }

        /// <summary>
        /// Gets the current state of the automata.
        /// </summary>
        IAutomataState<T> CurrentState { get; }
    }
}
