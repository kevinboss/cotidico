using System.Collections.Generic;
using System.IO;

namespace Cotidico.Generator.ConstructionPlanner.ConstructionPlan
{
    public class FactoryFileInfo
    {
        public IList<FactoryInfo> factories { get; } = new List<FactoryInfo>();
        public string FilePath { get; private set; }

        public string NameSpace { get; set; }

        private FactoryFileInfo()
        {
        }

        public static FactoryFileInfo Create(string documentFilePath, string nameSpace)
        {
            var fileInfo = new FileInfo(documentFilePath);
            var factoryFileName = $"{Path.GetFileNameWithoutExtension(fileInfo.Name)}.factory{fileInfo.Extension}";
            return new FactoryFileInfo
            {
                FilePath = Path.Combine(fileInfo.DirectoryName, factoryFileName),
                NameSpace = nameSpace
            };
        }

        public void AddFactory(FactoryInfo factory)
        {
            factories.Add(factory);
        }
    }
}