namespace Cotidico.Generator.AnalyzerResult
{
    public class ToInfo
    {
        public string FullMetadataName { get; }

        private ToInfo(string fullMetadataName)
        {
            FullMetadataName = fullMetadataName;
        }

        public static ToInfo Create(string fullMetadataName)
        {
            return new ToInfo(fullMetadataName);
        }
    }
}