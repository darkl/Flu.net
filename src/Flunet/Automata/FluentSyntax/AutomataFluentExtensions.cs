using Flunet.Automata.Interfaces;

namespace Flunet.Automata.FluentSyntax
{
#pragma warning disable 1591

    public static class AutomataFluentExtensions
    {
        public static AutomataFluentSyntax.IState2<T> WithValidState<T>(this IExtendableDeterministicAutomata<T> automata, string name)
        {
            AutomataFluentSyntax.IState0<T> result = new AutomataFluentSyntax.AutomataSyntaxImplementer<T>(automata);

            return result.WithValidState(name);
        }

        public static AutomataFluentSyntax.IState2<T> WithInvalidState<T>(this IExtendableDeterministicAutomata<T> automata, string name)
        {
            AutomataFluentSyntax.IState0<T> result = new AutomataFluentSyntax.AutomataSyntaxImplementer<T>(automata);

            return result.WithInvalidState(name);
        }
    }

#pragma warning restore 1591
}