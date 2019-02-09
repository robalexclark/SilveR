using SilveR.Helpers;
using SilveR.Models;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SilveR.StatsModels
{
    public enum ChangeTypeOption { Percent = 0, Absolute = 1 }
    public enum PlottingRangeTypeOption { SampleSize = 0, Power = 1 }
    public enum DeviationType { StandardDeviation = 0, Variance =1 }

    public class ComparisonOfMeansPowerAnalysisUserBasedInputsModel : AnalysisModelBase, IGraphSizeOptions, IMeanChangeOptions
    {
        [Range(0, Int32.MaxValue, ErrorMessage = "Group mean must be >= 0")]
        [DisplayName("Group mean")]
        public Nullable<decimal> GroupMean { get; set; }

        public DeviationType DeviationType { get; set; } = DeviationType.StandardDeviation;

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Standard deviation must be > 0")]
        [DisplayName("Standard deviation")]
        public Nullable<decimal> StandardDeviation { get; set; }

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Variance must be > 0")]
        [DisplayName("Variance")]
        public Nullable<decimal> Variance { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

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


        public ComparisonOfMeansPowerAnalysisUserBasedInputsModel() : base("ComparisonOfMeansPowerAnalysisUserBasedInputs") { }

        public override ValidationInfo Validate()
        {
            ComparisonOfMeansPowerAnalysisUserBasedInputsValidator meansComparisonValidator = new ComparisonOfMeansPowerAnalysisUserBasedInputsValidator(this);
            return meansComparisonValidator.Validate();
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(GroupMean), GroupMean));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(DeviationType), DeviationType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(StandardDeviation), StandardDeviation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Variance), Variance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
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

            this.GroupMean = argHelper.LoadNullableDecimalArgument(nameof(GroupMean));
            this.DeviationType = (DeviationType)Enum.Parse(typeof(DeviationType), argHelper.LoadStringArgument(nameof(DeviationType)), true);
            this.StandardDeviation = argHelper.LoadNullableDecimalArgument(nameof(StandardDeviation));
            this.Variance = argHelper.LoadNullableDecimalArgument(nameof(Variance));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
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

            arguments.Append(" " + "UserValues"); //4

            arguments.Append(" " + argFormatter.GetFormattedArgument(GroupMean.ToString(), false)); //5

            if (DeviationType == DeviationType.StandardDeviation)
            {
                arguments.Append(" " + "StandardDeviation"); //6
                arguments.Append(" " + argFormatter.GetFormattedArgument(StandardDeviation.ToString(), false)); //7
            }
            else
            {
                arguments.Append(" " + "Variance"); //6
                arguments.Append(" " + argFormatter.GetFormattedArgument(Variance)); //7
            }

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