using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Cotidico.Generator
{
    public class Program
    {
        private readonly Analyzer.Analyzer _analyzer;

        private Program()
        {
            _analyzer = new Analyzer.Analyzer();
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
            var analyzerResult = await _analyzer.AnalyzeSolution(solutionPath);
        }
    }
}