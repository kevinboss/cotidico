using System.Collections.Generic;

namespace Cotidico.Generator.AnalyzerResult
{
    public class DocumentInfo
    {
        public string FilePath { get; }
        public IEnumerable<ModuleInfo> ModuleInfos { get; }

        private DocumentInfo(string filePath, IEnumerable<ModuleInfo> moduleInfos)
        {
            FilePath = filePath;
            ModuleInfos = moduleInfos;
        }

        public static DocumentInfo Create(string filePath, IEnumerable<ModuleInfo> moduleInfos)
        {
            return new DocumentInfo(filePath, moduleInfos);
        }
    }
}