namespace Cotidico.Generator.Analyzer
{
    public static class ExternalLibraryInformation
    {
        public static class Module
        {
            public static string FullName => typeof(External.Module).FullName;

            public static class Load
            {
                public static string Name => nameof(External.Module.Load);
            }
            public static class Register
            {
                public static string FullName => $"{Module.FullName}.{nameof(External.Module.Register)}";
            }
            
            public const int RegisterMethodsGenericCount = 2;
        }
    }
}