using System.Collections.Generic;

namespace Cotidico.Generator.Analyzer.AnalyzerResult
{
    public class DocumentInfo
    {
        public string FilePath { get; private set; }
        public IEnumerable<ModuleInfo> ModuleInfos { get; private set; }
        public string NameSpace { get; private set; }

        private DocumentInfo()
        {
        }

        public static DocumentInfo Create(string filePath, string nameSpaceName, IEnumerable<ModuleInfo> moduleInfos)
        {
            return new DocumentInfo
            {
                FilePath = filePath,
                ModuleInfos = moduleInfos,
                NameSpace = nameSpaceName
            };
        }
    }
}