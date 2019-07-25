using System.Collections.Generic;

namespace Cotidico.Generator.AnalyzerResult
{
    public class ProjectInfo
    {
        public string ProjectPath { get; }
        public IReadOnlyList<DocumentInfo> DocumentInfos { get; }

        private ProjectInfo(string projectPath, IReadOnlyList<DocumentInfo> documentInfos)
        {
            ProjectPath = projectPath;
            DocumentInfos = documentInfos;
        }

        public static ProjectInfo Create(string projectPath, IReadOnlyList<DocumentInfo> documentInfos)
        {
            return new ProjectInfo(projectPath, documentInfos);
        }
    }
}