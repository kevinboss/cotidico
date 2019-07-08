using Microsoft.CodeAnalysis;

namespace Nodelium.Generator.Extensions
{
    public static class NamedTypeSymbolExtensions
    {
        public static bool InheritsFrom<T>(this INamedTypeSymbol symbol)
        {
            var baseType = symbol.BaseType;
            return
                baseType.TypeKind == TypeKind.Class
                && baseType.GetFullMetadataName() == typeof(T).FullName;
        }
    }
}