using SilveR.Models;
using SilveR.Validators;
using System.Collections.Generic;

namespace SilveR.StatsModels
{
    public abstract class AnalysisModelBase
    {
        public string ScriptFileName { get; private set; }

        protected AnalysisModelBase(string scriptFileName)
        {
            this.ScriptFileName = scriptFileName;
        }

        public string CustomRCode { get; private set; }

        public void SetCustomRCode(string rCode)
        {
            CustomRCode = rCode;
        }

        public abstract ValidationInfo Validate();

        public abstract IEnumerable<Argument> GetArguments();

        public abstract void LoadArguments(IEnumerable<Argument> arguments);

        public abstract string GetCommandLineArguments();
    }
}