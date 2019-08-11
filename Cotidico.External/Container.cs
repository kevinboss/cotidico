using System;
using System.Collections.Generic;
using System.Linq;

namespace Cotidico.External
{
    public class Container
    {
        private const string ConstructionMethodName = "Create";
        private readonly IList<Module> _modules = new List<Module>();
        private IEnumerable<Type> _registeredFactories;

        private Container()
        {
        }

        public static ContainerBuilder StartBuilding()
        {
            return new ContainerBuilder(new Container());
        }

        public TType Resolve<TType>()
        {
            var type = typeof(TType);
            var constructionType = GetFactories()
                .SingleOrDefault(t => t.GetMethod(ConstructionMethodName)?.ReturnType == type);

            if (constructionType == null) return default;

            var result = constructionType.GetMethod(ConstructionMethodName)?.Invoke(null, null);
            return result is TType type1 ? type1 : default;
        }

        private IEnumerable<Type> GetFactories()
        {
            var potentialFactories = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IFactory).IsAssignableFrom(p));
            return _registeredFactories ?? (_registeredFactories = potentialFactories.Where(factory =>
                       factory.GetMethod("GetModuleType")?.Invoke(null, null) is Type moduleType &&
                       _modules.Any(module => module.GetType() == moduleType)));
        }

        public class ContainerBuilder
        {
            private readonly Container _container;

            internal ContainerBuilder(Container container)
            {
                _container = container;
            }

            public ContainerBuilder AddModule(Module module)
            {
                _container._modules.Add(module);
                return this;
            }

            public Container Build()
            {
                return _container;
            }
        }
    }
}