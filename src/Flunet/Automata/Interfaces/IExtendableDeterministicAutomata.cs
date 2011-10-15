namespace Flunet.Automata.Interfaces
{
    /// <summary>
    /// Represents a <see cref="IDeterministicAutomata{T}"/>
    /// that can be extended from outside.
    /// </summary>
    /// <typeparam name="T">The given automata's alphabet type.</typeparam>
    public interface IExtendableDeterministicAutomata<T> : IDeterministicAutomata<T>
    {
        /// <summary>
        /// Adds a given state to the automata.
        /// </summary>
        /// <param name="state">The given state.</param>
        void Add(IAutomataState<T> state);
    }
}
