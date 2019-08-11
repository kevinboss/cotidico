using Cotidico.External;
using Testsolution.SampleAStuff;
using Testsolution.SampleBStuff;

namespace Testsolution
{
    class Program
    {
        static void Main(string[] args)
        {
            var container = Container.StartBuilding().AddModule(new SampleModule()).Build();
            var sampleB = container.Resolve<ISampleB>();
            var sampleA = container.Resolve<ISampleA>();
        }
    }
}
