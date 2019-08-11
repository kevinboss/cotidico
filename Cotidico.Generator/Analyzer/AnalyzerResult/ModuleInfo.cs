using System.Collections.Generic;

namespace Cotidico.Generator.Analyzer.AnalyzerResult
{
    public class ModuleInfo
    {
        public string FullName { get; private set; }

        public IEnumerable<MappingInfo> MappingInfos { get; private set; }

        private ModuleInfo()
        {
        }

        public static ModuleInfo Create(string fullName, IEnumerable<MappingInfo> mappingInfos)
        {
            return new ModuleInfo
            {
                FullName = fullName,
                MappingInfos = mappingInfos
            };
        }
    }
}