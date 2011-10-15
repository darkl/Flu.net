using System;
using System.Collections.Generic;
using System.Linq;
using Flunet.Automata.Interfaces;
using Flunet.Extensions;

namespace Flunet.Automata.Language
{
    /// <summary>
    /// A utility class that provides binary operations for
    /// automatas.
    /// </summary>
    public class AutomataBinaryOperations
    {
        /// <summary>
        /// Gets a variation of two automatas.
        /// </summary>
        /// <remarks>
        /// A variation of two automatas is a new automata that
        /// is valid by a function f:Z/2Z X Z/2Z -> Z/2Z
        /// The function recieves two states validity, and decides
        /// if the new state is valid or not. (it could by a and, or, xor
        /// function, or any other function from the 16 functions)
        /// </remarks>
        /// <typeparam name="T">The type of the automata.</typeparam>
        /// <param name="first">The first automata.</param>
        /// <param name="second">The second automata.</param>
        /// <param name="variation">The variation function. (Usualy and/or)</param>
        /// <param name="idSelector">An id selector for the states of 
        /// the new automata.</param>
        /// <param name="equalityComparer">A comparer used to compare
        /// the alphabet types with each other.</param>
        /// <returns>The variation of the automatas.</returns>
        private static IDeterministicAutomata<T> GetVariation<T>
            (IDeterministicAutomata<T> first,
            IDeterministicAutomata<T> second,
            Func<bool, bool, bool> variation,
            Func<string, string, string> idSelector,
            IEqualityComparer<T> equalityComparer)
        {
            var result =
                new DeterministicAutomata<T>(equalityComparer);

            IDictionary<Tuple<IAutomataState<T>, IAutomataState<T>>, IExtendableAutomataState<T>> pairToNewStates =
                BuildCartesianProduct(first, second, variation, idSelector, result);

            LinkCartesianProduct(pairToNewStates);

            return result;
        }

        /// <summary>
        /// Builds a cartesian product of the two given automatas, that is
        /// a dictionary which maps each pair of automata states to a new
        /// automata state.
        /// </summary>
        /// <typeparam name="T">The type of the alphabet of the automata.</typeparam>
        /// <param name="first">The first automata.</param>
        /// <param name="second">The second automata.</param>
        /// <param name="variation">The variation function which chooses
        /// what states are valid.</param>
        /// <param name="idSelector">The id selector function which chooses
        /// the ids of the new states.</param>
        /// <param name="resultAutomata">A automata containing all
        /// new states.</param>
        /// <returns>A dictionary which maps each pair of automata states to a new
        /// automata state.</returns>
        private static IDictionary<Tuple<IAutomataState<T>, IAutomataState<T>>, IExtendableAutomataState<T>>
            BuildCartesianProduct<T>
            (IDeterministicAutomata<T> first,
             IDeterministicAutomata<T> second,
             Func<bool, bool, bool> variation,
             Func<string, string, string> idSelector,
             DeterministicAutomata<T> resultAutomata)
        {
            var statesMapping =
                from firstState in first
                from secondState in second
                let isNewStateValid = variation(firstState.IsValid, secondState.IsValid)
                let newStateName = idSelector(firstState.Id, secondState.Id)
                select new
                           {
                               FirstState = firstState,
                               SecondState = secondState,
                               NewState = resultAutomata.AddState(newStateName, 
                                                                  isNewStateValid)
                           };

            var pairToNewStates =
                statesMapping.ToDictionary
                    (x => x.FirstState,
                     x => x.SecondState,
                     x => x.NewState);

            return pairToNewStates;
        }

        /// <summary>
        /// Links a given cartesian product by the transitions of the states,
        /// i.e. links each new state to another new state by the corresponding state
        /// of the trasition applied by transiting each pair by the given input.
        /// </summary>
        /// <param name="pairToNewStates">The cartesian product of the
        /// states, that is a dictionary which maps each pair of automata states 
        /// to a new automata state.</param>
        private static void LinkCartesianProduct<T>
            (IDictionary<Tuple<IAutomataState<T>, IAutomataState<T>>, IExtendableAutomataState<T>> pairToNewStates)
        {
            foreach (var automataStateTuple in pairToNewStates.Keys)
            {
                var firstState = automataStateTuple.Item1;
                var secondState = automataStateTuple.Item2;

                List<T> currentAlphabet =
                    firstState.Select(x => x.Key)
                    .Union(secondState.Select(x => x.Key)).ToList();

                IExtendableAutomataState<T> current =
                    pairToNewStates.Get(firstState, secondState);

                foreach (T symbol in currentAlphabet)
                {
                    IAutomataState<T> firstTransition = firstState.Transit(symbol);
                    IAutomataState<T> secondTransition = secondState.Transit(symbol);

                    IExtendableAutomataState<T> currentTransition =
                        pairToNewStates.Get(firstTransition, secondTransition);

                    current.Add(symbol, currentTransition);
                }
            }
        }

        /// <summary>
        /// Gets the union of two given automatas.
        /// </summary>
        /// <param name="first">The first automata.</param>
        /// <param name="second">The second automata.</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/>
        /// used to compare the automata's alphabet.</param>
        /// <returns>The union of the given given automatas.</returns>
        public static IDeterministicAutomata<T> GetUnion<T>(IDeterministicAutomata<T> first, IDeterministicAutomata<T> second, IEqualityComparer<T> equalityComparer)
        {
            return GetVariation(first, second, (x, y) => x || y,
                                (x, y) => x + "Or" + y,
                                equalityComparer);
        }

        /// <summary>
        /// Gets the intersection of two given automatas.
        /// </summary>
        /// <param name="first">The first automata.</param>
        /// <param name="second">The second automata.</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/>
        /// used to compare the automata's alphabet.</param>
        /// <returns>The intersection of the given given automatas.</returns>
        public static IDeterministicAutomata<T> GetIntersection<T>(IDeterministicAutomata<T> first, IDeterministicAutomata<T> second, IEqualityComparer<T> equalityComparer)
        {
            return GetVariation(first, second, (x, y) => x && y,
                                (x, y) => x + "And" + y,
                                equalityComparer);
        }

        /// <summary>
        /// Gets the symmetric difference of two given automatas.
        /// </summary>
        /// <param name="first">The first automata.</param>
        /// <param name="second">The second automata.</param>
        /// <param name="equalityComparer">The <see cref="IEqualityComparer{T}"/>
        /// used to compare the automata's alphabet.</param>
        /// <returns>The symmetric difference of the given automatas.</returns>
        public static IDeterministicAutomata<T> GetSymmetricDifference<T>(IDeterministicAutomata<T> first, IDeterministicAutomata<T> second, IEqualityComparer<T> equalityComparer)
        {
            return GetVariation(first, second, (x, y) => x ^ y,
                                (x, y) => x + "Xor" + y,
                                equalityComparer);
        }
    }
}