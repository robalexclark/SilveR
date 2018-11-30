using SilveR.Models;
using SilveR.Validators;
using System.Collections.Generic;

namespace SilveR.StatsModels
{
    public abstract class AnalysisModelBase
    {
        public string ScriptFileName { get; private set; }

        public AnalysisModelBase(string scriptFileName)
        {
            this.ScriptFileName = scriptFileName;
        }

        public abstract ValidationInfo Validate();

        public abstract IEnumerable<Argument> GetArguments();

        public abstract void LoadArguments(IEnumerable<Argument> arguments);

        public abstract string GetCommandLineArguments();
    }
}