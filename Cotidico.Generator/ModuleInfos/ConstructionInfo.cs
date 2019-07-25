using System.Collections.Generic;

namespace Cotidico.Generator.ModuleInfos
{
    public class ConstructionInfo
    {
        public IList<ParameterInfo> Parameters { get; }

        private ConstructionInfo(IList<ParameterInfo> parameters)
        {
            Parameters = parameters;
        }

        public static ConstructionInfo Create(IList<ParameterInfo> parameters)
        {
            return new ConstructionInfo(parameters);
        }
    }
}