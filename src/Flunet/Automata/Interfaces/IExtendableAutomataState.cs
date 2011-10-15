namespace Flunet.Automata.Interfaces
{
    /// <summary>
    /// Represents a <see cref="IAutomataState{T}"/> that
    /// can be extended from outside.
    /// </summary>
    /// <typeparam name="T">The automata's alphabet type.</typeparam>
    public interface IExtendableAutomataState<T> : IAutomataState<T>
    {
        /// <summary>
        /// Links the current state to the given state,
        /// on the given input. 
        /// </summary>
        /// <remarks>
        /// That means that when transited with the given input,
        /// the returned state will be the given state.
        /// </remarks>
        /// <param name="input">The given input.</param>
        /// <param name="state">The given state.</param>
        void Add(T input, IAutomataState<T> state);
    }
}
