using SilveR.Helpers;
using SilveRModel.Helpers;
using SilveRModel.Models;
using SilveRModel.Validators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;

namespace SilveRModel.StatsModel
{
    public class SummaryStatisticsModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "SummaryStatistics"; } }

        private DataTable dataTable;
        public DataTable DataTable
        {
            get { return dataTable; }
        }

        public Nullable<int> DatasetID { get; set; }

        private IEnumerable<string> availableVariables = new List<string>();
        public IEnumerable<string> AvailableVariables
        {
            get { return availableVariables; }
        }

        public IEnumerable<string> AvailableVariablesAllowNull
        {
            get
            {
                List<string> availableVars = availableVariables.ToList();
                availableVars.Insert(0, String.Empty);
                return availableVars.AsEnumerable();
            }
        }

        [HasAtLeastOneEntry]
        [CheckUsedOnceOnly]
        public List<string> Responses { get; set; }

        public List<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine" }; }
        }

        public string Transformation { get; set; }

        [DisplayName("1st factor")]
        [CheckUsedOnceOnly]
        public string FirstCatFactor { get; set; }

        [DisplayName("2nd factor")]
        [CheckUsedOnceOnly]
        public string SecondCatFactor { get; set; }

        [DisplayName("3rd factor")]
        [CheckUsedOnceOnly]
        public string ThirdCatFactor { get; set; }

        [DisplayName("4th factor")]
        [CheckUsedOnceOnly]
        public string FourthCatFactor { get; set; }

        public bool Mean { get; set; } = true;
        public bool N { get; set; } = true;

        [DisplayName("Standard deviation")]
        public bool StandardDeviation { get; set; } = true;
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

        [DisplayName("Level (%)")]
        [Required(ErrorMessage = "Level (%) is Required")]
        [ValidateConfidenceLimits]
        public decimal Significance { get; set; } = 95;

        [DisplayName("Normal probability plot")]
        public bool NormalProbabilityPlot { get; set; }

        [DisplayName("By categories and overall")]
        public bool ByCategoriesAndOverall { get; set; }

        public SummaryStatisticsModel() { }

        public SummaryStatisticsModel(Dataset dataset)
        {
            //setup model
            ReInitialize(dataset);
        }

        public void ReInitialize(Dataset dataset)
        {
            this.DatasetID = dataset.DatasetID;
            dataTable = dataset.DatasetToDataTable();

            availableVariables = dataTable.GetVariableNames();
        }

        public ValidationInfo Validate()
        {
            SummaryStatisticsValidator summaryStatisticsValidator = new SummaryStatisticsValidator(this);
            return summaryStatisticsValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (!Responses.Contains(columnName) && FirstCatFactor != columnName && SecondCatFactor != columnName && ThirdCatFactor != columnName && FourthCatFactor != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

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

            return dtNew.GetCSVArray();
        }

        public IEnumerable<Argument> GetArguments()
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

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Responses = argHelper.ArgumentLoader(nameof(Responses), Responses);
            this.FirstCatFactor = argHelper.ArgumentLoader(nameof(FirstCatFactor), FirstCatFactor);
            this.SecondCatFactor = argHelper.ArgumentLoader(nameof(SecondCatFactor), SecondCatFactor);
            this.ThirdCatFactor = argHelper.ArgumentLoader(nameof(ThirdCatFactor), ThirdCatFactor);
            this.FourthCatFactor = argHelper.ArgumentLoader(nameof(FourthCatFactor), FourthCatFactor);
            this.Transformation = argHelper.ArgumentLoader(nameof(Transformation), Transformation);
            this.ByCategoriesAndOverall = argHelper.ArgumentLoader(nameof(ByCategoriesAndOverall), ByCategoriesAndOverall);
            this.CoefficientOfVariation = argHelper.ArgumentLoader(nameof(CoefficientOfVariation), CoefficientOfVariation);
            this.ConfidenceInterval = argHelper.ArgumentLoader(nameof(ConfidenceInterval), ConfidenceInterval);
            this.Mean = argHelper.ArgumentLoader(nameof(Mean), Mean);
            this.MedianAndQuartiles = argHelper.ArgumentLoader(nameof(MedianAndQuartiles), MedianAndQuartiles);
            this.MinAndMax = argHelper.ArgumentLoader(nameof(MinAndMax), MinAndMax);
            this.N = argHelper.ArgumentLoader(nameof(N), N);
            this.NormalProbabilityPlot = argHelper.ArgumentLoader(nameof(NormalProbabilityPlot), NormalProbabilityPlot);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
            this.StandardDeviation = argHelper.ArgumentLoader(nameof(StandardDeviation), StandardDeviation);
            this.StandardErrorOfMean = argHelper.ArgumentLoader(nameof(StandardErrorOfMean), StandardErrorOfMean);
            this.Variance = argHelper.ArgumentLoader(nameof(Variance), Variance);
        }

        public string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + argFormatter.GetFormattedArgument(Responses)); //4

            //get transforms
            arguments.Append(" " + argFormatter.GetFormattedArgument(Transformation)); //5

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

            //St Dev
            arguments.Append(" " + argFormatter.GetFormattedArgument(StandardDeviation)); //12

            //Variances
            arguments.Append(" " + argFormatter.GetFormattedArgument(Variance)); //13

            //St Err
            arguments.Append(" " + argFormatter.GetFormattedArgument(StandardErrorOfMean)); //14

            //Min and Max
            arguments.Append(" " + argFormatter.GetFormattedArgument(MinAndMax)); //15

            //Median Quartile
            arguments.Append(" " + argFormatter.GetFormattedArgument(MedianAndQuartiles)); //16

            //Coefficient Variation
            arguments.Append(" " + argFormatter.GetFormattedArgument(CoefficientOfVariation)); //17

            //Confidence Limits
            arguments.Append(" " + argFormatter.GetFormattedArgument(ConfidenceInterval)); //18

            //Confidence Limits
            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance.ToString())); //19

            //Normal Probability Plots
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalProbabilityPlot));

            //By Categories and Overall
            arguments.Append(" " + argFormatter.GetFormattedArgument(ByCategoriesAndOverall)); //20

            return arguments.ToString();
        }


        public bool VariablesUsedOnceOnly(string memberName)
        {
            object varToBeChecked = this.GetType().GetProperty(memberName).GetValue(this, null);

            if (varToBeChecked != null)
            {
                UniqueVariableChecker checker = new UniqueVariableChecker();

                if (memberName != "Responses")
                    checker.AddVars(this.Responses);

                if (memberName != "FirstCatFactor")
                    checker.AddVar(this.FirstCatFactor);

                if (memberName != "SecondCatFactor")
                    checker.AddVar(this.SecondCatFactor);

                if (memberName != "ThirdCatFactor")
                    checker.AddVar(this.ThirdCatFactor);

                if (memberName != "FourthCatFactor")
                    checker.AddVar(this.FourthCatFactor);

                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}