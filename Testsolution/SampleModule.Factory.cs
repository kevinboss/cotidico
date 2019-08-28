namespace Testsolution
{
    using Cotidico.External;
    using System;

    public class Testsolution_SampleBStuff_ISampleB : IFactory
    {
        public static Testsolution.SampleBStuff.ISampleB Create()
        {
            return new Testsolution.SampleBStuff.SampleB(Testsolution.Testsolution_SampleAStuff_ISampleA.Create());
        }

        public static Type GetModuleType()
        {
            return typeof(Testsolution.SampleModule);
        }
    }

    public class Testsolution_SampleAStuff_ISampleA : IFactory
    {
        public static Testsolution.SampleAStuff.ISampleA Create()
        {
            return new Testsolution.SampleAStuff.SampleA();
        }

        public static Type GetModuleType()
        {
            return typeof(Testsolution.SampleModule);
        }
    }
}