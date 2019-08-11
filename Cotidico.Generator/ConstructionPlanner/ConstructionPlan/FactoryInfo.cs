using System.Collections.Generic;

namespace Cotidico.Generator.ConstructionPlanner.ConstructionPlan
{
    public class FactoryInfo
    {
        public string FactoryClassName { get; private set; }
        
        public string ModuleFullName { get; set; }

        public string ClassToConstruct { get; private set; }

        public List<FactoryAccessInfo> ParameterFactoryClassNames { get; private set; }

        public string ReturnType { get; private set; }


        private FactoryInfo()
        {
        }

        public static FactoryInfo Create(string factoryClassName, string moduleFullName, string classToConstruct,
            string returnType,
            List<FactoryAccessInfo> parameterFactoryClassNames)
        {
            return new FactoryInfo
            {
                FactoryClassName = factoryClassName,
                ModuleFullName = moduleFullName,
                ClassToConstruct = classToConstruct,
                ReturnType = returnType,
                ParameterFactoryClassNames = parameterFactoryClassNames
            };
        }
    }
}