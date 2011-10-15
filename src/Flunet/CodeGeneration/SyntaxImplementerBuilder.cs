using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Flunet.Extensions;
using Flunet.Automata.Interfaces;

namespace Flunet.CodeGeneration
{
    /// <summary>
    /// Creates an abstract class that implements all
    /// state interfaces by calling inner abstract methods.
    /// </summary>
    public class SyntaxImplementerBuilder
    {
        #region Members

        private readonly IEnumerable<MethodInfo> mAlphabet;
        private readonly IDictionary<IAutomataState<MethodInfo>, CodeTypeDeclaration> mStateToType;

        #endregion

        #region Constructor

        /// <summary>
        /// Creates a new instance of <see cref="SyntaxImplementerBuilder"/> given the
        /// automata's alphabet and a mapping between the automata's states to
        /// their corresponding <see cref="CodeTypeDeclaration"/>s.
        /// </summary>
        /// <param name="alphabet">The automata's alphabet.</param>
        /// <param name="stateToType">The mapping between the automata's states to
        /// their corresponding <see cref="CodeTypeDeclaration"/>s.</param>
        public SyntaxImplementerBuilder(IEnumerable<MethodInfo> alphabet, IDictionary<IAutomataState<MethodInfo>, CodeTypeDeclaration> stateToType)
        {
            mAlphabet = alphabet;
            mStateToType = stateToType;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Builds the abstract class.
        /// </summary>
        /// <returns>The built <see cref="CodeTypeDeclaration"/>.</returns>
        public CodeTypeDeclaration Build()
        {
            CodeTypeDeclaration result = new CodeTypeDeclaration("SyntaxImplementer");

            // Converts the base type to type references.
            var baseTypes =
                mStateToType
                    .Values
                    .Select(x => x.ToTypeReferece())
                    .ToArray();

            result.BaseTypes.AddRange(baseTypes);

            result.TypeAttributes = TypeAttributes.Abstract | TypeAttributes.NestedAssembly;

            // Adds all base types generic parameters.
            result.TypeParameters.AddRange(
                mStateToType
                    .Values.
                    Where(x => x.TypeParameters.Count > 0)
                    .SelectMany(x =>
                                x.TypeParameters.Cast<CodeTypeParameter>().
                                    Select(y => y.Name)).Distinct().
                                    Select(y => new CodeTypeParameter(y)).ToArray());

            // Adds abstract inner methods to be implemented by the programmer.
            AddAbstractImplementation(result);

            // Adds explicit interfaces implementations that call the
            // abstract methods.
            AddInterfaceImplementation(result);

            return result;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Explicitly implements the state interfaces by calling the
        /// abstract methods.
        /// </summary>
        /// <param name="typeDeclaration">The declaration to add to interfaces
        /// implementation to it.</param>
        private void AddInterfaceImplementation(CodeTypeDeclaration typeDeclaration)
        {
            var methodsToImplement =
                mStateToType.Values.SelectMany(
                    x => x.Members.Cast<CodeMemberMethod>()
                             .Select(y => new
                                              {
                                                  Method = y,
                                                  Type = x
                                              })).ToList();

            foreach (var methodToType in methodsToImplement)
            {
                CodeMemberMethod methodImplementation = methodToType.Method.Clone();

                // There is a CodeDom bug here :(
                // if the interface has a generic type, it doesn't
                // implement it right...
                // Update: Fixed in framework 4.0 :)
                methodImplementation.PrivateImplementationType =
                    methodToType.Type.ToTypeReferece();

                CodeTypeReference[] methodTypeParameters =
                    methodToType.
                        Method.
                        TypeParameters.
                        Cast<CodeTypeParameter>().
                        Select(x => new CodeTypeReference(x)).
                        ToArray();

                methodImplementation.Statements.Add
                    (new CodeMethodReturnStatement
                         (new CodeMethodInvokeExpression
                              (new CodeMethodReferenceExpression
                                   (new CodeThisReferenceExpression(),
                                    "Inner" + methodImplementation.Name,
                                    methodTypeParameters),
                               ParametersReference(methodImplementation))));

                typeDeclaration.Members.Add(methodImplementation);
            }
        }

        /// <summary>
        /// Creates an array of <see cref="CodeVariableReferenceExpression"/>s that reference
        /// to the parameters of the given <see cref="memberMethod"/>.
        /// </summary>
        /// <param name="memberMethod">The given <see cref="memberMethod"/>.</param>
        /// <returns>An array of <see cref="CodeVariableReferenceExpression"/>s that reference
        /// to the parameters of the given <see cref="memberMethod"/>.</returns>
        private static CodeVariableReferenceExpression[] ParametersReference(CodeMemberMethod memberMethod)
        {
            return memberMethod.Parameters.Cast<CodeParameterDeclarationExpression>()
                .Select(x => new CodeVariableReferenceExpression(x.Name))
                .ToArray();
        }


        /// <summary>
        /// Adds abstract inner methods for all methods in the alphabet.
        /// </summary>
        /// <param name="typeDeclaration">The <see cref="CodeTypeDeclaration"/>
        /// to add the abstract methods to.</param>
        private void AddAbstractImplementation(CodeTypeDeclaration typeDeclaration)
        {
            foreach (MethodInfo methodInfo in mAlphabet)
            {
                CodeMemberMethod memberMethod = methodInfo.ToMemberMethod();

                memberMethod.Name = "Inner" + memberMethod.Name;

                memberMethod.ReturnType = typeDeclaration.ToTypeReferece();

                memberMethod.Attributes = MemberAttributes.Abstract | MemberAttributes.Family;

                typeDeclaration.Members.Add(memberMethod);
            }
        }

        #endregion
    }
}