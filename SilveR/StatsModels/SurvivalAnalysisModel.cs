using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace SilveR.StatsModels
{
    public class SurvivalAnalysisModel : AnalysisDataModelBase
    {      
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Treatment factor")]
        public string Treatment { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Censorship")]
        public string Censorship { get; set; }

        [DisplayName("Summary results")]
        public bool SummaryResults { get; set; } = true;

        [DisplayName("Survival plot")]
        public bool SurvivalPlot { get; set; } = true;

        [DisplayName("Compare survival curves")]
        public bool CompareSurvivalCurves { get; set; }

        [DisplayName("Significance")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }


        public SurvivalAnalysisModel() : base("SurvivalAnalysis") { }

        public SurvivalAnalysisModel(IDataset dataset)
            : base(dataset, "SurvivalAnalysis") { }

        public override ValidationInfo Validate()
        {
            SurvivalAnalysisValidator survivalAnalysisValidator = new SurvivalAnalysisValidator(this);
            return survivalAnalysisValidator.Validate();
        }

        public override  string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string col in dtNew.GetVariableNames())
            {
                if (Response != col && Treatment != col && Censorship != col)
                {
                    dtNew.Columns.Remove(col);
                }
            }

            //ensure that all data is trimmed
            //dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            string[] csvArray = dtNew.GetCSVArray();

            //fix any columns with illegal chars here (at the end)
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            csvArray[0] = argFormatter.ConvertIllegalCharacters(csvArray[0]);

            return csvArray;
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatment), Treatment));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Censorship), Censorship));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SummaryResults), SummaryResults));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SurvivalPlot), SurvivalPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CompareSurvivalCurves), CompareSurvivalCurves));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public override  void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.Treatment = argHelper.LoadStringArgument(nameof(Treatment));
            this.Censorship = argHelper.LoadStringArgument(nameof(Censorship));
            this.SummaryResults = argHelper.LoadBooleanArgument(nameof(SummaryResults));
            this.SurvivalPlot = argHelper.LoadBooleanArgument(nameof(SurvivalPlot));
            this.CompareSurvivalCurves = argHelper.LoadBooleanArgument(nameof(CompareSurvivalCurves));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
        }

        public override  string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true)); //4
            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatment, true)); //5
            arguments.Append(" " + argFormatter.GetFormattedArgument(Censorship, true)); //6          

            arguments.Append(" " + argFormatter.GetFormattedArgument(SummaryResults)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(SurvivalPlot)); //8
            arguments.Append(" " + argFormatter.GetFormattedArgument(CompareSurvivalCurves)); //9
            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //10

            return arguments.ToString().Trim();
        }
    }
}