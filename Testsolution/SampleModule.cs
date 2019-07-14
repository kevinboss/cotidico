using Nodelium.External;

namespace Testsolution
{
    public class SampleModule : Module
    {
        public override void Load()
        {
            Register<SampleB, ISampleB>();
            Register<SampleA, ISampleA>();
        }
    }
}