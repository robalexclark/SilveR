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
    public class ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel : AnalysisDataModelBase, IGraphSizeOptions, IMeanChangeOptions
    {
        [CheckUsedOnceOnly]
        [Required]
        [DisplayName("Response")]
        public string Response { get; set; }

        [CheckUsedOnceOnly]
        public string Treatment { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [DisplayName("Control group")]
        public string ControlGroup { get; set; }

        public ChangeTypeOption ChangeType { get; set; } = ChangeTypeOption.Percent;

        [DisplayName("Percent changes")]
        public string PercentChange { get; set; }

        [DisplayName("Absolute changes")]
        public string AbsoluteChange { get; set; }

        public PlottingRangeTypeOption PlottingRangeType { get; set; } = PlottingRangeTypeOption.SampleSize;

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Sample size from must be > 0")]
        [DisplayName("Sample size from")]
        public Nullable<int> SampleSizeFrom { get; set; } = 6;

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Sample size to must be > 0")]
        [DisplayName("Sample size to")]
        public Nullable<int> SampleSizeTo { get; set; } = 15;

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Power from must be > 0")]
        [DisplayName("Power from")]
        public Nullable<int> PowerFrom { get; set; } = 70;

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Power to must be > 0")]
        [DisplayName("Power to")]
        public Nullable<int> PowerTo { get; set; } = 90;

        [DisplayName("Graph title")]
        public string GraphTitle { get; set; }


        public ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel() : base("ComparisonOfMeansPowerAnalysisDatasetBasedInputs") { }

        public ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel(IDataset dataset)
            : base(dataset, "ComparisonOfMeansPowerAnalysisDatasetBasedInputs") { }

        public override ValidationInfo Validate()
        {
            ComparisonOfMeansPowerAnalysisDatasetBasedInputsValidator meansComparisonValidator = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsValidator(this);
            return meansComparisonValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string col in this.DataTable.GetVariableNames())
            {
                if (Response != col && Treatment != col)
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
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ControlGroup), ControlGroup));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ChangeType), ChangeType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PercentChange), PercentChange));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(AbsoluteChange), AbsoluteChange));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PlottingRangeType), PlottingRangeType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SampleSizeFrom), SampleSizeFrom));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SampleSizeTo), SampleSizeTo));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PowerFrom), PowerFrom));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PowerTo), PowerTo));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(GraphTitle), GraphTitle));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.Treatment = argHelper.LoadStringArgument(nameof(Treatment));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
            this.ControlGroup = argHelper.LoadStringArgument(nameof(ControlGroup));
            this.ChangeType = (ChangeTypeOption)Enum.Parse(typeof(ChangeTypeOption), argHelper.LoadStringArgument(nameof(ChangeType)), true);
            this.PercentChange = argHelper.LoadStringArgument(nameof(PercentChange));
            this.AbsoluteChange = argHelper.LoadStringArgument(nameof(AbsoluteChange));
            this.PlottingRangeType = (PlottingRangeTypeOption)Enum.Parse(typeof(PlottingRangeTypeOption), argHelper.LoadStringArgument(nameof(PlottingRangeType)), true);
            this.SampleSizeFrom = argHelper.LoadNullableIntArgument(nameof(SampleSizeFrom));
            this.SampleSizeTo = argHelper.LoadNullableIntArgument(nameof(SampleSizeTo));
            this.PowerFrom = argHelper.LoadNullableIntArgument(nameof(PowerFrom));
            this.PowerTo = argHelper.LoadNullableIntArgument(nameof(PowerTo));
            this.GraphTitle = argHelper.LoadStringArgument(nameof(GraphTitle));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + "DatasetValues"); //4

            arguments.Append(" " + argFormatter.GetFormattedArgument(Response, true)); //5
            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatment, true)); //6
            arguments.Append(" " + argFormatter.GetFormattedArgument(ControlGroup, true)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //8

            if (ChangeType == ChangeTypeOption.Percent)
            {
                arguments.Append(" " + "Percent"); //9
                arguments.Append(" " + argFormatter.GetFormattedArgument(PercentChange, false)); //10
            }
            else
            {
                arguments.Append(" " + "Absolute"); //9
                arguments.Append(" " + argFormatter.GetFormattedArgument(AbsoluteChange, false)); //10
            }

            if (PlottingRangeType == PlottingRangeTypeOption.SampleSize)
            {
                arguments.Append(" " + "SampleSize"); //11
                arguments.Append(" " + argFormatter.GetFormattedArgument(SampleSizeFrom)); //12
                arguments.Append(" " + argFormatter.GetFormattedArgument(SampleSizeTo)); //13
            }
            else
            {
                arguments.Append(" " + "PowerAxis"); //11
                arguments.Append(" " + argFormatter.GetFormattedArgument(PowerFrom)); //12
                arguments.Append(" " + argFormatter.GetFormattedArgument(PowerTo)); //13
            }

            arguments.Append(" " + argFormatter.GetFormattedArgument(GraphTitle, false)); //14

            return arguments.ToString().Trim();
        }
    }
}