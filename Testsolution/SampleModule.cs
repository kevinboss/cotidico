using Nodelium.External;

namespace Testsolution
{
    public class SampleModule : Module
    {
        protected override void Load()
        {
            Register<SampleB, ISampleB>();
            Register<SampleA, ISampleA>();
        }
    }
}