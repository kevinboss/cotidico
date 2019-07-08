using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Nodelium.External;
using Nodelium.Generator.Extensions;

namespace Nodelium.Generator.Analyzer
{
    public class Analyzer
    {
        public async Task AnalyzeSolution(string solutionPath)
        {
            var msBuildInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();

            if (msBuildInstances.Length != 1)
            {
                throw new InvalidOperationException();
            }

            MSBuildLocator.RegisterInstance(msBuildInstances[0]);

            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceFailed += (sender, eventArgs) =>
                {
                    Console.WriteLine(eventArgs.Diagnostic.Message);
                };
                var solution = await workspace.OpenSolutionAsync(solutionPath);

                foreach (var project in solution.Projects)
                {
                    var compilation = await project.GetCompilationAsync();
                    foreach (var document in project.Documents)
                    {
                        var tree = await document.GetSyntaxTreeAsync();
                        var semanticModel = compilation.GetSemanticModel(tree);
                        var root = tree.GetCompilationUnitRoot();

                        var modulesToAnalyze = GetModulesToAnalyze(root, semanticModel);

                        if (!modulesToAnalyze.Any()) continue;

                        var moduleInfos = AnalyzeModules(modulesToAnalyze, semanticModel);

                        var filePath = document.FilePath;
                    }
                }
            }
        }

        private static IEnumerable<ModuleInfo> AnalyzeModules(List<INamedTypeSymbol> modulesToAnalyze, SemanticModel semanticModel)
        {
            foreach (var moduleToAnalyze in modulesToAnalyze)
            {
                var mappings = AnalyzeModule(semanticModel, moduleToAnalyze);
                var namespaceName = moduleToAnalyze.ContainingNamespace.Name;
                var className = moduleToAnalyze.Name;

                yield return ModuleInfo.Create(className, namespaceName, mappings);
            }
        }

        private static IEnumerable<MappingInfo> AnalyzeModule(SemanticModel semanticModel, INamedTypeSymbol moduleToAnalyze)
        {
            var loadMethods = moduleToAnalyze.GetMembers(ExternalLibraryNames.Module.Load.Name);
            foreach (var loadMethod in loadMethods)
            {
                var mappings = AnalyzeLoadMethod(semanticModel, loadMethod);
                foreach (var mapping in mappings)
                {
                    yield return mapping;
                }
            }
        }

        private static IEnumerable<MappingInfo> AnalyzeLoadMethod(SemanticModel semanticModel, ISymbol loadMethod)
        {
            var syntaxNode = loadMethod.DeclaringSyntaxReferences.Single().GetSyntax();
            var methodSymbols = syntaxNode.DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Select(invocationExpressionSyntax =>
                    semanticModel.GetSymbolInfo(invocationExpressionSyntax.Expression).Symbol as IMethodSymbol)
                .Where(e => e.GetFullMetadataName() == ExternalLibraryNames.Module.Register.FullName);
                
            var registerMethodsTypeArguments = methodSymbols.Select(e => e.TypeArguments).ToList();

            foreach (var typeArgument in registerMethodsTypeArguments)
            {
                yield return MappingInfo.Create(typeArgument[0], typeArgument[1]);
            }
        }

        private static List<INamedTypeSymbol> GetModulesToAnalyze(CompilationUnitSyntax root, SemanticModel semanticModel)
        {
            var modulesToAnalyze = root
                .DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .Select(classDeclarationSyntax => semanticModel.GetDeclaredSymbol(classDeclarationSyntax))
                .Where(namedTypeSymbol => namedTypeSymbol.InheritsFrom<Module>())
                .ToList();
            return modulesToAnalyze;
        }
    }
}