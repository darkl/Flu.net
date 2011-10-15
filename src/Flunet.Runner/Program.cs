using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using Microsoft.CSharp;

namespace Flunet.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            string assemblyName = args.GetArgument("/assembly:", "/a:");
            string type = args.GetArgument("/type:", "/t:");
            string output = args.GetArgument("/outputdir:", "/out:", "/o:");
            string namespaceName = args.GetArgument("/namespace:", "/n:");
            string className = args.GetArgument("/class:", "/c:");

            Type syntaxSkeleton = 
                Assembly.LoadFrom(assemblyName).GetType(type);

            CodeNamespace generated =
                FluentSyntaxGenerator.Generate(syntaxSkeleton,
                                               new FluentSyntaxGenerator.GenerateParams(namespaceName, className));

            CodeCompileUnit compileUnit = new CodeCompileUnit();
            compileUnit.Namespaces.Add(generated);

            CSharpCodeProvider codeProvider = new CSharpCodeProvider();

            string outputPath =
                Path.ChangeExtension(Path.Combine(output, className), ".cs");

            using (StreamWriter writer = new StreamWriter(outputPath))
            {
                codeProvider.GenerateCodeFromCompileUnit(compileUnit,
                                                         writer,
                                                         new CodeGeneratorOptions()
                                                             {
                                                                 BracingStyle = "C",
                                                                 BlankLinesBetweenMembers = true
                                                             });
            }

            Console.WriteLine("Generated successfully.");
            Console.ReadLine();
        }
    }
}
