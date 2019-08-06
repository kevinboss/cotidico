using System;
using System.Linq;
using Microsoft.Build.Locator;

namespace Cotidico.Generator
{
    public class EnvironmentPreparer
    {
        public void RegisterMsBuildInstance()
        {
            var msBuildInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();

            if (msBuildInstances.Length != 1)
            {
                throw new InvalidOperationException();
            }

            MSBuildLocator.RegisterInstance(msBuildInstances[0]);
        }
    }
}