using System.Collections.Generic;
using System.Linq;
using Cotidico.Generator.Analyzer.AnalyzerResult;
using Cotidico.Generator.ConstructionPlanner.ConstructionPlan;

namespace Cotidico.Generator.ConstructionPlanner
{
    public class ConstructionPlanner
    {
        private const string Dot = ".";
        private const string Underscore = "_";

        public ConstructionPlanInfo CreatePlanFromAnalysis(AnalyzerResultInfo analyzerResultInfo)
        {
            var factoryFiles = new List<FactoryFileInfo>();
            var knownDependencies = GetKnownDependencies(analyzerResultInfo);
            foreach (var projectInfo in analyzerResultInfo.ProjectInfos)
            {
                foreach (var documentInfo in projectInfo.DocumentInfos)
                {
                    var factoryFile = FactoryFileInfo.Create(projectInfo.ProjectPath, documentInfo.FilePath,
                        documentInfo.NameSpace);
                    foreach (var moduleInfo in documentInfo.ModuleInfos)
                    {
                        foreach (var mappingInfo in moduleInfo.MappingInfos)
                        {
                            var factoryClassName = CreateFactoryClassName(mappingInfo.To.FullMetadataName);
                            var moduleFullName = moduleInfo.FullName;
                            var classToConstruct = mappingInfo.From.FullMetadataName;
                            var returnType = mappingInfo.To.FullMetadataName;
                            var mostSatisfiedConstructionInfo =
                                GetMostSatisfiedConstructionInfo(mappingInfo, knownDependencies);
                            var parameterFactoryAccessInfos = mostSatisfiedConstructionInfo.Parameters.Select(
                                    parameterInfo =>
                                    {
                                        var (nameSpace, fullMetadataName) = knownDependencies.First(knownDependency =>
                                            knownDependency.FullMetadataname == parameterInfo.FullMetadataName);
                                        return FactoryAccessInfo.Create(nameSpace,
                                            CreateFactoryClassName(fullMetadataName));
                                    })
                                .ToList();
                            var factory = FactoryInfo.Create(factoryClassName,
                                moduleFullName,
                                classToConstruct,
                                returnType,
                                parameterFactoryAccessInfos);
                            factoryFile.AddFactory(factory);
                        }
                    }

                    factoryFiles.Add(factoryFile);
                }
            }

            return ConstructionPlanInfo.Create(factoryFiles);
        }

        private static ConstructionInfo GetMostSatisfiedConstructionInfo(MappingInfo mappingInfo,
            IReadOnlyList<(string NameSpace, string FullMetadataName)> knownDependencies)
        {
            var mostSatisfiedConstructionInfo = mappingInfo
                .From
                .ConstructionInfos
                .Where(constructionInfo => constructionInfo.Parameters
                    .All(parameterInfo =>
                        knownDependencies
                            .Select(knownDependency => knownDependency.FullMetadataName)
                            .Contains(parameterInfo.FullMetadataName)))
                .OrderByDescending(constructionInfo => constructionInfo.Parameters.Count)
                .SingleOrDefault();
            if (mostSatisfiedConstructionInfo == null)
            {
                throw new PlannerException(
                    mappingInfo.From.FullMetadataName +
                    " does not have a constructor for which all dependencies are registered.");
            }

            return mostSatisfiedConstructionInfo;
        }

        private static IReadOnlyList<(string NameSpace, string FullMetadataname)> GetKnownDependencies(
            AnalyzerResultInfo analyzerResultInfo)
        {
            return analyzerResultInfo.ProjectInfos
                .SelectMany(projectInfo => projectInfo.DocumentInfos
                    .Select(documentInfo =>
                    (
                        documentInfo.NameSpace,
                        documentInfo.ModuleInfos
                    )))
                .SelectMany(nameSpaceModuleInfo => nameSpaceModuleInfo.ModuleInfos
                    .Select(moduleInfo =>
                    (
                        nameSpaceModuleInfo.NameSpace,
                        moduleInfo.MappingInfos
                    )))
                .SelectMany(nameSpaceMappingInfo => nameSpaceMappingInfo.MappingInfos
                    .Select(mappingInfo =>
                    (
                        nameSpaceMappingInfo.NameSpace,
                        mappingInfo.To.FullMetadataName
                    )))
                .ToList();
        }

        private static string CreateFactoryClassName(string metaDataName)
        {
            return metaDataName.Replace(Dot, Underscore);
        }
    }
}