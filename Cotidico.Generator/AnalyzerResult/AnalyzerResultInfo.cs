using System.Collections.Generic;

namespace Cotidico.Generator.AnalyzerResult
{
    public class AnalyzerResultInfo
    {
        public List<ProjectInfo> ProjectInfos { get; }

        private AnalyzerResultInfo(List<ProjectInfo> projectInfos)
        {
            ProjectInfos = projectInfos;
        }

        public static AnalyzerResultInfo Create(List<ProjectInfo> projectInfos)
        {
            return new AnalyzerResultInfo(projectInfos);
        }
    }
}