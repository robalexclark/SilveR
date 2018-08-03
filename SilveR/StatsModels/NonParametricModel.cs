using SilveRModel.Helpers;
using SilveRModel.Models;
using SilveRModel.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace SilveRModel.StatsModel
{
    public class NonParametricModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "NonParametric"; } }

        private DataTable dataTable;
        public DataTable DataTable
        {
            get { return dataTable; }
        }

        public Nullable<int> DatasetID { get; set; }

        private IEnumerable<string> availableVariables = new List<string>();
        public IEnumerable<string> AvailableVariables
        {
            get { return availableVariables; }
        }

        public IEnumerable<string> AvailableVariablesAllowNull
        {
            get
            {
                List<string> availableVars = availableVariables.ToList();
                availableVars.Insert(0, String.Empty);
                return availableVars.AsEnumerable();
            }
        }

        [Required]
        [CheckUsedOnceOnly]
        public string Response { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        public string Treatment { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        public string OtherDesignFactor { get; set; }

        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [ValidateControlLevelSet]
        public string Control { get; set; }

        private IEnumerable<string> controlList = new List<string>();
        public IEnumerable<string> ControlList
        {
            get { return controlList; }
        }

        public enum AnalysisOption { MannWhitney = 0, AllComparisons = 1, CompareToControl = 2 }
        private AnalysisOption analysisType = AnalysisOption.MannWhitney;
        public AnalysisOption AnalysisType
        {
            get { return analysisType; }
            set
            {
                if (analysisType != value)
                {
                    analysisType = value;
                }
            }
        }

        public NonParametricModel() { }

        public NonParametricModel(Dataset dataset)
        {
            //setup model
            ReInitialize(dataset);
        }

        public void ReInitialize(Dataset dataset)
        {
            this.DatasetID = dataset.DatasetID;
            dataTable = dataset.DatasetToDataTable();

            availableVariables = dataTable.GetVariableNames();
        }

        public ValidationInfo Validate()
        {
            NonParametricsValidator nonParametricsValidator = new NonParametricsValidator(this);
            return nonParametricsValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response and treatment columns
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && Treatment != columnName && OtherDesignFactor != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //...and export them
            return dtNew.GetCSVArray();
        }

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.Treatment = argHelper.ArgumentLoader(nameof(Treatment), Treatment);
            this.OtherDesignFactor = argHelper.ArgumentLoader(nameof(OtherDesignFactor), OtherDesignFactor);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
            this.AnalysisType = (AnalysisOption)Enum.Parse(typeof(AnalysisOption), argHelper.ArgumentLoader(nameof(AnalysisType), String.Empty), true);
            this.Control = argHelper.ArgumentLoader(nameof(Control), Control);
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatment), Treatment));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OtherDesignFactor), OtherDesignFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(AnalysisType), AnalysisType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Control), Control));

            return args;
        }

        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(Response));
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(Treatment));
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(OtherDesignFactor));
            arguments.Append(" " + Significance);
            arguments.Append(" " + AnalysisType.ToString());
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(Control));

            arguments.Append(" " + "N");

            return arguments.ToString();
        }

        public bool VariablesUsedOnceOnly(string memberName)
        {
            object varToBeChecked = this.GetType().GetProperty(memberName).GetValue(this, null);

            if (varToBeChecked != null)
            {
                UniqueVariableChecker checker = new UniqueVariableChecker();

                if (memberName != "Response")
                    checker.AddVar(this.Response);

                if (memberName != "Treatment")
                    checker.AddVar(this.Treatment);

                if (memberName != "OtherDesignFactor")
                    checker.AddVar(this.OtherDesignFactor);

                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}