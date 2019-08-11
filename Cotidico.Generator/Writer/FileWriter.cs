using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Cotidico.Generator.ConstructionPlanner.ConstructionPlan;

namespace Cotidico.Generator.Writer
{
    public partial class FileWriter
    {
        private readonly CodeGenerator _codeGenerator;

        public FileWriter()
        {
            _codeGenerator = new CodeGenerator();
        }

        public async Task Write(ConstructionPlanInfo constructionPlan)
        {
            var filesToWrite = constructionPlan.FactoryFiles
                .Select(factoryFile =>
                    new
                    {
                        factoryFile.FilePath,
                        code = _codeGenerator.Generate(factoryFile.NameSpace, factoryFile.Factories)
                    })
                .Select(t => (t.code, t.FilePath)).ToList();

            foreach (var (code, filePath) in filesToWrite)
            {
                using (var outputFile = new StreamWriter(filePath))
                {
                    await outputFile.WriteAsync(code);
                }
            }
        }
    }
}