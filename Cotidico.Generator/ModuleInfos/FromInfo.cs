using System.Collections.Generic;

namespace Cotidico.Generator.ModuleInfos
{
    public class FromInfo
    {
        public string FullMetadataName { get; }
        public IList<ConstructionInfo> ConstructionInfos { get; }

        private FromInfo(string fullMetadataName, IList<ConstructionInfo> constructionInfos)
        {
            FullMetadataName = fullMetadataName;
            ConstructionInfos = constructionInfos;
        }

        public static FromInfo Create(string fullMetadataName, IList<ConstructionInfo> constructionInfos)
        {
            return new FromInfo(fullMetadataName, constructionInfos);
        }
    }
}