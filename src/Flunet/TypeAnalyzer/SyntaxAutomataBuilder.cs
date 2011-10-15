using System;
using System.Collections.Generic;
using System.Reflection;
using Flunet.Automata.Interfaces;

namespace Flunet.TypeAnalyzer
{
    /// <summary>
    /// Builds an automata that represents the syntax declared
    /// by the given type.
    /// </summary>
    public class SyntaxAutomataBuilder : ITypeAutomataBuilder
    {
        #region Members

        private readonly Type mType;
        private readonly ICollection<ITypeAutomataBuilder> mBuilders;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="SyntaxAutomataBuilder"/>
        /// given the type to build the automata in respect to.
        /// </summary>
        /// <param name="type">The type to build the automata
        /// in respect to.</param>
        public SyntaxAutomataBuilder(Type type)
        {
            mType = type;

            mBuilders =
                new List<ITypeAutomataBuilder>
                    {
                        new LegalSyntaxAutomataBuilder(type),
                        new UniquenessScopeValidationAutomataBuilder(type),
                        new MandatoryValidationAutomataBuilder(type)
                    };
        }

        #endregion

        #region Methods

        /// <summary>
        /// <see cref="ITypeAutomataBuilder.Build"/>
        /// </summary>
        public IDeterministicAutomata<MethodInfo> Build(IDeterministicAutomata<MethodInfo> aggregatedAutomata)
        {
            IDeterministicAutomata<MethodInfo> result = aggregatedAutomata;

            foreach (ITypeAutomataBuilder builder in mBuilders)
            {
                result = builder.Build(result);
            }

            return result;
        }

        #endregion
    }
}
