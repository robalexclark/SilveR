using SilveR.Helpers;
using SilveR.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SilveR.StatsModels
{
    public abstract class AnalysisDataModelBase : AnalysisModelBase
    {
        public DataTable DataTable { get; private set; }

        public int DatasetID { get; set; } //needs to be public so view can update it

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

        public AnalysisDataModelBase(IDataset dataset, string scriptFileName)
            : base(scriptFileName)
        {
            //setup model
            ReInitialize(dataset);
        }

        protected AnalysisDataModelBase(string scriptFileName)
            : base(scriptFileName)
        {
        }

        public void ReInitialize(IDataset dataset)
        {
            this.DatasetID = dataset.DatasetID;
            this.DataTable = dataset.DatasetToDataTable();

            this.AvailableVariables = DataTable.GetVariableNames();
        }

        public abstract string[] ExportData();
    }
}
