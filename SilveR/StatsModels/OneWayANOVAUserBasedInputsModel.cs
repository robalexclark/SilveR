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
    public enum VariabilityEstimate { ResidualMeanSquare = 0, Variance = 1, StandardDeviation = 2 }

    public class OneWayANOVAUserBasedInputsModel : AnalysisModelBase, IGraphSizeOptions
    {
        [DisplayName("Effect size estimate")]
        public EffectSizeEstimate EffectSizeEstimate { get; set; } = EffectSizeEstimate.TreatmentMeanSquare;

        [DisplayName("Treatment Mean square")]
        public Nullable<decimal> TreatmentMeanSquare { get; set; }

        [DisplayName("Means")]
        public string Means { get; set; }

        [DisplayName("Variability estimate")]
        public VariabilityEstimate VariabilityEstimate { get; set; } = VariabilityEstimate.ResidualMeanSquare;

        [DisplayName("Variance")]
        public Nullable<decimal> ResidualMeanSquare { get; set; }

        [DisplayName("Variance")]
        public Nullable<decimal> Variance { get; set; }

        [DisplayName("Variance")]
        public Nullable<decimal> StandardDeviation { get; set; }


        [Range(0, double.MaxValue)]
        [DisplayName("No. of treatment groups")]
        public Nullable<decimal> NoOfTreatmentGroups { get; set; }


        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        public PlottingRangeTypeOption PlottingRangeType { get; set; } = PlottingRangeTypeOption.SampleSize;

        [DisplayName("Sample size from")]
        public Nullable<int> SampleSizeFrom { get; set; } = 6;

        [DisplayName("Sample size to")]
        public Nullable<int> SampleSizeTo { get; set; } = 15;

        [DisplayName("Power from")]
        public Nullable<int> PowerFrom { get; set; } = 70;

        [DisplayName("Power to")]
        public Nullable<int> PowerTo { get; set; } = 90;

        [DisplayName("Graph title")]
        public string GraphTitle { get; set; }


        public OneWayANOVAUserBasedInputsModel() : base("OneWayANOVAUserBasedInputs") { }

        public override ValidationInfo Validate()
        {
            OneWayANOVAUserBasedInputsValidator meansComparisonValidator = new OneWayANOVAUserBasedInputsValidator(this);
            return meansComparisonValidator.Validate();
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(EffectSizeEstimate), EffectSizeEstimate.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(TreatmentMeanSquare), TreatmentMeanSquare));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Means), Means));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(VariabilityEstimate), VariabilityEstimate.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResidualMeanSquare), ResidualMeanSquare));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Variance), Variance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(StandardDeviation), StandardDeviation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NoOfTreatmentGroups), NoOfTreatmentGroups));
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

            this.EffectSizeEstimate = (EffectSizeEstimate)Enum.Parse(typeof(EffectSizeEstimate), argHelper.LoadStringArgument(nameof(EffectSizeEstimate)), true);
            this.TreatmentMeanSquare = argHelper.LoadNullableDecimalArgument(nameof(TreatmentMeanSquare));
            this.Means = argHelper.LoadStringArgument(nameof(Means));
            this.VariabilityEstimate = (VariabilityEstimate)Enum.Parse(typeof(VariabilityEstimate), argHelper.LoadStringArgument(nameof(VariabilityEstimate)), true);
            this.ResidualMeanSquare = argHelper.LoadNullableDecimalArgument(nameof(ResidualMeanSquare));
            this.Variance = argHelper.LoadNullableDecimalArgument(nameof(Variance));
            this.StandardDeviation = argHelper.LoadNullableDecimalArgument(nameof(StandardDeviation));
            this.NoOfTreatmentGroups = argHelper.LoadNullableDecimalArgument(nameof(NoOfTreatmentGroups));
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

            arguments.Append(" " + argFormatter.GetFormattedArgument(EffectSizeEstimate.ToString(), false)); //5

            arguments.Append(" " + argFormatter.GetFormattedArgument(TreatmentMeanSquare)); //6

            arguments.Append(" " + argFormatter.GetFormattedArgument(Means, false)); //6

            arguments.Append(" " + argFormatter.GetFormattedArgument(VariabilityEstimate.ToString(), false)); //7

            arguments.Append(" " + argFormatter.GetFormattedArgument(ResidualMeanSquare)); //8

            arguments.Append(" " + argFormatter.GetFormattedArgument(Variance)); //9

            arguments.Append(" " + argFormatter.GetFormattedArgument(StandardDeviation)); //10

            arguments.Append(" " + argFormatter.GetFormattedArgument(NoOfTreatmentGroups)); //11

            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //12 

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