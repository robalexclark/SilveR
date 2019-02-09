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
    public enum EffectSizeEstimate { TreatmentMeanSquare = 0, TreatmentGroupMeans = 1 }
    public enum VariabilityEstimate { Variance = 1, StandardDeviation = 2 }

    public class OneWayANOVAPowerAnalysisUserBasedInputsModel : AnalysisModelBase, IGraphSizeOptions
    {
        [Required]
        [DisplayName("Means")]
        public string Means { get; set; }

        [DisplayName("Variability estimate")]
        public VariabilityEstimate VariabilityEstimate { get; set; } = VariabilityEstimate.Variance;

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Variance must be > 0")]
        [DisplayName("Variance (residual mean square)")]
        public Nullable<decimal> Variance { get; set; }

        [Range(0.00000001, Int32.MaxValue, ErrorMessage = "Standard deviation must be > 0")]
        [DisplayName("Standard deviation")]
        public Nullable<decimal> StandardDeviation { get; set; }

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


        public OneWayANOVAPowerAnalysisUserBasedInputsModel() : base("OneWayANOVAPowerAnalysisUserBasedInputs") { }

        public override ValidationInfo Validate()
        {
            OneWayANOVAPowerAnalysisUserBasedInputsValidator meansComparisonValidator = new OneWayANOVAPowerAnalysisUserBasedInputsValidator(this);
            return meansComparisonValidator.Validate();
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Means), Means));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(VariabilityEstimate), VariabilityEstimate.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Variance), Variance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(StandardDeviation), StandardDeviation));
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

            this.Means = argHelper.LoadStringArgument(nameof(Means));
            this.VariabilityEstimate = (VariabilityEstimate)Enum.Parse(typeof(VariabilityEstimate), argHelper.LoadStringArgument(nameof(VariabilityEstimate)), true);
            this.Variance = argHelper.LoadNullableDecimalArgument(nameof(Variance));
            this.StandardDeviation = argHelper.LoadNullableDecimalArgument(nameof(StandardDeviation));
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

            arguments.Append(" " + "UserValues"); //4
            arguments.Append(" " + argFormatter.GetFormattedArgument(Means, false)); //5
            arguments.Append(" " + argFormatter.GetFormattedArgument(VariabilityEstimate.ToString(), false)); //6
            arguments.Append(" " + argFormatter.GetFormattedArgument(Variance)); //7
            arguments.Append(" " + argFormatter.GetFormattedArgument(StandardDeviation)); //8
            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //9

            if (PlottingRangeType == PlottingRangeTypeOption.SampleSize)
            {
                arguments.Append(" " + "SampleSize"); //10
                arguments.Append(" " + argFormatter.GetFormattedArgument(SampleSizeFrom)); //11
                arguments.Append(" " + argFormatter.GetFormattedArgument(SampleSizeTo)); //12
            }
            else
            {
                arguments.Append(" " + "PowerAxis"); //10
                arguments.Append(" " + argFormatter.GetFormattedArgument(PowerFrom)); //11
                arguments.Append(" " + argFormatter.GetFormattedArgument(PowerTo)); //12
            }

            arguments.Append(" " + argFormatter.GetFormattedArgument(GraphTitle, false)); //13

            return arguments.ToString().Trim();
        }
    }
}