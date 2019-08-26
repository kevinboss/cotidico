using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Cotidico.External;
using Cotidico.Generator.Analyzer.AnalyzerResult;
using Cotidico.Generator.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using DocumentInfo = Cotidico.Generator.Analyzer.AnalyzerResult.DocumentInfo;
using ProjectInfo = Cotidico.Generator.Analyzer.AnalyzerResult.ProjectInfo;

namespace Cotidico.Generator.Analyzer
{
    public class Analyzer
    {
        public async Task<AnalyzerResultInfo> AnalyzeSolution(string solutionPath)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceFailed += (sender, eventArgs) =>
                {
                    Console.WriteLine(eventArgs.Diagnostic.Message);
                };
                var solution = await workspace.OpenSolutionAsync(solutionPath);

                var projectInfos = new List<ProjectInfo>();
                foreach (var project in solution.Projects)
                {
                    var projectPath = project.FilePath;
                    var documentInfos = await AnalyzeProject(project);

                    projectInfos.Add(ProjectInfo.Create(projectPath, documentInfos));
                }

                return AnalyzerResultInfo.Create(projectInfos);
            }
        }

        private static async Task<IReadOnlyList<DocumentInfo>> AnalyzeProject(Project project)
        {
            var documentInfos = new List<DocumentInfo>();

            var compilation = await project.GetCompilationAsync();
            foreach (var document in project.Documents)
            {
                var syntaxTree = await document.GetSyntaxTreeAsync();
                
                var compilationUnitRoot = syntaxTree.GetCompilationUnitRoot();
                var semanticModel = compilation.GetSemanticModel(syntaxTree);

                var modulesToAnalyze = GetModulesToAnalyze(compilationUnitRoot, semanticModel);

                if (!modulesToAnalyze.Any()) continue;

                var moduleInfos = AnalyzeModules(modulesToAnalyze, semanticModel);
                var filePath = document.FilePath;
                var nameSpaceName = modulesToAnalyze.First().ContainingNamespace.Name;

                documentInfos.Add(DocumentInfo.Create(filePath, nameSpaceName, moduleInfos));
            }

            return documentInfos;
        }

        private static IEnumerable<ModuleInfo> AnalyzeModules(IEnumerable<INamedTypeSymbol> modulesToAnalyze,
            SemanticModel semanticModel)
        {
            return modulesToAnalyze
                .Select(moduleToAnalyze =>
                    new {fullName = moduleToAnalyze.GetFullMetadataName(), mappings = AnalyzeModule(semanticModel, moduleToAnalyze)})
                .Select(t => new {t.fullName, t.mappings})
                .Select(t => ModuleInfo.Create(t.fullName, t.mappings));
        }

        private static IEnumerable<MappingInfo> AnalyzeModule(SemanticModel semanticModel,
            INamespaceOrTypeSymbol moduleToAnalyze)
        {
            var loadMethods = moduleToAnalyze.GetMembers(ExternalLibraryNames.Module.Load.Name);
            return AnalyzeLoadMethods(semanticModel, loadMethods);
        }

        private static IEnumerable<MappingInfo> AnalyzeLoadMethods(SemanticModel semanticModel, ImmutableArray<ISymbol> loadMethods)
        {
            return loadMethods.Select(loadMethod => AnalyzeLoadMethod(semanticModel, loadMethod))
                .SelectMany(mappings => mappings);
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
                .Where(classSymbol => classSymbol.InheritsFrom<Module>())
                .ToList();
            return modulesToAnalyze;
        }
    }
}