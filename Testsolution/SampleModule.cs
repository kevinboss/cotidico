using Cotidico.External;
using Testsolution.SampleAStuff;
using Testsolution.SampleBStuff;

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