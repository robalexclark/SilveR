using SilveRModel.Models;
using SilveRModel.Validators;
using System;
using System.Collections.Generic;
using System.Data;

namespace SilveRModel.StatsModel
{
    public interface IAnalysisModel
    {
        DataTable DataTable { get; }

        Nullable<int> DatasetID { get; }

        void ReInitialize(Dataset dataset);

        string ScriptFileName { get; }

        ValidationInfo Validate();

        string[] ExportData();

        IEnumerable<Argument> GetArguments();

        void LoadArguments(IEnumerable<Argument> arguments);

        string GetCommandLineArguments();

        bool VariablesUsedOnceOnly(string memberName);
    }
}