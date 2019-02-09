using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace SilveR.StatsModels
{
    public class NonParametricAnalysisModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Treatment factor")]
        public string Treatment { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Other design (block) factor")]
        public string OtherDesignFactor { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [ValidateControlLevelSet]
        [DisplayName("Control")]
        public string Control { get; set; }

        public enum AnalysisOption { MannWhitney = 0, AllComparisons = 1, CompareToControl = 2 }
        [DisplayName("Analysis type")]
        public AnalysisOption AnalysisType { get; set; } = AnalysisOption.MannWhitney;

        public NonParametricAnalysisModel() : base("NonParametricAnalysis") { }

        public NonParametricAnalysisModel(IDataset dataset)
            : base(dataset, "NonParametricAnalysis") { }

        public override ValidationInfo Validate()
        {
            NonParametricAnalysisValidator nonParametricAnalysisValidator = new NonParametricAnalysisValidator(this);
            return nonParametricAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response and treatment columns
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && Treatment != columnName && OtherDesignFactor != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //...and export them
            string[] csvArray = dtNew.GetCSVArray();

            //fix any columns with illegal chars here (at the end)
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            csvArray[0] = argFormatter.ConvertIllegalCharacters(csvArray[0]);

            return csvArray;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.Treatment = argHelper.LoadStringArgument(nameof(Treatment));
            this.OtherDesignFactor = argHelper.LoadStringArgument(nameof(OtherDesignFactor));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
            this.AnalysisType = (AnalysisOption)Enum.Parse(typeof(AnalysisOption), argHelper.LoadStringArgument(nameof(AnalysisType)), true);
            this.Control = argHelper.LoadStringArgument(nameof(Control));
        }

        public override IEnumerable<Argument> GetArguments()
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

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true));
            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatment, true));
            arguments.Append(" " + argFormatter.GetFormattedArgument(OtherDesignFactor, true));

            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false));
            arguments.Append(" " + argFormatter.GetFormattedArgument(AnalysisType.ToString(), false));
            arguments.Append(" " + argFormatter.GetFormattedArgument(Control, true));

            return arguments.ToString().Trim();
        }
    }
}