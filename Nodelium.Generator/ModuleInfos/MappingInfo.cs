namespace Nodelium.Generator.ModuleInfos
{
    public class MappingInfo
    {
        private MappingInfo()
        {
        }

        public static MappingInfo Create(FromInfo @from, ToInfo to)
        {
            return new MappingInfo
            {
                From = @from,
                To = to
            };
        }

        public FromInfo From { get; private set; }
        public ToInfo To { get; private set; }
    }
}