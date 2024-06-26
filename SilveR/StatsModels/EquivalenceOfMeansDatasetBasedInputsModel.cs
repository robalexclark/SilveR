﻿using SilveR.Helpers;
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
    public class EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel : AnalysisDataModelBase, IGraphSizeOptions, ITrueDifference
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
            get { return new List<string>() { "0.1", "0.05", "0.025", "0.01", "0.001" }; }
        }

        [DisplayName("Control group")]
        public string ControlGroup { get; set; }

        [DisplayName("True difference")]
        [CheckTrueDifferenceAttribute]
        public string TrueDifference { get; set; } = "0";

        public enum EquivalenceBoundsOption { Absolute = 0, Percentage = 1 }
        [DisplayName("Equivalence bounds type")]
        public EquivalenceBoundsOption EquivalenceBoundsType { get; set; } = EquivalenceBoundsOption.Absolute;

        [DisplayName("Lower bound absolute")]
        public Nullable<decimal> LowerBoundAbsolute { get; set; }

        [DisplayName("Upper bound absolute")]
        public Nullable<decimal> UpperBoundAbsolute { get; set; }

        [DisplayName("Lower bound percentage")]
        [Range(0, 1000, ErrorMessage = "User cannot select a negative Percentage Change.")]
        public Nullable<decimal> LowerBoundPercentageChange { get; set; }

        [DisplayName("Upper bound percentage")]
        [Range(0, 1000, ErrorMessage = "User cannot select a negative Percentage Change.")]
        public Nullable<decimal> UpperBoundPercentageChange { get; set; }



        public PlottingRangeTypeOption PlottingRangeType { get; set; } = PlottingRangeTypeOption.SampleSize;

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Sample size from must be > 0.")]
        [DisplayName("Sample size from")]
        public Nullable<int> SampleSizeFrom { get; set; } = 6;

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Sample size to must be > 0.")]
        [DisplayName("Sample size to")]
        public Nullable<int> SampleSizeTo { get; set; } = 15;

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Power from must be > 0.")]
        [DisplayName("Power from")]
        public Nullable<int> PowerFrom { get; set; } = 70;

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Power to must be > 0.")]
        [DisplayName("Power to")]
        public Nullable<int> PowerTo { get; set; } = 90;

        [DisplayName("Graph title")]
        public string GraphTitle { get; set; }


        public EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel() : base("EquivalenceOfMeansPowerAnalysisDatasetBasedInputs") { }

        public EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel(IDataset dataset)
            : base(dataset, "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs") { }

        public override ValidationInfo Validate()
        {
            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsValidator equivalenceOfMeansPowerAnalysisDatasetBasedInputsValidator = new EquivalenceOfMeansPowerAnalysisDatasetBasedInputsValidator(this);
            return equivalenceOfMeansPowerAnalysisDatasetBasedInputsValidator.Validate();
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
            args.Add(ArgumentHelper.ArgumentFactory(nameof(TrueDifference), TrueDifference));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(EquivalenceBoundsType), EquivalenceBoundsType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LowerBoundAbsolute), LowerBoundAbsolute));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(UpperBoundAbsolute), UpperBoundAbsolute));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LowerBoundPercentageChange), LowerBoundPercentageChange));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(UpperBoundPercentageChange), UpperBoundPercentageChange));
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
            this.TrueDifference = argHelper.LoadStringArgument(nameof(TrueDifference));
            this.EquivalenceBoundsType = (EquivalenceBoundsOption)Enum.Parse(typeof(EquivalenceBoundsOption), argHelper.LoadStringArgument(nameof(EquivalenceBoundsType)), true);
            this.LowerBoundAbsolute = argHelper.LoadNullableDecimalArgument(nameof(LowerBoundAbsolute));
            this.UpperBoundAbsolute = argHelper.LoadNullableDecimalArgument(nameof(UpperBoundAbsolute));
            this.LowerBoundPercentageChange = argHelper.LoadNullableDecimalArgument(nameof(LowerBoundPercentageChange));
            this.UpperBoundPercentageChange = argHelper.LoadNullableDecimalArgument(nameof(UpperBoundPercentageChange));
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
            arguments.Append(" " + argFormatter.GetFormattedArgument(TrueDifference, false)); //9

            arguments.Append(" " + argFormatter.GetFormattedArgument(EquivalenceBoundsType.ToString())); //10

            if (EquivalenceBoundsType == EquivalenceBoundsOption.Absolute)
            {
                arguments.Append(" " + argFormatter.GetFormattedArgument(LowerBoundAbsolute)); //11
                arguments.Append(" " + argFormatter.GetFormattedArgument(UpperBoundAbsolute)); //12
            }
            else
            {
                arguments.Append(" " + argFormatter.GetFormattedArgument(LowerBoundPercentageChange)); //11
                arguments.Append(" " + argFormatter.GetFormattedArgument(UpperBoundPercentageChange)); //12
            }


            if (PlottingRangeType == PlottingRangeTypeOption.SampleSize)
            {
                arguments.Append(" " + "SampleSize"); //13
                arguments.Append(" " + argFormatter.GetFormattedArgument(SampleSizeFrom)); //14
                arguments.Append(" " + argFormatter.GetFormattedArgument(SampleSizeTo)); //15
            }
            else
            {
                arguments.Append(" " + "PowerAxis"); //13
                arguments.Append(" " + argFormatter.GetFormattedArgument(PowerFrom)); //14
                arguments.Append(" " + argFormatter.GetFormattedArgument(PowerTo)); //15
            }

            arguments.Append(" " + argFormatter.GetFormattedArgument(GraphTitle, false)); //16

            return arguments.ToString().Trim();
        }
    }
}