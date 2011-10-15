using System;
using System.Linq;
using System.Reflection;
using System.CodeDom;

namespace Flunet.Extensions
{
    /// <summary>
    /// Provides extension methods that constructs CodeDom
    /// objects from meta-data, and other utilites.
    /// </summary>
    public static class CodeDomExtensions
    {
        #region Conversion from MetaData

        /// <summary>
        /// Creates a <see cref="CodeTypeParameter"/> with
        /// the same name of a given <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <returns>A <see cref="CodeTypeParameter"/> with
        /// the same name of a given <see cref="Type"/>.</returns>
        public static CodeTypeParameter ToTypeParameter(this Type type)
        {
            return new CodeTypeParameter(type.Name);
        }

        /// <summary>
        /// Creates a <see cref="CodeMemberMethod"/> with the same
        /// signature of a given <see cref="MethodInfo"/>.
        /// </summary>
        /// <param name="method">The given <see cref="MethodInfo"/>.</param>
        /// <returns>A <see cref="CodeMemberMethod"/> with the same
        /// signature of a given <see cref="MethodInfo"/>.</returns>
        public static CodeMemberMethod ToMemberMethod(this MethodInfo method)
        {
            CodeMemberMethod result = new CodeMemberMethod();

            result.Name = method.Name;

            result.Parameters.AddRange(method.GetParameters().Select(x => x.ToParameterExpression()).ToArray());

            result.ReturnType = new CodeTypeReference(method.ReturnType);

            if (method.IsGenericMethod)
            {
                result.TypeParameters.AddRange(method.GetGenericArguments().Select(x => x.ToTypeParameter()).ToArray());
            }

            return result;
        }

        /// <summary>
        /// Creates a <see cref="CodeParameterDeclarationExpression"/> that
        /// describes the given <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="parameter">The given <see cref="ParameterInfo"/>.</param>
        /// <returns>A <see cref="CodeParameterDeclarationExpression"/> that
        /// describes the given <see cref="ParameterInfo"/>.</returns>
        public static CodeParameterDeclarationExpression ToParameterExpression(this ParameterInfo parameter)
        {
            CodeTypeReference parameterType = new CodeTypeReference(parameter.ParameterType);

            if (parameter.ParameterType.IsGenericParameter)
            {
                parameterType =
                    new CodeTypeReference(parameter.ParameterType.Name,
                                          CodeTypeReferenceOptions.GenericTypeParameter);
            }

            return new CodeParameterDeclarationExpression(parameterType, parameter.Name);
        }

        #endregion

        #region Other Utilites

        /// <summary>
        /// Creates a new <see cref="CodeMemberMethod"/> with the same
        /// signature as the given <see cref="CodeMemberMethod"/>.
        /// </summary>
        /// <remarks>
        /// TODO: this doesn't really clone the parameters.
        /// TODO: if anyone cares about it, feel free to fix it.
        /// </remarks>
        /// <param name="method">The given <see cref="CodeMemberMethod"/>.</param>
        /// <returns>A new <see cref="CodeMemberMethod"/> with the same
        /// signature as the given <see cref="CodeMemberMethod"/>.</returns>
        public static CodeMemberMethod Clone(this CodeMemberMethod method)
        {
            var result = new CodeMemberMethod
                             {
                                 Attributes = method.Attributes,
                                 Name = method.Name,
                                 ReturnType = method.ReturnType
                             };

            // Doesn't really clone them...
            result.Parameters.AddRange(method.Parameters);

            result.TypeParameters.AddRange(method.TypeParameters);

            return result;
        }

        /// <summary>
        /// Creates a new <see cref="CodeTypeReference"/> that references
        /// to a given <see cref="CodeTypeDeclaration"/>.
        /// </summary>
        /// <param name="type">The given <see cref="CodeTypeDeclaration"/>.</param>
        /// <returns>A new <see cref="CodeTypeReference"/> that references
        /// to a given <see cref="CodeTypeDeclaration"/>.</returns>
        public static CodeTypeReference ToTypeReferece(this CodeTypeDeclaration type)
        {
            var result = new CodeTypeReference(type.Name);

            result.TypeArguments.AddRange(
                type.TypeParameters.Cast<CodeTypeParameter>()
                    .Select(x => new CodeTypeReference(x.Name))
                    .ToArray());

            return result;
        }

        #endregion
    }
}
