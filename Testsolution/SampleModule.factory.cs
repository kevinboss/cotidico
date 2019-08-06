namespace Testsolution
{
    using Cotidico.External;

    public class Testsolution_ISampleB : IFactory
    {
        public static Testsolution.ISampleB Create()
        {
            return new Testsolution.SampleB(Testsolution.Testsolution_ISampleA.Create());
        }
    }

    public class Testsolution_ISampleA : IFactory
    {
        public static Testsolution.ISampleA Create()
        {
            return new Testsolution.SampleA();
        }
    }
}