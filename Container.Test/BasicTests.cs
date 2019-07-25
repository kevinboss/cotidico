using System.IO;
using Cotidico.Generator;
using NUnit.Framework;

namespace ContainerTests
{
    public class BasicTests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void Test1()
        {
            var dir = Directory.GetParent(TestContext.CurrentContext.TestDirectory)
                .Parent?.Parent?.Parent?.FullName;
            var solutionPath = Path.Combine(dir, Path.Combine("Testsolution", "Testsolution.sln"));
            var result = Program.Main(new[]
            {
                "-sln",
                solutionPath
            });
        }
    }
}