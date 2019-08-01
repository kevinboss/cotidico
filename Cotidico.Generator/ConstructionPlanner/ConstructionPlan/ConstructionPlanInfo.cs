using System.Collections.Generic;

namespace Cotidico.Generator.ConstructionPlanner.ConstructionPlan
{
    public class ConstructionPlanInfo
    {
        public List<FactoryFileInfo> FactoryFiles { get; set; }

        private ConstructionPlanInfo()
        {
        }

        public static ConstructionPlanInfo Create(List<FactoryFileInfo> factoryFiles)
        {
            return new ConstructionPlanInfo
            {
                FactoryFiles = factoryFiles
            };
        }
    }
}