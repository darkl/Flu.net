using System.Collections.Generic;
using System.Linq;
using Flunet.Extensions;
using Flunet.Automata.Interfaces;

namespace Flunet.Automata.Language
{
    /// <summary>
    /// Provides common automata languages constructs.
    /// </summary>
    public static class FundamentalAutomatas
    {
        #region Trivial Constructs

        /// <summary>
        /// Gets an automata that validates on all words from the
        /// alphabet.
        /// </summary>
        /// <param name="rootName">The name of the root state of the
        /// automata.</param>
        /// <returns>An automata that validates on all words from the
        /// alphabet.</returns>
        public static IExtendableDeterministicAutomata<T> True<T>(string rootName)
        {
            return True(rootName, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Gets an automata that validates on all words from the
        /// alphabet.
        /// </summary>
        /// <param name="rootName">The name of the root state of the
        /// automata.</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/>
        /// to use in order to compare the alphabet type's instances.</param>
        /// <returns>An automata that validates on all words from the
        /// alphabet.</returns>
        public static IExtendableDeterministicAutomata<T> True<T>(string rootName, IEqualityComparer<T> equalityComparer)
        {
            var result = new DeterministicAutomata<T>(equalityComparer);
            result.AddState(rootName, true);
            return result;
        }

        /// <summary>
        /// Gets an automata that is invalid on all words from the
        /// alphabet.
        /// </summary>
        /// <param name="rootName">The name of the root state of the
        /// automata.</param>
        /// <returns>An automata that is invalid on all words from the
        /// alphabet.</returns>
        public static IExtendableDeterministicAutomata<T> False<T>(string rootName)
        {
            return False(rootName, EqualityComparer<T>.Default);
        }

        /// <summary>
        /// Gets an automata that is invalid on all words from the
        /// alphabet.
        /// </summary>
        /// <param name="rootName">The name of the root state of the
        /// automata.</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/>
        /// to use in order to compare the alphabet type's instances.</param>
        /// <returns>An automata that is invalid on all words from the
        /// alphabet.</returns>
        public static IExtendableDeterministicAutomata<T> False<T>(string rootName, IEqualityComparer<T> equalityComparer)
        {
            DeterministicAutomata<T> result = new DeterministicAutomata<T>(equalityComparer);
            result.AddState(rootName, false);
            return result;
        }

        #endregion

        #region Common Languages

        /// <summary>
        /// Gets a language that allows a colllection of symbols to appear
        /// at most once.
        /// </summary>
        /// <param name="symbols">The symbols that can appear (as a set)
        /// at most once.</param>
        /// <param name="resetTokens">A collection of symbols that reset
        /// the automata to the initial state.</param>
        /// <param name="equalityComparer">An <see cref="IEqualityComparer{T}"/> to 
        /// use in order to compare alphabet type's instances.</param>
        /// <typeparam name="T">The type of the alphabet of the automata.</typeparam>
        /// <returns>a automata that represents a language that allows a 
        /// colllection of symbols to appear at most once.</returns>
        public static IDeterministicAutomata<T> ZeroOrOneTimesSymbol<T>(ICollection<T> symbols, IEnumerable<T> resetTokens, IEqualityComparer<T> equalityComparer)
        {
            DeterministicAutomata<T> result = new DeterministicAutomata<T>(equalityComparer);

            var firstState = result.AddState("None", true);

            var secondState = result.AddState("Once", true);

            foreach (T symbol in symbols)
            {
                firstState.Add(symbol, secondState);
            }

            var thirdState = result.AddState("MoreThanOnce", false);

            foreach (T symbol in symbols)
            {
                secondState.Add(symbol, thirdState);
            }

            foreach (T resetToken in resetTokens)
            {
                secondState.Add(resetToken, firstState);
            }

            return result;
        }

        /// <summary>
        /// Gets an automata that allows words to begin only with the given
        /// symbols.
        /// </summary>
        /// <remarks>
        /// This doesn't consider permutations of the words, because
        /// the factorial function is fast increasing.
        /// </remarks>
        /// <param name="alphabet">The alphabet of the automata.</param>
        /// <param name="symbols">The symbols that must appear in
        /// the beginning of the words.</param>
        /// <param name="resetTokens">A collection of symbols that
        /// restart the automata to the initial state.</param>
        /// <param name="equalityComparer">An <see cref="IEqualityComparer{T}"/> to 
        /// use in order to compare alphabet type's instances.</param>
        /// <returns>An automata that allows words to begin only with the given
        /// symbols.</returns>
        public static IExtendableDeterministicAutomata<T> SymbolsAppearFirst<T>
            (IEnumerable<T> alphabet,
             ICollection<T> symbols,
             IEnumerable<T> resetTokens,
             IEqualityComparer<T> equalityComparer)
        {
            DeterministicAutomata<T> result = 
                new DeterministicAutomata<T>(equalityComparer);

            var states =
                symbols.Select((symbol, index) =>
                               result.AddState(index.ToString(), true)).ToList();

            var finished = result.AddState("Valid", true);
            states.Add(finished);

            var invalid = result.AddState("Invalid", false);

            var symbolsToStates =
                symbols.Select((symbol, index) =>
                               new
                                   {
                                       Symbol = symbol,
                                       State = states[index]
                                   });

            foreach (var symbolToState in symbolsToStates)
            {
                var symbol = symbolToState.Symbol;
                var state = symbolToState.State;

                state.AddRange
                    (alphabet.Except(new[] {symbol}, equalityComparer).Select
                         (badSymbol => new KeyValuePair<T, IAutomataState<T>>(
                                           badSymbol, invalid)));
            }

            int i = 0;

            foreach (T symbol in symbols)
            {
                states[i].Add(symbol, states[i + 1]);
                i++;
            }

            foreach (T resetToken in resetTokens)
            {
                finished.Add(resetToken, states[0]);
            }

            return result;
        }

        /// <summary>
        /// Creates an automata that validates only words that are built
        /// from a given collection of symbols.
        /// </summary>
        /// <param name="alphabet">The alphabet of the automata.</param>
        /// <param name="symbols">The symbols that words are
        /// allowed to be built from.</param>
        /// <param name="equalityComparer">An <see cref="IEqualityComparer{T}"/> to 
        /// use in order to compare alphabet type's instances.</param>
        /// <returns>An automata that validates only words that are built
        /// from a given collection of symbols.</returns>
        public static IExtendableDeterministicAutomata<T> OnlySymbolsAreLegal<T>
            (IEnumerable<T> alphabet,
            IEnumerable<T> symbols,
            IEqualityComparer<T> equalityComparer)
        {
            DeterministicAutomata<T> result =
                new DeterministicAutomata<T>(equalityComparer);

            var root =
                result.AddState("Root", true);

            var invalid =
                result.AddState("Invalid", false);

            foreach (var item in alphabet.Except(symbols, equalityComparer))
            {
                root.Add(item, invalid);
            }

            return result;
        }

        #endregion
    }
}