namespace Cotidico.Generator.ConstructionPlanner.ConstructionPlan
{
    public class FactoryAccessInfo
    {
        public string CreateFactoryClassName { get; set; }

        public string DependencyNameSpace { get; set; }

        private FactoryAccessInfo()
        {
        }

        public static FactoryAccessInfo Create(string dependencyNameSpace, string createFactoryClassName)
        {
            return new FactoryAccessInfo
            {
                DependencyNameSpace = dependencyNameSpace,
                CreateFactoryClassName = createFactoryClassName
            };
        }
    }
}