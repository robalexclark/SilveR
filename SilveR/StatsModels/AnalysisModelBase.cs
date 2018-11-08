using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SilveR.StatsModels
{
    public abstract class AnalysisModelBase
    {
        public string ScriptFileName { get; private set; }

        public DataTable DataTable { get; private set; }

        public Nullable<int> DatasetID { get; set; }

        public IEnumerable<string> AvailableVariables { get; private set; } = new List<string>();

        public IEnumerable<string> AvailableVariablesAllowNull
        {
            get
            {
                List<string> availableVars = AvailableVariables.ToList();
                availableVars.Insert(0, String.Empty);
                return availableVars.AsEnumerable();
            }
        }

        public void ReInitialize(IDataset dataset)
        {
            this.DatasetID = dataset.DatasetID;
            this.DataTable = dataset.DatasetToDataTable();

            this.AvailableVariables = DataTable.GetVariableNames();
        }

        public AnalysisModelBase(IDataset dataset, string scriptFileName)
        {
            this.ScriptFileName = scriptFileName;

            if (dataset != null)
            {
                //setup model
                ReInitialize(dataset);
            }
        }


        public abstract ValidationInfo Validate();

        public abstract string[] ExportData();

        public abstract IEnumerable<Argument> GetArguments();

        public abstract void LoadArguments(IEnumerable<Argument> arguments);

        public abstract string GetCommandLineArguments();
    }
}