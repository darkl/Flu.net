using System;
using System.Collections.Generic;
using Flunet.Automata.Interfaces;
using Flunet.Extensions;

namespace Flunet.Automata.FluentSyntax
{
    /// <summary>
    /// Provides fluent syntax for automatas.
    /// </summary>
    public partial class AutomataFluentSyntax
    {
        internal class AutomataSyntaxImplementer<T> : SyntaxImplementer<T>
        {
            private readonly IExtendableDeterministicAutomata<T> mAutomata;

            private readonly Dictionary<string, IExtendableAutomataState<T>> mPreparedStates =
                new Dictionary<string, IExtendableAutomataState<T>>();

            private IExtendableAutomataState<T> mCurrentState;
            private IExtendableAutomataState<T> mTransitedFrom;
            private IExtendableAutomataState<T> mTransitedTo;

            public AutomataSyntaxImplementer(IExtendableDeterministicAutomata<T> automata)
            {
                mAutomata = automata;
            }

            protected override SyntaxImplementer<T> InnerWithValidState(string name)
            {
                InnerAddState(name, true);

                return this;
            }

            protected override SyntaxImplementer<T> InnerWithInvalidState(string name)
            {
                InnerAddState(name, false);

                return this;
            }

            private void InnerAddState(string name, bool isValid)
            {
                mCurrentState =
                    new AutomataState<T>(name, isValid, mAutomata.Comparer);

                if (mAutomata.Root != null)
                {
                    mAutomata.Add(mCurrentState);
                }

                mPreparedStates.Add(name, mCurrentState);
            }

            protected override SyntaxImplementer<T> InnerIsRoot()
            {
                mAutomata.Add(mCurrentState);

                mAutomata.AddRange(mPreparedStates.Values);

                return this;
            }

            protected override SyntaxImplementer<T> InnerTransitFrom(string name)
            {
                if (!mPreparedStates.ContainsKey(name))
                {
                    throw new ArgumentException(string.Format("No state named {0} was added",
                                                              name),
                                                "name");
                }

                mTransitedFrom = mPreparedStates[name];
                mTransitedTo = mCurrentState;

                return this;
            }

            protected override SyntaxImplementer<T> InnerTransitTo(string name)
            {
                if (!mPreparedStates.ContainsKey(name))
                {
                    throw new ArgumentException(string.Format("No state named {0} was added",
                                                              name),
                                                "name");
                }

                mTransitedTo = mPreparedStates[name];
                mTransitedFrom = mCurrentState;

                return this;
            }

            protected override SyntaxImplementer<T> InnerOnInput(T input)
            {
                mTransitedFrom.Add(input, mTransitedTo);

                mTransitedTo = null;
                mTransitedFrom = null;

                return this;
            }
        }
    }
}