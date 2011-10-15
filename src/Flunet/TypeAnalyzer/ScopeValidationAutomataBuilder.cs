using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Flunet.Extensions;
using Flunet.Automata;
using Flunet.Automata.Interfaces;
using Flunet.Automata.Language;

namespace Flunet.TypeAnalyzer
{
    /// <summary>
    /// Represents a base class for <see cref="ITypeAutomataBuilder"/> that 
    /// are scope oriented validators.
    /// </summary>
    public abstract class ScopeValidationAutomataBuilder : ITypeAutomataBuilder
    {
        #region Constructors

        /// <summary>
        /// Creates a <see cref="ScopeValidationAutomataBuilder"/>
        /// given its <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The given type.</param>
        protected ScopeValidationAutomataBuilder(Type type)
        {
            this.Type = type;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type that this builder builds an automata for.
        /// </summary>
        protected Type Type
        {
            get;
            private set;
        }

        #endregion

        #region Abstract methods

        /// <summary>
        /// <see cref="ITypeAutomataBuilder.Build"/>
        /// </summary>
        public abstract IDeterministicAutomata<MethodInfo> Build(IDeterministicAutomata<MethodInfo> aggregatedAutomata);

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets a dictionary of scope names to their corresponding validation automatas
        /// and creates an automata that links them.
        /// </summary>
        /// <param name="scopesToValidation">A dictionary that
        /// maps scope names to their validating automatas.</param>
        /// <returns>An automata that links each scope to its
        /// corresponding validation automata</returns>
        protected IDeterministicAutomata<MethodInfo> LinkScopes(IDictionary<string, IDeterministicAutomata<MethodInfo>> scopesToValidation)
        {
            return LinkScopes(this.Type, scopesToValidation);
        }

        /// <summary>
        /// Inner implementation of <see cref="LinkScopes(System.Collections.Generic.IDictionary{string,Flunet.Automata.Interfaces.IDeterministicAutomata{System.Reflection.MethodInfo}})"/>.
        /// Gets a dictionary of scope names to their corresponding validation automatas
        /// and creates an automata in respect to the given type that links them.
        /// </summary>
        /// <param name="type">The type to build the automata in respect to.</param>
        /// <param name="scopesToValidation">A dictionary that
        /// maps scope names to their validating automatas.</param>
        /// <returns>An automata that links each scope to its
        /// corresponding validation automata</returns>
        private IDeterministicAutomata<MethodInfo> LinkScopes(Type type, IDictionary<string, IDeterministicAutomata<MethodInfo>> scopesToValidation)
        {
            IExtendableDeterministicAutomata<MethodInfo> result =
                FundamentalAutomatas.True("Root", new ToStringComparer<MethodInfo>());

            string currentScope = type.GetScopeName();

            IDeterministicAutomata<MethodInfo> currentScopeValidation =
                FundamentalAutomatas.True("Root", new ToStringComparer<MethodInfo>());

            if (scopesToValidation.ContainsKey(currentScope))
            {
                currentScopeValidation =
                    scopesToValidation[currentScope];
            }

            var relevantMethods =
                from method in type.GetMethods()
                let methodScope = method.ReturnType.GetScopeName()
                where methodScope != currentScope
                select method;

            foreach (MethodInfo method in relevantMethods)
            {
                IExtendableDeterministicAutomata<MethodInfo> childValidation =
                    new DeterministicAutomata<MethodInfo>(new ToStringComparer<MethodInfo>());

                IExtendableAutomataState<MethodInfo> childValidationRoot =
                    childValidation.AddState("Root", true);

                IDeterministicAutomata<MethodInfo> childLink =
                    LinkScopes(method.ReturnType, scopesToValidation);

                childValidationRoot.Add(method, childLink.Root);

                childValidation.AddRange(childLink);

                result = result.Intersect(childValidation);
            }

            result = result.Intersect(currentScopeValidation);

            return result;
        }

        /// <summary>
        /// Gets the reset tokens of the automata of the given scope.
        /// That is all methods from the given alphabet that return the scope's
        /// type.
        /// </summary>
        /// <param name="scopeName">The given scope's name.</param>
        /// <param name="alphabet">The given alphabet.</param>
        /// <returns>The requested scope's reset tokens.</returns>
        protected static List<MethodInfo> GetScopeResetTokens(string scopeName, IEnumerable<MethodInfo> alphabet)
        {
            return (from method in alphabet
                    let methodScope = method.ReturnType.GetScopeName()
                    let declaringScope = method.DeclaringType.GetScopeName()
                    where (methodScope == scopeName) &&
                          (declaringScope != scopeName)
                    select method).ToList();
        }

        #endregion
    }
}