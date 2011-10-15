using System.Collections.Generic;
using System.Linq;
using Flunet.Automata;
using Flunet.Automata.Interfaces;
using Flunet.Automata.Language;

namespace Flunet.Extensions
{
    /// <summary>
    /// Provides extension methods for <see cref="IAutomata{T}"/>.
    /// </summary>
    public static class AutomataExtensions
    {
        public static void ReadWord<T>(this IAutomata<T> automata, IEnumerable<T> word)
        {
            foreach (T symbol in word)
            {
                automata.Read(symbol);
            }
        }

        public static void AddRange<T>(this IExtendableDeterministicAutomata<T> source, IEnumerable<IAutomataState<T>> range)
        {
            foreach (var currentState in range)
            {
                if (!source.Contains(currentState))
                {
                    source.Add(currentState);
                }
            }
        }

        public static void AddRange<T>(this IExtendableAutomataState<T> source, IEnumerable<KeyValuePair<T, IAutomataState<T>>> range)
        {
            foreach (var currentState in range)
            {
                if (!source.Contains(currentState))
                {
                    source.Add(currentState.Key, currentState.Value);
                }
            }
        }

        public static IExtendableAutomataState<T> AddState<T>(this IExtendableDeterministicAutomata<T> automata, string id, bool isValid)
        {
            var result = new AutomataState<T>(id, isValid, automata.Comparer);

            automata.Add(result);

            return result;
        }

        public static bool IsReachableFrom<T>(this IAutomataState<T> source, IAutomataState<T> from)
        {
            return AutomataUnaryOperations.IsReachableFrom(source, from);
        }

        public static IExtendableDeterministicAutomata<T> WithoutRedundantStates<T>(this IDeterministicAutomata<T> source)
        {
            return AutomataUnaryOperations.ClearRedundantStates(source);
        }

        public static IExtendableDeterministicAutomata<T> Union<T>(this IDeterministicAutomata<T> source, IDeterministicAutomata<T> other)
        {
            return AutomataBinaryOperations.GetUnion(source.WithoutRedundantStates(), other.WithoutRedundantStates(), source.Comparer).WithoutRedundantStates();
        }

        public static IExtendableDeterministicAutomata<T> Intersect<T>(this IDeterministicAutomata<T> source, IDeterministicAutomata<T> other)
        {
            return AutomataBinaryOperations.GetIntersection(source.WithoutRedundantStates(), other.WithoutRedundantStates(), source.Comparer).WithoutRedundantStates();
        }

        public static IExtendableDeterministicAutomata<T> Not<T>(this IDeterministicAutomata<T> source)
        {
            return AutomataUnaryOperations.GetNot(source, source.Comparer);
        }
    }
}