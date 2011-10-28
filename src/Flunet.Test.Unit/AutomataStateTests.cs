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
        [ExpectedException(typeof(System.ArgumentException))]
        public void Add_AddingSameInputTwice_ThrowsException()
        {
            AutomataState<int> q1 = new AutomataState<int>("Q1", true);
            int inputToSelf = 1;
            q1.Add(inputToSelf, q1);
            q1.Add(inputToSelf, q1);
        }


    }
}
