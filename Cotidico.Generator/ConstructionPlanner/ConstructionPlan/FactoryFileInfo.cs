using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Cotidico.Generator.ConstructionPlanner.ConstructionPlan
{
    public class FactoryFileInfo
    {
        private readonly IList<FactoryInfo> _factories = new List<FactoryInfo>();

        public IReadOnlyList<FactoryInfo> Factories => _factories.ToList();

        public string FilePath { get; private set; }

        public string NameSpace { get; private set; }

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
            _factories.Add(factory);
        }
    }
}