using System.Collections.Generic;
using System.IO;

namespace Cotidico.Generator.ConstructionPlanner.ConstructionPlan
{
    public class FactoryFileInfo
    {
        public IList<FactoryInfo> factories { get; } = new List<FactoryInfo>();
        public string FilePath { get; private set; }

        public string NameSpace { get; set; }

        public string ProjectPath { get; set; }

        private FactoryFileInfo()
        {
        }

        public static FactoryFileInfo Create(string projectPath, string documentFilePath, string nameSpace)
        {
            var documentFileInfo = new FileInfo(documentFilePath);
            var factoryFileName =
                $"{Path.GetFileNameWithoutExtension(documentFileInfo.Name)}.factory{documentFileInfo.Extension}";
            var factoryFileDirectoryName = documentFileInfo.DirectoryName;

            return new FactoryFileInfo
            {
                ProjectPath = projectPath,
                FilePath = Path.Combine(factoryFileDirectoryName, factoryFileName),
                NameSpace = nameSpace
            };
        }

        public void AddFactory(FactoryInfo factory)
        {
            factories.Add(factory);
        }
    }
}