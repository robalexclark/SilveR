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
    public class CorrelationAnalysisModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "CorrelationAnalysis"; } }

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

        [HasAtLeastTwoEntries]
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

        public List<string> MethodsList
        {
            get { return new List<string>() { "Pearson", "Kendall", "Spearman" }; }
        }

        public string Method { get; set; }

        public List<string> HypothesesList
        {
            get { return new List<string>() { "2-sided", "Less than", "Greater than" }; }
        }

        public string Hypothesis { get; set; }

        public bool Estimate { get; set; } = true;
        public bool Statistic { get; set; } = true;

        [DisplayName("p-Value")]
        public bool PValue { get; set; } = true;
        public bool Scatterplot { get; set; }

        public bool Matrixplot { get; set; }

        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [DisplayName("By categories and overall")]
        public bool ByCategoriesAndOverall { get; set; }

        public CorrelationAnalysisModel() { }

        public CorrelationAnalysisModel(Dataset dataset)
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
            CorrelationAnalysisValidator correlationAnalysisValidator = new CorrelationAnalysisValidator(this);
            return correlationAnalysisValidator.Validate();
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
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Method), Method));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Hypothesis), Hypothesis));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Estimate), Estimate));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Statistic), Statistic));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PValue), PValue));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Scatterplot), Scatterplot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Matrixplot), Matrixplot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
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
            this.Method = argHelper.ArgumentLoader(nameof(Method), Method);
            this.Hypothesis = argHelper.ArgumentLoader(nameof(Hypothesis), Hypothesis);
            this.Estimate = argHelper.ArgumentLoader(nameof(Estimate), Estimate);
            this.Statistic = argHelper.ArgumentLoader(nameof(Statistic), Statistic);
            this.PValue = argHelper.ArgumentLoader(nameof(PValue), PValue);
            this.Scatterplot = argHelper.ArgumentLoader(nameof(Scatterplot), Scatterplot);
            this.Matrixplot = argHelper.ArgumentLoader(nameof(Matrixplot), Matrixplot);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
            this.ByCategoriesAndOverall = argHelper.ArgumentLoader(nameof(ByCategoriesAndOverall), ByCategoriesAndOverall);
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

            //Method
            arguments.Append(" " + Method); //10

            //Hypothesis
            arguments.Append(" " + Hypothesis); //11

            //Mean
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(Estimate)); //12

            //N
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(Statistic)); //13

            //St Dev
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(PValue)); //14

            //Variances
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(Scatterplot)); //15

            //St Err
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(Matrixplot)); //16

            //Min and Max
            arguments.Append(" " + Significance); //17

            //By Categories and Overall
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ByCategoriesAndOverall)); //18

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