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
    public class OneWayANOVAPowerAnalysisDatasetBasedInputsModel : AnalysisDataModelBase, IGraphSizeOptions
    {
        [CheckUsedOnceOnly]
        [Required]
        [DisplayName("Response")]
        public string Response { get; set; }

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [CheckUsedOnceOnly]
        public string Treatment { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

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


        public OneWayANOVAPowerAnalysisDatasetBasedInputsModel() : base("OneWayANOVAPowerAnalysisDatasetBasedInputs") { }

        public OneWayANOVAPowerAnalysisDatasetBasedInputsModel(IDataset dataset)
            : base(dataset, "OneWayANOVAPowerAnalysisDatasetBasedInputs") { }

        public override ValidationInfo Validate()
        {
            OneWayANOVAPowerAnalysisDatasetBasedInputsValidator meansComparisonValidator = new OneWayANOVAPowerAnalysisDatasetBasedInputsValidator(this);
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

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Now do transformations...
            dtNew.TransformColumn(Response, ResponseTransformation);

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
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatment), Treatment));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
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
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.Treatment = argHelper.LoadStringArgument(nameof(Treatment));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
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
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false)); //6

            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatment, true)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //8

            if (PlottingRangeType == PlottingRangeTypeOption.SampleSize)
            {
                arguments.Append(" " + "SampleSize"); //9
                arguments.Append(" " + argFormatter.GetFormattedArgument(SampleSizeFrom)); //10
                arguments.Append(" " + argFormatter.GetFormattedArgument(SampleSizeTo)); //11
            }
            else
            {
                arguments.Append(" " + "PowerAxis"); //9
                arguments.Append(" " + argFormatter.GetFormattedArgument(PowerFrom)); //10
                arguments.Append(" " + argFormatter.GetFormattedArgument(PowerTo)); //11
            }

            arguments.Append(" " + argFormatter.GetFormattedArgument(GraphTitle, false)); //12

            return arguments.ToString().Trim();
        }
    }
}