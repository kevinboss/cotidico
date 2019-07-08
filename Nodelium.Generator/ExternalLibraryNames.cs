namespace Nodelium.Generator
{
    public static class ExternalLibraryNames
    {
        public static class Module
        {
            public static string FullName => typeof(Nodelium.External.Module).FullName;

            public static class Load
            {
                public static string FullName => Module.FullName + "." + nameof(External.Module.Load);
                public static string Name => nameof(External.Module.Load);
            }
            public static class Register
            {
                public static string FullName => Module.FullName + "." + nameof(External.Module.Register);
            }
        }
    }
}