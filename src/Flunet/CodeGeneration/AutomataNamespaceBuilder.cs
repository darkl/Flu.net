using System;
using System.Collections.Generic;
using System.Linq;
using System.CodeDom;
using System.Reflection;
using Flunet.Extensions;
using Flunet.Automata.Interfaces;
using Flunet.TypeAnalyzer;

namespace Flunet.CodeGeneration
{
    /// <summary>
    /// Builds a <see cref="CodeNamespace"/> representing a
    /// <see cref="IDeterministicAutomata{T}"/> of <see cref="MethodInfo"/>.
    /// </summary>
    public class AutomataNamespaceBuilder
    {
        #region Members

        private readonly IDeterministicAutomata<MethodInfo> mAutomata;
        private readonly IEnumerable<MethodInfo> mAlphabet;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="AutomataNamespaceBuilder"/> given 
        /// a <see cref="IDeterministicAutomata{T}"/> of <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="automata">The given <see cref="IDeterministicAutomata{T}"/>.</param>
        public AutomataNamespaceBuilder(IDeterministicAutomata<MethodInfo> automata)
        {
            mAutomata = automata;
            mAlphabet = automata.Alphabet;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Builds the <see cref="CodeNamespace"/> that represents
        /// the given <see cref="IDeterministicAutomata{T}"/> of
        /// <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="namespaceName">The given namespace's name.</param>
        /// <param name="syntaxTypeName"></param>
        /// <returns>The <see cref="CodeNamespace"/> that represents
        /// the given <see cref="IDeterministicAutomata{T}"/> of
        /// <see cref="MethodInfo"/>.</returns>
        public CodeNamespace Build(string namespaceName, string syntaxTypeName)
        {
            CodeNamespace result = new CodeNamespace(namespaceName);
            
            CodeTypeDeclaration syntaxClass =
                new CodeTypeDeclaration
                    {
                        Name = syntaxTypeName,
                        IsPartial = true,
                        Attributes = MemberAttributes.Public
                    };

            result.Types.Add(syntaxClass);

            var stateToType = new Dictionary<IAutomataState<MethodInfo>, CodeTypeDeclaration>();

            var statesToGenericParameters =
                GatherStatesToGenericParameters();

            foreach (IAutomataState<MethodInfo> state in mAutomata)
            {
                CodeTypeDeclaration currentType = 
                    CreateTypeDeclaration(state, statesToGenericParameters[state]);

                stateToType.Add(state, currentType);
                syntaxClass.Members.Add(currentType);
            }

            foreach (IAutomataState<MethodInfo> state in mAutomata)
            {
                StateInterfaceBuilder stateBuilder =
                    new StateInterfaceBuilder(state, mAlphabet, stateToType);

                stateBuilder.Build();
            }

            SyntaxImplementerBuilder builder =
                new SyntaxImplementerBuilder(mAlphabet, stateToType);

            syntaxClass.Members.Add(builder.Build());

            return result;
        }

        #endregion

        #region Private Members

        /// <summary>
        /// Creates a mapping between the states to their generic types.
        /// </summary>
        /// <returns>The dictionary that forms a mapping between the states to their generic types</returns>
        private IDictionary<IAutomataState<MethodInfo>, ICollection<Type>> GatherStatesToGenericParameters()
        {
            IDictionary<IAutomataState<MethodInfo>, ICollection<Type>> result =
                new Dictionary<IAutomataState<MethodInfo>, ICollection<Type>>();

            foreach (IAutomataState<MethodInfo> state in mAutomata)
            {
                var typeGenericParameters =
                    mAlphabet.Where(x => !x.IsGenericMethod && state.Transit(x).IsValid)
                        .SelectMany(x => x.GetParameters()
                                             .Where(y => y.ParameterType.IsGenericParameter))
                        .Select(x => x.ParameterType)
                        .Distinct(new ToStringComparer<Type>()).ToList();

                result[state] = typeGenericParameters;
            }

            FixGenericTypes(result);

            return result;
        }

        /// <summary>
        /// Finds generic types dependencies between states.
        /// </summary>
        /// <param name="result">The initial generic types dependencies</param>
        private void FixGenericTypes(IDictionary<IAutomataState<MethodInfo>, ICollection<Type>> result)
        {
            int? currentGenericTypes = null;
            int? previousGenericTypes;

            // TODO: This part sucks, feel free to rewrite it.
            do
            {
                foreach (IAutomataState<MethodInfo> state in mAutomata)
                {
                    // Finds all missing generic types returned by
                    // non generic methods.
                    var nonGenericMethodsTypeGenericParameters =
                        (from method in mAlphabet
                         where !method.IsGenericMethod
                         let transitedState = state.Transit(method)
                         where transitedState.IsValid
                         from genericType in result[transitedState]
                         select genericType);

                    // Finds all missing generic types returned by
                    // generic methods.
                    var genericMethodsTypeGenericParameters =
                        from method in mAlphabet
                        where method.IsGenericMethod
                        let transitedState = state.Transit(method)
                        where transitedState.IsValid
                        from genericType in
                            result[transitedState].Except(method.GetGenericArguments(),
                                                          new ToStringComparer<Type>())
                        select genericType;

                    result[state] =
                        nonGenericMethodsTypeGenericParameters
                            .Union(genericMethodsTypeGenericParameters)
                            .Union(result[state], new ToStringComparer<Type>()).ToList();
                }

                previousGenericTypes = currentGenericTypes;
                currentGenericTypes = result.SelectMany(x => x.Value).Count();
            }
            while (previousGenericTypes != currentGenericTypes);
        }

        /// <summary>
        /// Creates an empty <see cref="CodeTypeDeclaration"/> representing a given
        /// <see cref="IAutomataState{T}"/> of <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="state">The give state.</param>
        /// <param name="typeGenericParameters">The generic types of the
        /// interface.</param>
        /// <returns>An empty <see cref="CodeTypeDeclaration"/> representing the given
        /// <see cref="IAutomataState{T}"/>.</returns>
        private CodeTypeDeclaration CreateTypeDeclaration(IAutomataState<MethodInfo> state,
                                                          IEnumerable<Type> typeGenericParameters)
        {
            // Finds all the methods in the alphabet that
            // are generic and transit the current state to a valid state
            // These methods declare the generic types of the state.
            CodeTypeDeclaration result =
                new CodeTypeDeclaration("IState" + state.Id);

            result.IsInterface = true;

            result.TypeParameters.AddRange
                (typeGenericParameters.Select(x => x.ToTypeParameter()).ToArray());

            return result;
        }

        #endregion
    }
}