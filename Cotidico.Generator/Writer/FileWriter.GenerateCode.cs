using System.Collections.Generic;
using System.Text;
using Cotidico.External;
using Cotidico.Generator.ConstructionPlanner.ConstructionPlan;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cotidico.Generator.Writer
{
    public class CodeGenerator
    {
        private const string ConstructionMethodName = "Create";
        
        public string Generate(string factoryFileNameSpace, IReadOnlyList<FactoryInfo> factoryFileFactories)
        {
            var @namespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName(factoryFileNameSpace))
                .NormalizeWhitespace();
            @namespace =
                @namespace.AddUsings(
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName(typeof(IFactory).Namespace)),
                    SyntaxFactory.UsingDirective(SyntaxFactory.ParseName("System")));
            foreach (var factory in factoryFileFactories)
            {
                var classDeclaration = SyntaxFactory.ClassDeclaration(factory.FactoryClassName);
                classDeclaration = classDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword));

                classDeclaration =
                    classDeclaration.AddBaseListTypes(
                        SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(nameof(IFactory))));

                var constructionMethodDeclaration = CreateConstructionMethodDeclaration(factory);

                var moduleTypeMethodDeclaration = CreateModuleTypeMethodDeclaration(factory);

                classDeclaration =
                    classDeclaration.AddMembers(constructionMethodDeclaration, moduleTypeMethodDeclaration);

                @namespace = @namespace.AddMembers(classDeclaration);
            }

            var code = @namespace
                .NormalizeWhitespace()
                .ToFullString();
            return code;
        }

        private static MethodDeclarationSyntax CreateConstructionMethodDeclaration(FactoryInfo factory)
        {
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
            return constructionMethodDeclaration;
        }

        private static MethodDeclarationSyntax CreateModuleTypeMethodDeclaration(FactoryInfo factory)
        {
            var moduleTypeMethodDeclaration = SyntaxFactory
                .MethodDeclaration(SyntaxFactory.ParseTypeName("Type"), "GetModuleType")
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                    SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                .WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement(
                    $"return typeof({factory.ModuleFullName});")));
            return moduleTypeMethodDeclaration;
        }
    }
}