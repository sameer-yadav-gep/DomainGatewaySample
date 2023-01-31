using Leo.Subtypes.Flows;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Leo.Subtypes.Nuget.Builder
{
    // This class is using Roslyn to build flows and emit dll | @JG
    public static class SubTypeBuilder
    {
        public static EmitResult Build(string projectDirectoryLocation, string version, string destinationdirectory)
        {
            DirectoryInfo projectDirectory = new DirectoryInfo(projectDirectoryLocation);

            string[] sourceFiles = projectDirectory.EnumerateFiles("*.cs", SearchOption.AllDirectories)
                .Select(a => a.FullName).ToArray();

            List<SyntaxTree> trees = new List<SyntaxTree>();
            foreach (string file in sourceFiles)
            {
                string code = File.ReadAllText(file);
                SyntaxTree tree = CSharpSyntaxTree.ParseText(code, path: System.IO.Path.GetFileName(file));

                trees.Add(tree);
            }

            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(projectDirectory.Name)
                .WithOptions(options)
                .AddReferences(GetReferences(projectDirectoryLocation).ToArray())
                .AddSyntaxTrees(trees);

            string distinationDirectoryPath = Path.Combine(destinationdirectory, projectDirectory.Name, version);
            Directory.CreateDirectory(distinationDirectoryPath);
            string distationDllLocation = Path.Combine(distinationDirectoryPath, projectDirectory.Name + ".dll");
            var result = compilation.Emit(distationDllLocation);

            if (result.Success)
            {
                string referenceLocation = Path.Combine(projectDirectoryLocation, "References");
                var depenecyFiles = Directory.GetFiles(referenceLocation);
                if (depenecyFiles.Length != 0)
                {
                    string destinationReferenceLocation = Path.Combine(distinationDirectoryPath, "References");
                    Directory.CreateDirectory(destinationReferenceLocation);
                    foreach (var file in depenecyFiles)
                    {
                        File.Copy(file, Path.Combine(destinationReferenceLocation, System.IO.Path.GetFileName(file)));
                    }
                }
            }
            else
            {
                Directory.Delete(distinationDirectoryPath, true);
            }


            return result;
        }



        private static IEnumerable<MetadataReference> GetReferences(string projectDirectoryLocation)
        {
            List<MetadataReference> metadataReference = new List<MetadataReference>();
            // IFlowRegistry cashed 
            metadataReference.Add(MetadataReference.CreateFromFile(typeof(IFlowRegistry).Assembly.Location));
            metadataReference.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));

            //The location of the .NET assemblies
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            metadataReference.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")));
            metadataReference.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")));
            metadataReference.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")));
            metadataReference.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")));

            // Load the flow local dependencies from folder named refrences
            string referenceLocation = Path.Combine(projectDirectoryLocation, "References");

            if (Directory.Exists(referenceLocation))
            {
                DirectoryInfo referenceDirectory = new DirectoryInfo(referenceLocation);
                string[] projectDepenecies = referenceDirectory.EnumerateFiles("*.dll", SearchOption.AllDirectories)
                        .Select(a => a.FullName).ToArray();
                foreach (var projectDepency in projectDepenecies)
                    metadataReference.Add(MetadataReference.CreateFromFile(projectDepency));
            }

            return metadataReference;
        }

    }
}
