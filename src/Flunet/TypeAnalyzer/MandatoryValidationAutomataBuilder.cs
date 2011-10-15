using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Flunet.Extensions;
using Flunet.Attributes;
using Flunet.Automata.Interfaces;
using Flunet.Automata.Language;

namespace Flunet.TypeAnalyzer
{
    /// <summary>
    /// Builds a validation automata due to <see cref="MandatoryAttribute"/>s
    /// on methods. That ensures you must call these methods before you can
    /// call other methods.
    /// </summary>
    /// <remarks>
    /// The built automata doesn't deal with all permutation orders of
    /// the mandatory methods. That is because the factorial function
    /// grows very fast!
    /// TODO: add support for aliases for mandatory methods.
    /// TODO: that is a group of methods which at least one of them
    /// TODO: is mandatory.
    /// </remarks>
    public class MandatoryValidationAutomataBuilder : ScopeValidationAutomataBuilder
    {
        #region Constructors

        /// <summary>
        /// Builds a <see cref="MandatoryValidationAutomataBuilder"/> in respect to
        /// the given type.
        /// </summary>
        /// <param name="type">The type to build the automata in respect to.</param>
        public MandatoryValidationAutomataBuilder(Type type)
            : base(type)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// <see cref="ITypeAutomataBuilder.Build"/>
        /// </summary>
        public override IDeterministicAutomata<MethodInfo> Build(IDeterministicAutomata<MethodInfo> aggregatedAutomata)
        {
            IDictionary<string, ICollection<MethodInfo>> scopesToMandatoryMethods =
                GatherScopesMandatoryMethods();

            ICollection<MethodInfo> alphabet = aggregatedAutomata.Alphabet;

            var scopesToMandatoryValidation =
                scopesToMandatoryMethods.ToDictionary
                    (x => x.Key,
                     x =>
                     (IDeterministicAutomata<MethodInfo>)FundamentalAutomatas.SymbolsAppearFirst
                         (alphabet,
                          x.Value,
                          GetScopeResetTokens(x.Key,
                                              alphabet),
                          new ToStringComparer<MethodInfo>()));

            IDeterministicAutomata<MethodInfo> mandatoryValidation = LinkScopes(scopesToMandatoryValidation);

            IDeterministicAutomata<MethodInfo> result =
                aggregatedAutomata.Intersect(mandatoryValidation);

            result.Alphabet.AddRange(alphabet);

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Builds a mapping between scope names to their available mandatory methods.
        /// </summary>
        /// <returns>A mapping between scope names to their available mandatory methods.</returns>
        private IDictionary<string, ICollection<MethodInfo>> GatherScopesMandatoryMethods()
        {
            IDictionary<string, ICollection<MethodInfo>> result =
                new Dictionary<string, ICollection<MethodInfo>>();

            GatherScopesMandatoryMethods(this.Type, result);

            return result;
        }

        /// <summary>
        /// Inner implementation of <see cref="GatherScopesMandatoryMethods()"/>.
        /// </summary>
        /// <param name="type">The current type to scan for mandatory methods.</param>
        /// <param name="scopeToMandatoryMethods">The mapping to fill.</param>
        private void GatherScopesMandatoryMethods(Type type,
                                                  IDictionary<string, ICollection<MethodInfo>> scopeToMandatoryMethods)
        {
            string currentScope = type.GetScopeName();

            var relevantMethods =
                (from method in type.GetMethods()
                 let methodScope = method.ReturnType.GetScopeName()
                 where methodScope != currentScope
                 select method).Distinct(new ToStringComparer<MethodInfo>());

            foreach (MethodInfo relevantMethod in type.GetMethods())
            {
                if (relevantMethod.HasAttribute<MandatoryAttribute>())
                {
                    scopeToMandatoryMethods.Add(currentScope, relevantMethod);
                }
            }

            foreach (MethodInfo method in relevantMethods)
            {
                GatherScopesMandatoryMethods(method.ReturnType,
                                             scopeToMandatoryMethods);
            }
        }

        #endregion
    }
}
