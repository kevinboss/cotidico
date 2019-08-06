using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Cotidico.Generator
{
    public class Program
    {
        private readonly EnvironmentPreparer _environmentPreparer;
        private readonly Analyzer.Analyzer _analyzer;
        private readonly ConstructionPlanner.ConstructionPlanner _constructionPlanner;
        private readonly Writer.FileWriter _fileWriter;

        private Program()
        {
            _environmentPreparer = new EnvironmentPreparer();
            _analyzer = new Analyzer.Analyzer();
            _constructionPlanner = new ConstructionPlanner.ConstructionPlanner();
            _fileWriter = new Writer.FileWriter();
        }

        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption();
            var solutionPath = app.Option("-sln|--solution <SOLUTION>",
                    "Solution",
                    CommandOptionType.SingleValue)
                .IsRequired()
                .Accepts(v => v.ExistingFile());

            app.OnExecute(async () =>
            {
                await new Program().Run(solutionPath.Value());

                return 0;
            });

            return app.Execute(args);
        }

        private async Task Run(string solutionPath)
        {
            _environmentPreparer.RegisterMsBuildInstance();
            var analyzerResult = await _analyzer.AnalyzeSolution(solutionPath);
            var constructionPlan = _constructionPlanner.CreatePlanFromAnalysis(analyzerResult);
            await _fileWriter.Write(constructionPlan, solutionPath);
        }
    }
}