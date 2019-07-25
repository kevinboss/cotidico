namespace Cotidico.Generator.AnalyzerResult
{
    public class ParameterInfo
    {
        public string FullMetadataName { get; private set; }

        private ParameterInfo(string fullMetadataName)
        {
            FullMetadataName = fullMetadataName;
        }


        public static ParameterInfo Create(string fullMetadataName)
        {
            return new ParameterInfo(fullMetadataName);
        }
    }
}