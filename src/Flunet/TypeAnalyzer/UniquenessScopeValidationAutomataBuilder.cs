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
    /// Builds an automata that checks that validates that every
    /// method with a <see cref="UniqueInScopeAttribute"/> is unique in its scope.
    /// </summary>
    public class UniquenessScopeValidationAutomataBuilder : ScopeValidationAutomataBuilder
    {
        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="UniquenessScopeValidationAutomataBuilder"/>
        /// in respect to the given type.
        /// </summary>
        /// <param name="type">The given type.</param>
        public UniquenessScopeValidationAutomataBuilder(Type type)
            : base(type)
        {
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// <see cref="ITypeAutomataBuilder.Build"/>.
        /// Builds a uniquess validation automata.
        /// </summary>
        public override IDeterministicAutomata<MethodInfo> Build(IDeterministicAutomata<MethodInfo> aggregatedAutomata)
        {
            IDictionary<string, ICollection<MethodInfo>> aliasesToMethods =
                GatherAliasesToMethods();

            IDictionary<string, ICollection<string>> scopesToConstraints =
                GatherScopeConstraints();

            ICollection<MethodInfo> alphabet = aggregatedAutomata.Alphabet;

            Dictionary<string, IDeterministicAutomata<MethodInfo>> scopesToUniquenessValidation =
                scopesToConstraints.ToDictionary
                    (x => x.Key,
                     x => BuildScopeValidation(x.Key,
                                               x.Value.Select(y => aliasesToMethods[y]),
                                               alphabet));

            IDeterministicAutomata<MethodInfo> linkedScopes =
                LinkScopes(scopesToUniquenessValidation);

            IExtendableDeterministicAutomata<MethodInfo> result =
                linkedScopes.Intersect(aggregatedAutomata);

            result.Alphabet.AddRange(aggregatedAutomata.Alphabet);

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Gathers a mapping between the aliases names to all methods
        /// with the given alias.
        /// </summary>
        /// <returns>The mapping between the aliases names to all methods
        /// with the given alias.</returns>
        private IDictionary<string, ICollection<MethodInfo>> GatherAliasesToMethods()
        {
            Dictionary<string, ICollection<MethodInfo>> result =
                new Dictionary<string, ICollection<MethodInfo>>();

            GatherAliasesToMethods(this.Type, result);

            return result;
        }

        /// <summary>
        /// Inner implementation of <see cref="GatherAliasesToMethods()"/>.
        /// </summary>
        /// <param name="type">The current type to scan for aliases.</param>
        /// <param name="aliasesToMethods">The mapping between aliases to methods
        /// to fill.</param>
        private static void GatherAliasesToMethods(Type type, IDictionary<string, ICollection<MethodInfo>> aliasesToMethods)
        {
            foreach (MethodInfo currentMethod in type.GetMethods())
            {
                if (currentMethod.HasAttribute<UniqueInScopeAttribute>())
                {
                    string methodAlias = currentMethod.Name;

                    AliasAttribute methodAliasAttribute =
                        currentMethod.GetAttribute<AliasAttribute>();

                    if (methodAliasAttribute != null)
                    {
                        methodAlias = methodAliasAttribute.Alias;
                    }

                    aliasesToMethods.Add(methodAlias,
                                         currentMethod);
                }

                if (currentMethod.ReturnType != type)
                {
                    GatherAliasesToMethods(currentMethod.ReturnType, aliasesToMethods);
                }
            }
        }

        /// <summary>
        /// Gathers a mapping between the scope names to the unique aliases
        /// in them.
        /// </summary>
        /// <returns>The following mapping.</returns>
        private IDictionary<string, ICollection<string>> GatherScopeConstraints()
        {
            Dictionary<string, ICollection<string>> result =
                new Dictionary<string, ICollection<string>>();

            GatherScopeConstraints(this.Type, result);

            return result;
        }

        /// <summary>
        /// Inner implementation of <see cref="GatherScopeConstraints()"/>.
        /// </summary>
        /// <param name="type">The current type to scan for 
        /// <see cref="UniqueInScopeAttribute"/> attributes.</param>
        /// <param name="scopesToAliases">The mapping to fill.</param>
        private static void GatherScopeConstraints(Type type, IDictionary<string, ICollection<string>> scopesToAliases)
        {
            foreach (MethodInfo currentMethod in type.GetMethods())
            {
                UniqueInScopeAttribute uniqueInScopeAttribute =
                    currentMethod.GetAttribute<UniqueInScopeAttribute>();

                if (uniqueInScopeAttribute != null)
                {
                    string methodAlias = currentMethod.Name;

                    AliasAttribute methodAliasAttribute =
                        currentMethod.GetAttribute<AliasAttribute>();

                    if (methodAliasAttribute != null)
                    {
                        methodAlias = methodAliasAttribute.Alias;
                    }

                    scopesToAliases.Add(uniqueInScopeAttribute.Scope,
                                       methodAlias);
                }

                if (currentMethod.ReturnType != type)
                {
                    GatherScopeConstraints(currentMethod.ReturnType, scopesToAliases);
                }
            }
        }

        /// <summary>
        /// Builds a validation automata for a given scope, given the methods
        /// that are unique in it, and the alphabet of the automata.
        /// </summary>
        /// <param name="scopeName">The given scope's name.</param>
        /// <param name="constraints">The methods that are unique in the givn
        /// scope.</param>
        /// <param name="alphabet">The alphabet of the automata.</param>
        /// <returns>The validation automata for the given scope.</returns>
        private static IDeterministicAutomata<MethodInfo> BuildScopeValidation(string scopeName, IEnumerable<ICollection<MethodInfo>> constraints, IEnumerable<MethodInfo> alphabet)
        {
            List<MethodInfo> scopeResetTokens =
                GetScopeResetTokens(scopeName, alphabet);

            var result = FundamentalAutomatas.True("Root", new ToStringComparer<MethodInfo>());

            foreach (ICollection<MethodInfo> constraint in constraints)
            {
                var constraintValidation =
                    FundamentalAutomatas.ZeroOrOneTimesSymbol
                        (constraint,
                         scopeResetTokens,
                         new ToStringComparer<MethodInfo>());

                result =
                    result.Intersect(constraintValidation);
            }

            return result;
        }

        #endregion
    }
}
