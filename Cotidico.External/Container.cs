using System;
using System.Linq;

namespace Cotidico.External
{
    public class Container
    {
        public static TType Resolve<TType>()
        {
            var type = typeof(TType);
            var constructionType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => typeof(IFactory).IsAssignableFrom(p))
                .FirstOrDefault(t => t.GetMethod("Create")?.ReturnType == type);
            if (constructionType != null)
            {
                var result = constructionType.GetMethod("Create")?.Invoke(null, null);
                return result is TType type1 ? type1 : default;
            }
            else
            {
                return default;
            }
        }
    }
}