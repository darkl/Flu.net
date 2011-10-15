using System.Collections.Generic;
using System.Linq;
using Flunet.Extensions;
using System.Reflection;
using System.CodeDom;
using Flunet.Automata.Interfaces;

namespace Flunet.CodeGeneration
{
    /// <summary>
    /// Builds an interface that represents a given <see cref="IAutomataState{T}"/>
    /// of <see cref="MethodInfo"/>.
    /// </summary>
    public class StateInterfaceBuilder
    {
        #region Members

        private readonly IAutomataState<MethodInfo> mState;
        private readonly IEnumerable<MethodInfo> mAlphabet;
        private readonly IDictionary<IAutomataState<MethodInfo>, CodeTypeDeclaration> mStateToType;
        private readonly CodeTypeDeclaration mStateTypeDeclaration;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new <see cref="StateInterfaceBuilder"/> given
        /// a state, the alphabet of the automata and a mapping
        /// between the states to their <see cref="CodeTypeDeclaration"/>.
        /// </summary>
        /// <param name="state">The given state.</param>
        /// <param name="alphabet">The given automata's alphabet.</param>
        /// <param name="stateToType">The mapping between the automata's
        /// states to their <see cref="CodeTypeDeclaration"/>s.</param>
        public StateInterfaceBuilder(IAutomataState<MethodInfo> state,
                                     IEnumerable<MethodInfo> alphabet,
                                     IDictionary<IAutomataState<MethodInfo>, CodeTypeDeclaration> stateToType)
        {
            mState = state;
            mAlphabet = alphabet;
            mStateToType = stateToType;
            mStateTypeDeclaration = mStateToType[mState];
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Builds the interface representing the given
        /// <see cref="IAutomataState{T}"/>.
        /// </summary>
        public void Build()
        {
            var inputToStates =
                mAlphabet.Select
                    (x => new KeyValuePair<MethodInfo, IAutomataState<MethodInfo>>(x, mState.Transit(x)))
                    .Where(x => x.Value.IsValid).ToArray();

            mStateTypeDeclaration.Members.AddRange
                (inputToStates.Select(x => WriteMethod(x.Key, mStateToType[x.Value])).ToArray());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Returns a <see cref="CodeMemberMethod"/> representing the
        /// given <see cref="MethodInfo"/> and considering the
        /// <see cref="resultType"/>.
        /// </summary>
        /// <param name="methodInfo">The given <see cref="MethodInfo"/>.</param>
        /// <param name="resultType">The <see cref="CodeTypeDeclaration"/> representing
        /// the state that was transited by the given method.</param>
        /// <returns>A <see cref="CodeMemberMethod"/> representing the transition.</returns>
        private static CodeMemberMethod WriteMethod(MethodInfo methodInfo, CodeTypeDeclaration resultType)
        {
            var result = methodInfo.ToMemberMethod();

            CodeTypeReference resultReference =
                new CodeTypeReference(resultType.Name);

            if (resultType.TypeParameters.Count > 0)
            {
                // I ain't a wizard.
                // I guess that all generic parameters the
                // result needs are required by the method/class.
                // and have the same names..
                var typeParameters =
                    resultType.TypeParameters.Cast<CodeTypeParameter>()
                    .Select(x => new CodeTypeReference(x.Name)).ToArray();

                resultReference.TypeArguments.AddRange(typeParameters);
            }

            result.ReturnType = resultReference;

            return result;
        }

        #endregion
    }
}
