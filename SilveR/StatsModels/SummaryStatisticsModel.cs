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
    public class SummaryStatisticsModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Responses")]
        public IEnumerable<string> Responses { get; set; }

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine" }; }
        }

        [DisplayName("Response transformation")]
        public string Transformation { get; set; } = "None";

        [CheckUsedOnceOnly]
        [DisplayName("1st factor")]
        public string FirstCatFactor { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("2nd factor")]
        public string SecondCatFactor { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("3rd factor")]
        public string ThirdCatFactor { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("4th factor")]
        public string FourthCatFactor { get; set; }

        [DisplayName("Mean")]
        public bool Mean { get; set; } = true;

        [DisplayName("N")]
        public bool N { get; set; } = false;

        [DisplayName("Sum")]
        public bool Sum { get; set; }

        [DisplayName("Standard deviation")]
        public bool StandardDeviation { get; set; } = true;

        [DisplayName("Variance")]
        public bool Variance { get; set; }

        [DisplayName("Standard error of mean")]
        public bool StandardErrorOfMean { get; set; }

        [DisplayName("Min and max")]
        public bool MinAndMax { get; set; }

        [DisplayName("Median and quartiles")]
        public bool MedianAndQuartiles { get; set; }

        [DisplayName("% coefficient of variation")]
        public bool CoefficientOfVariation { get; set; }

        [DisplayName("Confidence interval of the mean")]
        public bool ConfidenceInterval { get; set; } = true;

        [Required(ErrorMessage = "Level (%) is Required")]
        [ValidateConfidenceLimits]
        [DisplayName("Level (%)")]
        public int Significance { get; set; } = 95;

        [DisplayName("Normal probability plot")]
        public bool NormalProbabilityPlot { get; set; }

        [DisplayName("By categories and overall")]
        public bool ByCategoriesAndOverall { get; set; }

        public SummaryStatisticsModel() : base("SummaryStatistics") { }

        public SummaryStatisticsModel(IDataset dataset)
            : base(dataset, "SummaryStatistics") { }


        public override ValidationInfo Validate()
        {
            SummaryStatisticsValidator summaryStatisticsValidator = new SummaryStatisticsValidator(this);
            return summaryStatisticsValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (!Responses.Contains(columnName) && FirstCatFactor != columnName && SecondCatFactor != columnName && ThirdCatFactor != columnName && FourthCatFactor != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //Generate a "catfact" column from the CatFactors!
            DataColumn catFactor = new DataColumn("catfact");
            dtNew.Columns.Add(catFactor);

            foreach (DataRow r in dtNew.Rows) //go through each row...
            {
                string firstCatFactorValue = null;
                string secondCatFactorValue = null;
                string thirdCatFactorValue = null;
                string fourthCatFactorValue = null;

                if (!String.IsNullOrEmpty(FirstCatFactor))
                    firstCatFactorValue = r[FirstCatFactor].ToString() + " ";

                if (!String.IsNullOrEmpty(SecondCatFactor))
                    secondCatFactorValue = r[SecondCatFactor].ToString() + " ";

                if (!String.IsNullOrEmpty(ThirdCatFactor))
                    thirdCatFactorValue = r[ThirdCatFactor].ToString() + " ";

                if (!String.IsNullOrEmpty(FourthCatFactor))
                    fourthCatFactorValue = r[FourthCatFactor].ToString();

                string catFactorValue = firstCatFactorValue + secondCatFactorValue + thirdCatFactorValue + fourthCatFactorValue;
                r["catfact"] = catFactorValue.Trim(); //copy the cat factor to the new column
            }

            //Now do transformations
            foreach (string resp in Responses)
            {
                dtNew.TransformColumn(resp, Transformation);
            }


            string[] csvArray = dtNew.GetCSVArray();

            //fix any columns with illegal chars here (at the end)
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            csvArray[0] = argFormatter.ConvertIllegalCharacters(csvArray[0]);

            return csvArray;
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Responses), Responses));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Transformation), Transformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(FirstCatFactor), FirstCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SecondCatFactor), SecondCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ThirdCatFactor), ThirdCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(FourthCatFactor), FourthCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Mean), Mean));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(N), N));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Sum), Sum));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(StandardDeviation), StandardDeviation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Variance), Variance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(StandardErrorOfMean), StandardErrorOfMean));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(MinAndMax), MinAndMax));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(MedianAndQuartiles), MedianAndQuartiles));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CoefficientOfVariation), CoefficientOfVariation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ConfidenceInterval), ConfidenceInterval));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalProbabilityPlot), NormalProbabilityPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ByCategoriesAndOverall), ByCategoriesAndOverall));

            return args;
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Responses = argHelper.LoadIEnumerableArgument(nameof(Responses));
            this.FirstCatFactor = argHelper.LoadStringArgument(nameof(FirstCatFactor));
            this.SecondCatFactor = argHelper.LoadStringArgument(nameof(SecondCatFactor));
            this.ThirdCatFactor = argHelper.LoadStringArgument(nameof(ThirdCatFactor));
            this.FourthCatFactor = argHelper.LoadStringArgument(nameof(FourthCatFactor));
            this.Transformation = argHelper.LoadStringArgument(nameof(Transformation));
            this.ByCategoriesAndOverall = argHelper.LoadBooleanArgument(nameof(ByCategoriesAndOverall));
            this.CoefficientOfVariation = argHelper.LoadBooleanArgument(nameof(CoefficientOfVariation));
            this.ConfidenceInterval = argHelper.LoadBooleanArgument(nameof(ConfidenceInterval));
            this.Mean = argHelper.LoadBooleanArgument(nameof(Mean));
            this.MedianAndQuartiles = argHelper.LoadBooleanArgument(nameof(MedianAndQuartiles));
            this.MinAndMax = argHelper.LoadBooleanArgument(nameof(MinAndMax));
            this.N = argHelper.LoadBooleanArgument(nameof(N));
            this.Sum = argHelper.LoadBooleanArgument(nameof(Sum));
            this.NormalProbabilityPlot = argHelper.LoadBooleanArgument(nameof(NormalProbabilityPlot));
            this.Significance = argHelper.LoadIntArgument(nameof(Significance));
            this.StandardDeviation = argHelper.LoadBooleanArgument(nameof(StandardDeviation));
            this.StandardErrorOfMean = argHelper.LoadBooleanArgument(nameof(StandardErrorOfMean));
            this.Variance = argHelper.LoadBooleanArgument(nameof(Variance));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Responses)); //4

            //get transforms
            arguments.Append(" " + argFormatter.GetFormattedArgument(Transformation, false)); //5

            //1st cat factor
            arguments.Append(" " + argFormatter.GetFormattedArgument(FirstCatFactor, true)); //6

            //2nd cat factor
            arguments.Append(" " + argFormatter.GetFormattedArgument(SecondCatFactor, true)); //7

            //3rd cat factor
            arguments.Append(" " + argFormatter.GetFormattedArgument(ThirdCatFactor, true)); //8

            //4th cat factor
            arguments.Append(" " + argFormatter.GetFormattedArgument(FourthCatFactor, true)); //9

            //Mean
            arguments.Append(" " + argFormatter.GetFormattedArgument(Mean)); //10

            //N
            arguments.Append(" " + argFormatter.GetFormattedArgument(N)); //11

            //Sum
            arguments.Append(" " + argFormatter.GetFormattedArgument(Sum)); //12

            //St Dev
            arguments.Append(" " + argFormatter.GetFormattedArgument(StandardDeviation)); //13

            //Variances
            arguments.Append(" " + argFormatter.GetFormattedArgument(Variance)); //14

            //St Err
            arguments.Append(" " + argFormatter.GetFormattedArgument(StandardErrorOfMean)); //15

            //Min and Max
            arguments.Append(" " + argFormatter.GetFormattedArgument(MinAndMax)); //16

            //Median Quartile
            arguments.Append(" " + argFormatter.GetFormattedArgument(MedianAndQuartiles)); //17

            //Coefficient Variation
            arguments.Append(" " + argFormatter.GetFormattedArgument(CoefficientOfVariation)); //18

            //Confidence Limits
            arguments.Append(" " + argFormatter.GetFormattedArgument(ConfidenceInterval)); //19

            //Confidence Limits
            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance.ToString(), false)); //20

            //Normal Probability Plots
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalProbabilityPlot)); //21

            //By Categories and Overall
            arguments.Append(" " + argFormatter.GetFormattedArgument(ByCategoriesAndOverall)); //22

            return arguments.ToString().Trim();
        }
    }
}