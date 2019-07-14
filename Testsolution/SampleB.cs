namespace Testsolution
{
    class SampleB : ISampleB
    {
        private readonly ISampleA _sampleA;

        public SampleB(ISampleA sampleA)
        {
            _sampleA = sampleA;
        }
    }
}