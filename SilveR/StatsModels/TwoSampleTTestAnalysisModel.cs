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
    public class TwoSampleTTestAnalysisModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "TwoSampleTTestAnalysis"; } }

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

        [Display(Name = "Response")]
        [CheckUsedOnceOnly]
        [Required]
        public string Response { get; set; }

        [DisplayName("Response Transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public List<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [Display(Name = "Treatment")]
        [CheckUsedOnceOnly]
        [Required]
        public string Treatment { get; set; }

        [Display(Name = "Significance")]
        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [Display(Name = "Unequal variance case")]
        public bool UnequalVariance { get; set; }

        [Display(Name = "Equal variance case")]
        public bool EqualVariance { get; set; } = true;

        [Display(Name = "Predicated vs. residuals plot")]
        public bool PRPlot { get; set; } = true;

        [Display(Name = "Normal probability plot")]
        public bool NormalPlot { get; set; }


        public TwoSampleTTestAnalysisModel() { }

        public TwoSampleTTestAnalysisModel(Dataset dataset)
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
            TwoSampleTTestAnalysisValidator twoSampleTTestAnalysisValidator = new TwoSampleTTestAnalysisValidator(this);
            return twoSampleTTestAnalysisValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && Treatment != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Now do transformations...
            dtNew.TransformColumn(Response, ResponseTransformation);

            //Finally, as numeric categorical variables get misinterpreted by r, we need to go through
            //each column and put them in quotes...
            if (dtNew.CheckIsNumeric(Treatment))
            {
                foreach (DataRow row in dtNew.Rows)
                {
                    row[Treatment] = "'" + row[Treatment] + "'";
                }
            }

            return dtNew.GetCSVArray();
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatment), Treatment));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(EqualVariance), EqualVariance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(UnequalVariance), UnequalVariance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PRPlot), PRPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalPlot), NormalPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.ResponseTransformation = argHelper.ArgumentLoader(nameof(ResponseTransformation), ResponseTransformation);
            this.Treatment = argHelper.ArgumentLoader(nameof(Treatment), Treatment);
            this.EqualVariance = argHelper.ArgumentLoader(nameof(EqualVariance), EqualVariance);
            this.UnequalVariance = argHelper.ArgumentLoader(nameof(UnequalVariance), UnequalVariance);
            this.PRPlot = argHelper.ArgumentLoader(nameof(PRPlot), PRPlot);
            this.NormalPlot = argHelper.ArgumentLoader(nameof(NormalPlot), NormalPlot);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
        }

        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(Response));
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(ResponseTransformation));

            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(Treatment));

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(EqualVariance));
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(UnequalVariance));

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(PRPlot));
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(NormalPlot));

            arguments.Append(" " + Significance);

            arguments.Append(" " + "N");

            return arguments.ToString();
        }

        public bool VariablesUsedOnceOnly(string memberName)
        {
            object varToBeChecked = this.GetType().GetProperty(memberName).GetValue(this, null);

            if (varToBeChecked != null)
            {
                UniqueVariableChecker checker = new UniqueVariableChecker();

                if (memberName != "Response")
                    checker.AddVar(this.Response);

                if (memberName != "Treatment")
                    checker.AddVar(this.Treatment);

                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}