using Microsoft.CodeAnalysis;

namespace Nodelium.Generator.Analyzer
{
    public class MappingInfo
    {
        private MappingInfo()
        {
        }

        public static MappingInfo Create(ITypeSymbol from, ITypeSymbol to)
        {
            return new MappingInfo
            {
                From = from,
                To = to
            };
        }

        public ITypeSymbol From { get; private set; }
        public ITypeSymbol To { get; private set; }
    }
}