using SilveR.Helpers;
using SilveRModel.Helpers;
using SilveRModel.Models;
using SilveRModel.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace SilveRModel.StatsModel
{
    public class SurvivalAnalysisModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "SurvivalAnalysis"; } }

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
        public string Grouping { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        public string Censorship { get; set; }


        [DisplayName("Summary results")]
        public bool SummaryResults { get; set; } = true;

        [DisplayName("Survival plot")]
        public bool SurvivalPlot { get; set; } = true;

        [DisplayName("Compare survival curves")]
        public bool CompareSurvivalCurves { get; set; }

        [Display(Name = "Significance")]
        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }


        public SurvivalAnalysisModel() { }

        public SurvivalAnalysisModel(Dataset dataset)
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
            SurvivalAnalysisValidator survivalAnalysisValidator = new SurvivalAnalysisValidator(this);
            return survivalAnalysisValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string col in dtNew.GetVariableNames())
            {
                if (Response != col && Grouping != col && Censorship != col)
                {
                    dtNew.Columns.Remove(col);
                }
            }

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            return dtNew.GetCSVArray();
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Grouping), Grouping));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Censorship), Censorship));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SummaryResults), SummaryResults));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SurvivalPlot), SurvivalPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CompareSurvivalCurves), CompareSurvivalCurves));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.Grouping = argHelper.ArgumentLoader(nameof(Grouping), Grouping);
            this.Censorship = argHelper.ArgumentLoader(nameof(Censorship), Censorship);
            this.SummaryResults = argHelper.ArgumentLoader(nameof(SummaryResults), SummaryResults);
            this.SurvivalPlot = argHelper.ArgumentLoader(nameof(SurvivalPlot), SurvivalPlot);
            this.CompareSurvivalCurves = argHelper.ArgumentLoader(nameof(CompareSurvivalCurves), CompareSurvivalCurves);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
        }

        public string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true)); //4
            arguments.Append(" " + argFormatter.GetFormattedArgument(Grouping, true)); //5
            arguments.Append(" " + argFormatter.GetFormattedArgument(Censorship, true)); //6          

            arguments.Append(" " + argFormatter.GetFormattedArgument(SummaryResults)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(SurvivalPlot)); //8
            arguments.Append(" " + argFormatter.GetFormattedArgument(CompareSurvivalCurves)); //9
            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance)); //10

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

                if (memberName != "Grouping")
                    checker.AddVar(this.Grouping);

                if (memberName != "Censorship")
                    checker.AddVar(this.Censorship);

                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}