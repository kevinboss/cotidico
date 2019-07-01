using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Nodelium.External;

namespace Nodelium.Generator
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption();
            var solutionPath = app.Option("-sln|--solution <SOLUTION>",
                    "Solution",
                    CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.ExistingFile());

            app.OnExecute(async () =>
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
                    var solution = await workspace.OpenSolutionAsync(solutionPath.Value());

                    foreach (var project in solution.Projects)
                    {
                        var compilation = await project.GetCompilationAsync();
                        foreach (var document in project.Documents)
                        {
                            var tree = await document.GetSyntaxTreeAsync();
                            var semanticModel = compilation.GetSemanticModel(tree);
                            var root = tree.GetCompilationUnitRoot();

                            var result = root.DescendantNodes()
                                .OfType<ClassDeclarationSyntax>()
                                .Where(classDeclarationSyntax
                                    => classDeclarationSyntax.BaseList != null)
                                .Select(classDeclarationSyntax
                                    => semanticModel.GetDeclaredSymbol(classDeclarationSyntax))
                                .Where(InheritsFrom<IModule>)
                                .ToList();
                        }
                    }
                }

                return 0;
            });

            return app.Execute(args);
        }

        private static bool InheritsFrom<T>(INamedTypeSymbol symbol)
        {
            var interfaces = symbol.AllInterfaces.Cast<ITypeSymbol>().ToList();
            return interfaces.Any(@interface =>
                @interface.TypeKind == TypeKind.Interface
                && GetFullMetadataName(@interface) == typeof(T).FullName);
        }

        private static string GetFullMetadataName(ISymbol s)
        {
            if (s == null || IsRootNamespace(s))
            {
                return string.Empty;
            }

            var result = new List<string> {s.MetadataName};
            var last = s;

            s = s.ContainingSymbol;

            while (!IsRootNamespace(s))
            {
                if (s is ITypeSymbol && last is ITypeSymbol)
                {
                    result.Add("+");
                }
                else
                {
                    result.Add(".");
                }

                result.Add(s.OriginalDefinition.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat));
                s = s.ContainingSymbol;
            }

            result.Reverse();

            return result.Aggregate(new StringBuilder(), (builder, @string) => builder.Append(@string)).ToString();
        }

        private static bool IsRootNamespace(ISymbol symbol)
        {
            INamespaceSymbol s = null;
            return ((s = symbol as INamespaceSymbol) != null) && s.IsGlobalNamespace;
        }
    }
}