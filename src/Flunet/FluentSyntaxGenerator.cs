using System;
using System.CodeDom;
using System.Reflection;
using Flunet.Automata.Interfaces;
using Flunet.Automata.Language;
using Flunet.CodeGeneration;
using Flunet.TypeAnalyzer;

namespace Flunet
{
    /// <summary>
    /// Generates a fluent syntax from a given type.
    /// </summary>
    public static class FluentSyntaxGenerator
    {
        #region Nested Types

        /// <summary>
        /// Provides parameters for the <see cref="FluentSyntaxGenerator"/>.
        /// </summary>
        public class GenerateParams
        {
            #region Members

            private readonly string mNamespaceName;
            private readonly string mSyntaxTypeName;

            #endregion

            /// <summary>
            /// Creates a new instance of <see cref="GenerateParams"/>.
            /// </summary>
            /// <param name="namespaceName">The output fluent syntax's namespace.</param>
            /// <param name="syntaxTypeName">The output fluent syntax's 
            /// class's name.</param>
            public GenerateParams(string namespaceName, string syntaxTypeName)
            {
                mNamespaceName = namespaceName;
                mSyntaxTypeName = syntaxTypeName;
            }

            /// <summary>
            /// Gets the output fluent syntax's namespace.
            /// </summary>
            public string NamespaceName
            {
                get
                {
                    return mNamespaceName;
                }
            }

            /// <summary>
            /// Gets the output fluent syntax's class's name.
            /// </summary>
            public string SyntaxTypeName
            {
                get
                {
                    return mSyntaxTypeName;
                }
            }
        }

        #endregion

        /// <summary>
        /// Generates a fluent syntax from a given type.
        /// </summary>
        /// <param name="root">The type to analyze for fluent syntax
        /// creation.</param>
        /// <param name="generateParams">Other parameters relevant for
        /// fluent syntax generation output.</param>
        /// <returns></returns>
        public static CodeNamespace Generate(Type root, GenerateParams generateParams)
        {
            SyntaxAutomataBuilder syntaxAutomataBuilder =
                new SyntaxAutomataBuilder(root);

            IDeterministicAutomata<MethodInfo> syntaxAutomata =
                syntaxAutomataBuilder.Build
                    (FundamentalAutomatas.True("Root",
                                               new ToStringComparer<MethodInfo>()));

            AutomataNamespaceBuilder automataNamespaceBuilder =
                new AutomataNamespaceBuilder(syntaxAutomata);

            CodeNamespace result =
                automataNamespaceBuilder.Build(generateParams.NamespaceName,
                                               generateParams.SyntaxTypeName);

            return result;
        }
    }
}
