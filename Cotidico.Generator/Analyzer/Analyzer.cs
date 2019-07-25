using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cotidico.External;
using Cotidico.Generator.Extensions;
using Cotidico.Generator.ModuleInfos;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;

namespace Cotidico.Generator.Analyzer
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

                        foreach (var moduleInfo in moduleInfos)
                        {
                            foreach (var mappingInfo in moduleInfo.MappingInfos)
                            {
                            }
                        }

                        var filePath = document.FilePath;
                    }
                }
            }
        }

        private static IEnumerable<ModuleInfo> AnalyzeModules(List<INamedTypeSymbol> modulesToAnalyze,
            SemanticModel semanticModel)
        {
            foreach (var moduleToAnalyze in modulesToAnalyze)
            {
                var mappings = AnalyzeModule(semanticModel, moduleToAnalyze);
                var namespaceName = moduleToAnalyze.ContainingNamespace.Name;
                var className = moduleToAnalyze.Name;

                yield return ModuleInfo.Create(className, namespaceName, mappings);
            }
        }

        private static IEnumerable<MappingInfo> AnalyzeModule(SemanticModel semanticModel,
            INamedTypeSymbol moduleToAnalyze)
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

            var mappings = methodSymbols.Select(e => e.TypeArguments).ToList();

            foreach (var mapping in mappings)
            {
                if (!(mapping[0] is INamedTypeSymbol from))
                {
                    throw new AnalyzerException(
                        $"Implementation is invalid: {mapping[0].ToDisplayString()}");
                }

                if (!from.Constructors.Any())
                {
                    throw new AnalyzerException(
                        $"Implementation has no constructor: {mapping[0].ToDisplayString()}");
                }

                var constructionInfos = from.Constructors.Select(constructor =>
                        ConstructionInfo.Create(
                            constructor.Parameters.Select(
                                    parameter => ParameterInfo.Create(parameter.Type.GetFullMetadataName()))
                                .ToList()))
                    .ToList();
                var fromInfo = FromInfo.Create(from.GetFullMetadataName(), constructionInfos);

                var to = mapping[1];
                var toInfo = ToInfo.Create(to.GetFullMetadataName());

                yield return MappingInfo.Create(fromInfo, toInfo);
            }
        }

        private static List<INamedTypeSymbol> GetModulesToAnalyze(CompilationUnitSyntax root,
            SemanticModel semanticModel)
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