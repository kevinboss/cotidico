using Testsolution.SampleAStuff;

namespace Testsolution.SampleBStuff
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