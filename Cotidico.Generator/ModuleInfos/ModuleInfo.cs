using System.Collections.Generic;

namespace Cotidico.Generator.ModuleInfos
{
    public class ModuleInfo
    {
        private ModuleInfo()
        {
        }

        public static ModuleInfo Create(string className, string namespaceName, IEnumerable<MappingInfo> mappingInfos)
        {
            return new ModuleInfo
            {
                ClassName = className,
                NamespaceName = namespaceName,
                MappingInfos = mappingInfos
            };
        }

        public string ClassName { get; set; }
        public string NamespaceName { get; set; }
        public IEnumerable<MappingInfo> MappingInfos { get; set; }
    }
}