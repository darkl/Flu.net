using System;
using System.Linq;
using System.Collections.Generic;
using Flunet.Automata;
using Flunet.Automata.Interfaces;
using NUnit.Framework;

namespace Flunet.Test.Unit
{
    [TestFixture]
    public class AutomataStateTests
    {
        [Test]
        public void Transit_UnknownInput_ReturnSelf()
        {
            AutomataState<int> q1 = new AutomataState<int>("Q1", true);
            int unknownInput = -1;

            IAutomataState<int> transitedState = q1.Transit(unknownInput);

            Assert.That(transitedState, Is.SameAs(q1));
        }

        [Test]
        public void Transit_InputToSelf_ReturnSelf()
        {
            AutomataState<int> q1 = new AutomataState<int>("Q1", true);
            int knownInput = 1;
            q1.Add(knownInput, q1);

            IAutomataState<int> transitedState = q1.Transit(knownInput);

            Assert.That(transitedState, Is.SameAs(q1));
        }

        [Test]
        public void Transit_InputToSecondState_ReturnSecondState()
        {
            AutomataState<int> q1 = new AutomataState<int>("Q1", true);
            AutomataState<int> q2 = new AutomataState<int>("Q2", true);
            int inputToSecondState = 1;
            q1.Add(inputToSecondState, q2);

            IAutomataState<int> transitedState = q1.Transit(inputToSecondState);

            Assert.That(transitedState, Is.SameAs(q2));
        }

        [Test]
        public void Add_AddingSameInputTwice_ThrowsException()
        {
            AutomataState<int> q1 = new AutomataState<int>("Q1", true);
            int inputToSelf = 1;
            q1.Add(inputToSelf, q1);

            Assert.Throws<ArgumentException>(() => q1.Add(inputToSelf, q1));
        }

        [Test]
        public void GetEnumerator_NoTransitions_NoEnumeration()
        {
            AutomataState<int> state = new AutomataState<int>("Root", true);

            Assert.IsTrue(!state.Any());
        }

        [Test]
        public void GetEnumerator_SingleTransition_EnumeratesTransition()
        {
            AutomataState<int> state = new AutomataState<int>("Root", true);
            AutomataState<int> state1 = new AutomataState<int>("State1", true);

            state.Add(1, state1);

            Dictionary<int, IAutomataState<int>> exceptedEnumeration =
                new Dictionary<int, IAutomataState<int>>()
                    {
                        {1, state1}
                    };

            Assert.IsTrue(state.SequenceEqual(exceptedEnumeration));
        }

        [Test]
        public void GetEnumerator_AddingFewTransition_EnumeratesTransition()
        {
            AutomataState<int> state = new AutomataState<int>("Root", true);

            AutomataState<int> state1 = new AutomataState<int>("State1", true);
            AutomataState<int> state2 = new AutomataState<int>("State2", true);
            AutomataState<int> state3 = new AutomataState<int>("State3", true);
            
            state.Add(1, state1);
            state.Add(2, state2);
            state.Add(3, state3);

            Dictionary<int, IAutomataState<int>> exceptedEnumeration =
                new Dictionary<int, IAutomataState<int>>()
                    {
                        {1, state1},
                        {2, state2},
                        {3, state3}
                    };

            Assert.IsTrue(state.SequenceEqual(exceptedEnumeration));
        }
    }
}
