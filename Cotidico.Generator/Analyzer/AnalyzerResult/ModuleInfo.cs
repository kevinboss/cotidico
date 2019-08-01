using System.Collections.Generic;

namespace Cotidico.Generator.Analyzer.AnalyzerResult
{
    public class ModuleInfo
    {
        private ModuleInfo()
        {
        }

        public static ModuleInfo Create(string className, IEnumerable<MappingInfo> mappingInfos)
        {
            return new ModuleInfo
            {
                ClassName = className,
                MappingInfos = mappingInfos
            };
        }

        public string ClassName { get; set; }
        public IEnumerable<MappingInfo> MappingInfos { get; set; }
    }
}