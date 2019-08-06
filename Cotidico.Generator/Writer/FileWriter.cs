using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Cotidico.External;
using Cotidico.Generator.ConstructionPlanner.ConstructionPlan;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cotidico.Generator.Writer
{
    public class FileWriter
    {
        private const string ConstructionMethodName = "Create";

        public async Task Write(ConstructionPlanInfo constructionPlan, string solutionPath)
        {
            var filesToWrite = new List<(string code, string filePath)>();
            foreach (var factoryFile in constructionPlan.FactoryFiles)
            {
                var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(factoryFile.NameSpace))
                    .NormalizeWhitespace();
                @namespace =
                    @namespace.AddUsings(
                        SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(IFactory).Namespace)));
                foreach (var factory in factoryFile.factories)
                {
                    var classDeclaration = SyntaxFactory.ClassDeclaration(factory.FactoryClassName);
                    classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                    classDeclaration =
                        classDeclaration.AddBaseListTypes(
                            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(nameof(IFactory))));

                    var constructionMethodCallBuilder = new StringBuilder($"return new {factory.ClassToConstruct}(");

                    var needsSeparator = false;
                    foreach (var factoryAccess in factory.ParameterFactoryClassNames)
                    {
                        if (needsSeparator)
                        {
                            constructionMethodCallBuilder.Append(", ");
                        }
                        else
                        {
                            needsSeparator = true;
                        }

                        var dependencyCall =
                            $"{factoryAccess.DependencyNameSpace}." +
                            $"{factoryAccess.CreateFactoryClassName}." +
                            $"{ConstructionMethodName}()";
                        constructionMethodCallBuilder.Append(dependencyCall);
                    }

                    constructionMethodCallBuilder.Append(");");

                    var syntax = SyntaxFactory.ParseStatement(constructionMethodCallBuilder.ToString());
                    var constructionMethodDeclaration = SyntaxFactory
                        .MethodDeclaration(SyntaxFactory.ParseTypeName(factory.ReturnType), ConstructionMethodName)
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                            SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                        .WithBody(SyntaxFactory.Block(syntax));

                    classDeclaration = classDeclaration.AddMembers(constructionMethodDeclaration);

                    @namespace = @namespace.AddMembers(classDeclaration);
                }

                var code = @namespace
                    .NormalizeWhitespace()
                    .ToFullString();

                filesToWrite.Add((code, factoryFile.FilePath));
            }

            foreach (var (code, filePath) in filesToWrite)
            {
                using (var outputFile = new StreamWriter(filePath))
                {
                    await outputFile.WriteAsync(code);
                }
            }
        }
    }
}