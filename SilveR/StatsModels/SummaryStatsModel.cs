using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using SilveRModel.Helpers;
using SilveRModel.Models;
using SilveRModel.Validators;

namespace SilveRModel.StatsModel
{
    public class SummaryStatsModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "SummaryStats"; } }

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
        public bool Variances { get; set; }

        [DisplayName("Standard error of mean")]
        public bool StandardErrorOfMean { get; set; }

        [DisplayName("Min and Max")]
        public bool MinAndMax { get; set; }

        [DisplayName("Median and quartile")]
        public bool MedianAndQuartile { get; set; }

        [DisplayName("% coefficient of variation")]
        public bool CoefficientOfVariation { get; set; }

        [DisplayName("Confidence intervals")]
        public bool ConfidenceIntervals { get; set; } = true;

        [DisplayName("Significance level")]
        [Required(ErrorMessage = "Significance is Required")]
        [ValidateConfidenceLimits]
        public int Significance { get; set; } = 95;

        [DisplayName("Normal probability plot")]
        public bool NormalProbabilityPlot { get; set; }

        [DisplayName("By categories and overall")]
        public bool ByCategoriesAndOverall { get; set; }

        public SummaryStatsModel() { }

        public SummaryStatsModel(Dataset dataset)
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
            SummaryStatsValidator summaryStatsValidator = new SummaryStatsValidator(this);
            return summaryStatsValidator.Validate();
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

            args.Add(ArgumentHelper.ArgumentFactory("Responses", Responses));
            args.Add(ArgumentHelper.ArgumentFactory("Transformation", Transformation));
            args.Add(ArgumentHelper.ArgumentFactory("FirstCatFactor", FirstCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory("SecondCatFactor", SecondCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory("ThirdCatFactor", ThirdCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory("FourthCatFactor", FourthCatFactor));
            args.Add(ArgumentHelper.ArgumentFactory("Mean", Mean));
            args.Add(ArgumentHelper.ArgumentFactory("N", N));
            args.Add(ArgumentHelper.ArgumentFactory("StandardDeviation", StandardDeviation));
            args.Add(ArgumentHelper.ArgumentFactory("Variances", Variances));
            args.Add(ArgumentHelper.ArgumentFactory("StandardErrorOfMean", StandardErrorOfMean));
            args.Add(ArgumentHelper.ArgumentFactory("MinAndMax", MinAndMax));
            args.Add(ArgumentHelper.ArgumentFactory("MedianAndQuartile", MedianAndQuartile));
            args.Add(ArgumentHelper.ArgumentFactory("CoefficientOfVariation", CoefficientOfVariation));
            args.Add(ArgumentHelper.ArgumentFactory("ConfidenceIntervals", ConfidenceIntervals));
            args.Add(ArgumentHelper.ArgumentFactory("Significance", Significance));
            args.Add(ArgumentHelper.ArgumentFactory("NormalProbabilityPlot", NormalProbabilityPlot));
            args.Add(ArgumentHelper.ArgumentFactory("ByCategoriesAndOverall", ByCategoriesAndOverall));

            return args;
        }

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Responses = argHelper.ArgumentLoader("Responses", Responses);
            this.FirstCatFactor = argHelper.ArgumentLoader("FirstCatFactor", FirstCatFactor);
            this.SecondCatFactor = argHelper.ArgumentLoader("SecondCatFactor", SecondCatFactor);
            this.ThirdCatFactor = argHelper.ArgumentLoader("ThirdCatFactor", ThirdCatFactor);
            this.FourthCatFactor = argHelper.ArgumentLoader("FourthCatFactor", FourthCatFactor);
            this.Transformation = argHelper.ArgumentLoader("Transformation", Transformation);
            this.ByCategoriesAndOverall = argHelper.ArgumentLoader("ByCategoriesAndOverall", ByCategoriesAndOverall);
            this.CoefficientOfVariation = argHelper.ArgumentLoader("CoefficientOfVariation", CoefficientOfVariation);
            this.ConfidenceIntervals = argHelper.ArgumentLoader("ConfidenceIntervals", ConfidenceIntervals);
            this.Mean = argHelper.ArgumentLoader("Mean", Mean);
            this.MedianAndQuartile = argHelper.ArgumentLoader("MedianAndQuartile", MedianAndQuartile);
            this.MinAndMax = argHelper.ArgumentLoader("MinAndMax", MinAndMax);
            this.N = argHelper.ArgumentLoader("N", N);
            this.NormalProbabilityPlot = argHelper.ArgumentLoader("NormalProbabilityPlot", NormalProbabilityPlot);
            this.Significance = argHelper.ArgumentLoader("Significance", Significance);
            this.StandardDeviation = argHelper.ArgumentLoader("StandardDeviation", StandardDeviation);
            this.StandardErrorOfMean = argHelper.ArgumentLoader("StandardErrorOfMean", StandardErrorOfMean);
            this.Variances = argHelper.ArgumentLoader("Variances", Variances);
        }

        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            //get responses, comma separated
            string responses = null;
            foreach (string resp in Responses)
            {
                responses = responses + "," + resp;
            }

            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(responses.TrimStart(','))); //4

            //get transforms
            arguments.Append(" " + "\"" + Transformation + "\""); //5

            //1st cat factor
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(FirstCatFactor))); //6

            //2nd cat factor
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(SecondCatFactor))); //7

            //3rd cat factor
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(ThirdCatFactor))); //8

            //4th cat factor
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(FourthCatFactor))); //9

            //Mean
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(Mean)); //10

            //N
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(N)); //11

            //St Dev
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(StandardDeviation)); //12

            //Variances
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(Variances)); //13

            //St Err
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(StandardErrorOfMean)); //14

            //Min and Max
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(MinAndMax)); //15

            //Median Quartile
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(MedianAndQuartile)); //16

            //Coefficient Variation
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(CoefficientOfVariation)); //17

            //Confidence Limits
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ConfidenceIntervals)); //18

            //Confidence Limits
            arguments.Append(" " + Significance.ToString()); //19

            //Normal Probability Plots
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(NormalProbabilityPlot));

            //By Categories and Overall
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ByCategoriesAndOverall)); //20

            arguments.Append(" " + "N");

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