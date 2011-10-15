using System;
using System.Collections.Generic;
using System.Linq;
using Flunet.Automata.Interfaces;
using Flunet.Extensions;

namespace Flunet.Automata.Language
{
    /// <summary>
    /// Provides unary operations for <see cref="IDeterministicAutomata{T}"/>s.
    /// </summary>
    public static class AutomataUnaryOperations
    {
        #region Not Implementation

        /// <summary>
        /// Returns a not automata of the given automata.
        /// </summary>
        /// <param name="source">The given automata.</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/>
        /// to compare the automata's alphabet.</param>
        /// <returns>A not automtata of the given automata.</returns>
        public static IExtendableDeterministicAutomata<T> GetNot<T>(IDeterministicAutomata<T> source,
                                                                    IEqualityComparer<T> equalityComparer)
        {
            var result =
                new DeterministicAutomata<T>(equalityComparer);

            var dictionary = new Dictionary<IAutomataState<T>, IExtendableAutomataState<T>>();

            foreach (var state in source)
            {
                var automataState =
                    result.AddState(state.Id, !state.IsValid);

                dictionary[state] = automataState;
            }

            foreach (var current in source)
            {
                foreach (var currentSymbolToState in current)
                {
                    dictionary[current].Add(currentSymbolToState.Key,
                                            dictionary[currentSymbolToState.Value]);
                }
            }

            return result;
        }

        #endregion

        #region ClearRedundantStates Implementation

        /// <summary>
        /// Returns a new automata that agrees with the given automata on
        /// every word, but without redundant states.
        /// </summary>
        /// <param name="source">The automata to shrink.</param>
        /// <returns>A new automata that agrees with the given automata on
        /// every word, but without redundant states.</returns>
        public static IExtendableDeterministicAutomata<T> ClearRedundantStates<T>(IDeterministicAutomata<T> source)
        {
            DeterministicAutomata<T> result =
                new DeterministicAutomata<T>(source.Comparer);

            IAutomataState<T> root = source.Root;

            List<IAutomataState<T>> reachable =
                source.Where(x => IsReachableFrom(x, root)).ToList();

            var nonEquivalent =
                new HashSet<Tuple<IAutomataState<T>, IAutomataState<T>>>
                    (new SymmetricTupleComparer<IAutomataState<T>>());

            nonEquivalent.UnionWith(GetTrivialNonEquivalent(reachable));

            FindAllNonEquivalent(reachable, nonEquivalent);

            // After we find the equivalent states, we build new states from them.
            Dictionary<IAutomataState<T>, IExtendableAutomataState<T>> originalToNewState =
                BuildNewStatesFromEquivalence(result, reachable, nonEquivalent);

            // Then we link the equivalent states with each other.
            LinkNewStatesByEquivalentStates
                (reachable, nonEquivalent, originalToNewState);

            return result;
        }

        /// <summary>
        /// Finds the trivial non equivalent state pairs of the automata.
        /// That is - all the states that their <see cref="IAutomataState{T}.IsValid"/>
        /// property isn't equal.
        /// </summary>
        /// <param name="reachable">The states to check.</param>
        /// <returns>The trivial non equivalent state pairs from the given
        /// collection.</returns>
        private static IEnumerable<Tuple<IAutomataState<T>, IAutomataState<T>>> GetTrivialNonEquivalent<T>
            (ICollection<IAutomataState<T>> reachable)
        {
            IEnumerable<IAutomataState<T>> valid = reachable.Where(x => x.IsValid);
            IEnumerable<IAutomataState<T>> invalid = reachable.Where(x => !x.IsValid);

            IEnumerable<Tuple<IAutomataState<T>, IAutomataState<T>>> trivialNonEquivalent =
                from currentValid in valid
                from currentInvalid in invalid
                select Tuple.Create(currentValid, currentInvalid);

            return trivialNonEquivalent;
        }

        /// <summary>
        /// Finds all non equivalent states from a given collection.
        /// </summary>
        /// <param name="reachable">The given collection.</param>
        /// <param name="nonEquivalent">A collection to be filled with
        /// all non equivalent states. Should be passed with the
        /// trivial non equivalent states.</param>
        private static void FindAllNonEquivalent<T>(List<IAutomataState<T>> reachable,
                                                    HashSet<Tuple<IAutomataState<T>, IAutomataState<T>>> nonEquivalent)
        {
            int? count = null;
            int? previousCount;

            // Find all reachable pairs that are not equivalent -
            // that is all reachable pairs that have common transits
            // which are not equivalent.
            do
            {
                var candidates =
                    (from firstReachable in reachable
                     from secondReachable in reachable
                     where !nonEquivalent.Contains(firstReachable, secondReachable)
                     select new { firstReachable, secondReachable }).ToList();

                foreach (var candidate in candidates)
                {
                    IEnumerable<T> currentAlphabet =
                        candidate.firstReachable.Select(x => x.Key).Union(candidate.secondReachable.Select(x => x.Key));

                    if (currentAlphabet.Any(x => nonEquivalent.Contains(candidate.firstReachable.Transit(x),
                                                                        candidate.secondReachable.Transit(x))))
                    {
                        nonEquivalent.Add(candidate.firstReachable,
                                          candidate.secondReachable);
                    }
                }

                previousCount = count;
                count = candidates.Count;
            }
            while (previousCount != count);
        }

        /// <summary>
        /// Builds new states for each equivalent set of states.
        /// </summary>
        /// <param name="automata">The automata to add the states to.</param>
        /// <param name="reachable">All the original states.</param>
        /// <param name="nonEquivalent">A mapping of all non equivalent states.</param>
        /// <returns>A dictionary that maps each state to its new state.</returns>
        private static Dictionary<IAutomataState<T>, IExtendableAutomataState<T>> BuildNewStatesFromEquivalence<T>
            (IExtendableDeterministicAutomata<T> automata,
             IEnumerable<IAutomataState<T>> reachable,
             HashSet<Tuple<IAutomataState<T>, IAutomataState<T>>> nonEquivalent)
        {
            Dictionary<IAutomataState<T>, IExtendableAutomataState<T>> originalToNewState =
                reachable.GroupBy(x => x,
                                  new NonEquivalentComparer<IAutomataState<T>>(nonEquivalent))
                    .Select((x, i) => new
                                          {
                                              EquivalentStates = x,
                                              NewState = automata.AddState(i.ToString(), x.Key.IsValid)
                                          })
                    .SelectMany(x => x.EquivalentStates.Select(state => new { Original = state, x.NewState }))
                    .ToDictionary(x => x.Original,
                                  x => x.NewState);

            return originalToNewState;
        }

        /// <summary>
        /// Links the new states to each other, by the links of
        /// the original states that are equivalent to it.
        /// </summary>
        /// <param name="reachable">The original states.</param>
        /// <param name="nonEquivalent">The none equivalent states.</param>
        /// <param name="originalToNewState">The mapping between the original states
        /// to their corresponding new states.</param>
        private static void LinkNewStatesByEquivalentStates<T>
            (IEnumerable<IAutomataState<T>> reachable,
             HashSet<Tuple<IAutomataState<T>, IAutomataState<T>>> nonEquivalent,
             IDictionary<IAutomataState<T>, IExtendableAutomataState<T>> originalToNewState)
        {
            foreach (var currentState in reachable.Distinct(new NonEquivalentComparer<IAutomataState<T>>(nonEquivalent)))
            {
                IExtendableAutomataState<T> newState = originalToNewState[currentState];

                foreach (var inputToState in currentState)
                {
                    newState.Add(inputToState.Key,
                                 originalToNewState[inputToState.Value]);
                }
            }
        }

        #endregion

        #region IsReachableFrom Implementation

        /// <summary>
        /// Returns a value indicating whether the given automata state
        /// is reachable from the other automata state.
        /// </summary>
        /// <param name="source">The automata state to check that is reachable from.</param>
        /// <param name="from">The automata state to start the search from.</param>
        /// <returns>A value indicating whether the given automata state
        /// is reachable from the other automata state.</returns>
        public static bool IsReachableFrom<T>(IAutomataState<T> source, IAutomataState<T> from)
        {
            return IsReachableFrom(source, from, new List<IAutomataState<T>>());
        }

        /// <summary>
        /// Returns a value indicating whether the given automata state
        /// is reachable from the other automata state.
        /// </summary>
        /// <param name="source">The automata state to check that is reachable from.</param>
        /// <param name="from">The automata state to start the search from.</param>
        /// <returns>A value indicating whether the given automata state
        /// is reachable from the other automata state.</returns>
        /// <param name="visited">The states already visited, in order
        /// to avoid a cyclic loop.</param>
        private static bool IsReachableFrom<T>(IAutomataState<T> source,
                                               IAutomataState<T> from,
                                               ICollection<IAutomataState<T>> visited)
        {
            if (source == from)
            {
                return true;
            }
            else
            {
                visited.Add(from);

                return from.Select(x => x.Value)
                    .Except(visited).
                    Any(x => IsReachableFrom(source, x, visited));
            }
        }

        #endregion
    }
}