using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Cotidico.Generator.Extensions
{
    public static class SymbolExtensions
    {
        public static string GetFullMetadataName(this ISymbol s)
        {
            if (s == null || s.IsRootNamespace())
            {
                return string.Empty;
            }

            var result = new List<string> {s.MetadataName};
            var last = s;

            s = s.ContainingSymbol;

            while (!s.IsRootNamespace())
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
        

        private static bool IsRootNamespace(this ISymbol symbol)
        {
            INamespaceSymbol s;
            return (s = symbol as INamespaceSymbol) != null && s.IsGlobalNamespace;
        }
    }
}