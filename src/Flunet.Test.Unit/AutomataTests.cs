using System;
using System.Collections.Generic;
using System.Linq;
using Flunet.Automata;
using Flunet.Automata.Interfaces;
using NUnit.Framework;

namespace Flunet.Test.Unit
{
    [TestFixture]
    public class AutomataTests
    {
        [Test]
        public void Root_NoStates_Null()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            Assert.That(automata.Root, Is.Null);
        }

        [Test]
        public void IsValid_NoStates_Exception()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            Assert.Throws<InvalidOperationException>
                (() => { bool isValid = automata.IsValid; });
        }

        [Test]
        public void CurrentState_NoElement_Null()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            Assert.That(automata.CurrentState, Is.Null);
        }

        [Test]
        public void Add_SingleElement_RootChanged()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> state = 
                new AutomataState<int>("Root", true);

            automata.Add(state);

            Assert.That(automata.Root, Is.SameAs(state));            
        }

        [Test]
        public void Add_FewElements_RootIsFirst()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> root =
                new AutomataState<int>("Root", true);

            AutomataState<int> state1 =
                new AutomataState<int>("State1", true);

            AutomataState<int> state2 =
                new AutomataState<int>("State2", true);

            automata.Add(root);
            automata.Add(state1);
            automata.Add(state2);

            Assert.That(automata.Root, Is.SameAs(root));
        }

        [Test]
        public void Add_SingleElement_CurrentStateChanged()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> state =
                new AutomataState<int>("Root", true);

            automata.Add(state);

            Assert.That(automata.CurrentState, Is.SameAs(state));
        }

        [Test]
        public void Add_FewElements_CurrentStateIsFirst()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> root =
                new AutomataState<int>("Root", true);

            AutomataState<int> state1 =
                new AutomataState<int>("State1", true);

            AutomataState<int> state2 =
                new AutomataState<int>("State2", true);

            automata.Add(root);
            automata.Add(state1);
            automata.Add(state2);

            Assert.That(automata.CurrentState, Is.SameAs(root));
        }

        [Test]
        public void GetEnumerator_NoElements_NoEnumeration()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            Assert.IsTrue(!automata.Any());
        }

        [Test]
        public void GetEnumerator_SingleElement_SingleEnumeration()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> root =
                new AutomataState<int>("Root", true);

            automata.Add(root);

            HashSet<IAutomataState<int>> expectedEnumeration =
                new HashSet<IAutomataState<int>>()
                    {
                        root,
                    };

            Assert.IsTrue(expectedEnumeration.SetEquals(automata));
        }

        [Test]
        public void GetEnumerator_FewElements_ExpectedEnumeration()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> root =
                new AutomataState<int>("Root", true);

            AutomataState<int> state1 =
                new AutomataState<int>("State1", true);

            AutomataState<int> state2 =
                new AutomataState<int>("State2", true);

            automata.Add(root);
            automata.Add(state1);
            automata.Add(state2);

            HashSet<IAutomataState<int>> expectedEnumeration =
                new HashSet<IAutomataState<int>>()
                    {
                        root,
                        state1,
                        state2
                    };

            Assert.IsTrue(expectedEnumeration.SetEquals(automata));
        }

        [Test]
        public void Read_NoElements_Exception()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            Assert.Throws<InvalidOperationException>(() => automata.Read(1));
        }

        [Test]
        public void Read_SingleState_Self()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();
            
            AutomataState<int> root = 
                new AutomataState<int>("Root", true);

            automata.Add(root);

            automata.Read(3);

            Assert.That(automata.CurrentState, Is.SameAs(root));
            Assert.That(automata.IsValid, Is.EqualTo(root.IsValid));
        }

        [Test]
        public void Read_TwoStatesSingleInput_SecondState()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> valid =
                new AutomataState<int>("Valid", true);

            AutomataState<int> invalid =
                new AutomataState<int>("Invalid", false);

            automata.Add(valid);
            automata.Add(invalid);

            valid.Add(3, invalid);
            valid.Add(4, valid);

            automata.Read(3);

            Assert.That(automata.CurrentState, Is.SameAs(invalid));
            Assert.That(automata.IsValid, Is.EqualTo(false));
        }

        [Test]
        public void Read_TwoStatesTwoInputs_SecondState()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> valid =
                new AutomataState<int>("Valid", true);

            AutomataState<int> invalid =
                new AutomataState<int>("Invalid", false);

            automata.Add(valid);
            automata.Add(invalid);

            valid.Add(3, invalid);
            valid.Add(4, valid);

            automata.Read(3);
            automata.Read(5);

            Assert.That(automata.CurrentState, Is.SameAs(invalid));
            Assert.That(automata.IsValid, Is.EqualTo(false));
        }

        [Test]
        public void Read_TwoStatesSingleInput_FirstState()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> valid =
                new AutomataState<int>("Valid", true);

            AutomataState<int> invalid =
                new AutomataState<int>("Invalid", false);

            automata.Add(valid);
            automata.Add(invalid);

            valid.Add(3, invalid);
            valid.Add(4, valid);

            automata.Read(4);

            Assert.That(automata.CurrentState, Is.SameAs(valid));
            Assert.That(automata.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void Read_TwoStatesTwoInputs_FirstState()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> valid =
                new AutomataState<int>("Valid", true);

            AutomataState<int> invalid =
                new AutomataState<int>("Invalid", false);

            automata.Add(valid);
            automata.Add(invalid);

            valid.Add(3, invalid);
            invalid.Add(4, valid);

            automata.Read(3);
            automata.Read(4);

            Assert.That(automata.CurrentState, Is.SameAs(valid));
            Assert.That(automata.IsValid, Is.EqualTo(true));
        }

        [Test]
        public void Reset_NoStates_Exception()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            Assert.Throws<InvalidOperationException>(() => automata.Reset());
        }


        [Test]
        public void Reset_SingleState_Root()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> valid =
                new AutomataState<int>("Valid", true);

            automata.Add(valid);

            automata.Reset();

            Assert.That(automata.CurrentState, Is.SameAs(valid));
        }

        [Test]
        public void Reset_TwoStatesSingleInput_Root()
        {
            DeterministicAutomata<int> automata =
                new DeterministicAutomata<int>();

            AutomataState<int> valid =
                new AutomataState<int>("Valid", true);

            AutomataState<int> invalid =
                new AutomataState<int>("Invalid", false);

            automata.Add(valid);
            automata.Add(invalid);

            valid.Add(3, invalid);
            valid.Add(4, valid);

            automata.Read(3);
            automata.Reset();

            Assert.That(automata.CurrentState, Is.SameAs(valid));
        }

    }
}
