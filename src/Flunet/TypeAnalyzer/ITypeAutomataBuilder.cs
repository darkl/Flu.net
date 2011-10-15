using System.Reflection;
using Flunet.Automata.Interfaces;

namespace Flunet.TypeAnalyzer
{
    /// <summary>
    /// Represents a specific purpose automata builder. 
    /// (Like legal syntax validation, and other validations)
    /// </summary>
    public interface ITypeAutomataBuilder
    {
        /// <summary>
        /// Builds a new automata given the aggregated automata.
        /// </summary>
        /// <param name="aggregatedAutomata">The given aggregated automata.</param>
        /// <returns>A new built automata.</returns>
        IDeterministicAutomata<MethodInfo> Build(IDeterministicAutomata<MethodInfo> aggregatedAutomata);
    }
}