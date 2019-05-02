using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace SilveR.StatsModels
{
    public class PValueAdjustmentDatasetBasedInputsModel : AnalysisDataModelBase
    {
        [Required]
        [DisplayName("Unadjusted p-Values")]
        public string PValues { get; set; }

        [DisplayName("Dataset labels")]
        public string DatasetLabels { get; set; }

        [DisplayName("Multiple comparison adjustment")]
        public string SelectedTest { get; set; }

        public IEnumerable<string> MultipleComparisonTests
        {
            get { return new List<string>() { "Holm", "Hochberg", "Hommel", "Benjamini-Hochberg", "Bonferroni" }; }
        }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        public PValueAdjustmentDatasetBasedInputsModel() : base("PValueAdjustmentDatasetBasedInputs") { }

        public PValueAdjustmentDatasetBasedInputsModel(IDataset dataset)
            : base(dataset, "PValueAdjustmentDatasetBasedInputs") { }


        public override ValidationInfo Validate()
        {
            PValueAdjustmentDatasetBasedInputsValidator pValueAdjustmentDatasetBasedInputsValidator = new PValueAdjustmentDatasetBasedInputsValidator();
            return pValueAdjustmentDatasetBasedInputsValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response and treatment columns
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (PValues != columnName && DatasetLabels != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(PValues);

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

            this.PValues = argHelper.LoadStringArgument(nameof(PValues));
            this.DatasetLabels = argHelper.LoadStringArgument(nameof(DatasetLabels));
            this.SelectedTest = argHelper.LoadStringArgument(nameof(SelectedTest));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(PValues), PValues));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(DatasetLabels), DatasetLabels));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SelectedTest), SelectedTest));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(PValues, true)); //4
            arguments.Append(" " + argFormatter.GetFormattedArgument(DatasetLabels, true)); //5
            arguments.Append(" " + SelectedTest); //6
            arguments.Append(" " + Significance.Replace("<", "^<")); //7

            return arguments.ToString().Trim();
        }
    }
}