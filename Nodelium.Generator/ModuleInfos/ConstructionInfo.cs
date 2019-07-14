using System.Collections.Generic;

namespace Nodelium.Generator.ModuleInfos
{
    public class ConstructionInfo
    {
        public IList<string> Parameters { get; }

        private ConstructionInfo(IList<string> parameters)
        {
            Parameters = parameters;
        }

        public static ConstructionInfo Create(IList<string> parameters)
        {
            return new ConstructionInfo(parameters);
        }
    }
}