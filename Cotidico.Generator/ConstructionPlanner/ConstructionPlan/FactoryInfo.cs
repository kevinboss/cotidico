using System.Collections.Generic;

namespace Cotidico.Generator.ConstructionPlanner.ConstructionPlan
{
    public class FactoryInfo
    {
        public string FactoryClassName { get; set; }

        public string ClassToConstruct { get; set; }

        public List<FactoryAccessInfo> ParameterFactoryClassNames { get; set; }

        private FactoryInfo()
        {
        }

        public static FactoryInfo Create(string factoryClassName, string classToConstruct,
            List<FactoryAccessInfo> parameterFactoryClassNames)
        {
            return new FactoryInfo
            {
                FactoryClassName = factoryClassName,
                ClassToConstruct = classToConstruct,
                ParameterFactoryClassNames = parameterFactoryClassNames
            };
        }
    }
}