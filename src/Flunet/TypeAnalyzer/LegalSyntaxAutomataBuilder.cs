using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Flunet.Extensions;
using Flunet.Attributes;
using Flunet.Automata;
using Flunet.Automata.Interfaces;
using Flunet.Automata.Language;

namespace Flunet.TypeAnalyzer
{
    /// <summary>
    /// Builds an automata that validates legal syntax,
    /// that is - from a scope only the types that are declared
    /// in it are allowed.
    /// </summary>
    public class LegalSyntaxAutomataBuilder : ITypeAutomataBuilder
    {
        #region Constructors

        /// <summary>
        /// Builds a new instance of <see cref="LegalSyntaxAutomataBuilder"/>.
        /// </summary>
        /// <param name="type">The type to build the automata in
        /// respect to.</param>
        public LegalSyntaxAutomataBuilder(Type type)
        {
            this.Type = type;
        }

        #endregion

        #region Properties

        /// <summary>
        /// The type to build the automata in respect to.
        /// </summary>
        private Type Type
        {
            get; 
            set;
        }

        #endregion

        #region Public Members

        /// <summary>
        /// <see cref="ITypeAutomataBuilder.Build"/>
        /// </summary>
        public IDeterministicAutomata<MethodInfo> Build(IDeterministicAutomata<MethodInfo> aggregatedAutomata)
        {
            IDictionary<Type, ICollection<MethodInfo>> typeToMethods =
                GatherTypeToMethods();

            ICollection<MethodInfo> alphabet =
                typeToMethods.Values.SelectMany(x => x)
                    .Distinct(new ToStringComparer<MethodInfo>())
                    .ToList();

            IDictionary<Type, IExtendableDeterministicAutomata<MethodInfo>> typeToAutomata =
                typeToMethods.ToDictionary
                    (x => x.Key,
                     x => FundamentalAutomatas.OnlySymbolsAreLegal(alphabet, x.Value, new ToStringComparer<MethodInfo>()),
                     new ToStringComparer<Type>());

            IExtendableDeterministicAutomata<MethodInfo> result =
                new DeterministicAutomata<MethodInfo>(new ToStringComparer<MethodInfo>());

            result.Alphabet.AddRange(alphabet);

            foreach (KeyValuePair<Type, ICollection<MethodInfo>> typeToMethod in typeToMethods)
            {
                IExtendableDeterministicAutomata<MethodInfo> currentTypeAutomata =
                    typeToAutomata[typeToMethod.Key];

                foreach (var method in typeToMethod.Value)
                {
                    // Yuck cast...
                    ((IExtendableAutomataState<MethodInfo>)currentTypeAutomata.Root)
                        .Add(method,
                             typeToAutomata[method.ReturnType].Root);
                }

                result.AddRange(currentTypeAutomata);
            }

            return result.Intersect(aggregatedAutomata);
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Gathers a mapping between each type to its available methods.
        /// </summary>
        /// <returns>A mapping between each type to its available methods.</returns>
        /// <remarks>This method exists because we consider also the
        /// <see cref="InheritedAttribute"/>.</remarks>
        private IDictionary<Type, ICollection<MethodInfo>> GatherTypeToMethods()
        {
            IDictionary<Type, ICollection<MethodInfo>> result =
                new Dictionary<Type, ICollection<MethodInfo>>(new ToStringComparer<Type>());

            GatherTypeToMethods(this.Type, new List<MethodInfo>(), result);

            return result;
        }

        /// <summary>
        /// Inner implementation of <see cref="GatherTypeToMethods()"/>.
        /// </summary>
        /// <param name="currentType">The current type to scan for methods.</param>
        /// <param name="gatheredMethods">The methods inherited from parent.</param>
        /// <param name="typeToMethods">The mapping to fill.</param>
        private void GatherTypeToMethods(Type currentType, ICollection<MethodInfo> gatheredMethods, IDictionary<Type, ICollection<MethodInfo>> typeToMethods)
        {
            ICollection<MethodInfo> methodsForChildren = gatheredMethods;

            List<MethodInfo> currentMethods =
                gatheredMethods.Union(currentType.GetMethods(),
                                      new ToStringComparer<MethodInfo>()).ToList();

            typeToMethods[currentType] = currentMethods;

            if (currentType.HasAttribute<InheritedAttribute>())
            {
                methodsForChildren = currentMethods;
            }

            foreach (var method in currentType.GetMethods())
            {
                if (!typeToMethods.ContainsKey(method.ReturnType))
                {
                    GatherTypeToMethods(method.ReturnType,
                                        methodsForChildren,
                                        typeToMethods);
                }
            }
        }

        #endregion
    }
}