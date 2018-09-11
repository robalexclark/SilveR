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
    public class UnpairedTTestAnalysisModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "UnpairedTTestAnalysis"; } }

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

        [Required]
        [CheckUsedOnceOnly]
        public string Response { get; set; }

        [DisplayName("Response Transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public List<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [Required]
        [CheckUsedOnceOnly]
        public string Treatment { get; set; }


        [DisplayName("Equal variance case")]
        public bool EqualVarianceCaseSelected { get; set; } = true;

        [DisplayName("Unequal variance case")]
        public bool UnequalVarianceCaseSelected { get; set; } = true;

        [DisplayName("Residuals vs. predicted plot")]
        public bool ResidualsVsPredictedPlotSelected { get; set; }

        [DisplayName("Normal Probability Plot")]
        public bool NormalProbabilityPlotSelected { get; set; }

        [Display(Name = "Significance")]
        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }


        public UnpairedTTestAnalysisModel() { }

        public UnpairedTTestAnalysisModel(Dataset dataset)
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
            UnpairedTTestAnalysisValidator unpairedTTestAnalysisValidator = new UnpairedTTestAnalysisValidator(this);
            return unpairedTTestAnalysisValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string col in dtNew.GetVariableNames())
            {
                if (Response != col && Treatment != col)
                {
                    dtNew.Columns.Remove(col);
                }
            }

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Now do transformations...
            dtNew.TransformColumn(Response, ResponseTransformation);

            return dtNew.GetCSVArray();
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatment), Treatment));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(EqualVarianceCaseSelected), EqualVarianceCaseSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(UnequalVarianceCaseSelected), UnequalVarianceCaseSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResidualsVsPredictedPlotSelected), ResidualsVsPredictedPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalProbabilityPlotSelected), NormalProbabilityPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.ResponseTransformation = argHelper.ArgumentLoader(nameof(ResponseTransformation), ResponseTransformation);
            this.Treatment = argHelper.ArgumentLoader(nameof(Treatment), Treatment);
            this.EqualVarianceCaseSelected = argHelper.ArgumentLoader(nameof(EqualVarianceCaseSelected), EqualVarianceCaseSelected);
            this.UnequalVarianceCaseSelected = argHelper.ArgumentLoader(nameof(UnequalVarianceCaseSelected), UnequalVarianceCaseSelected);
            this.ResidualsVsPredictedPlotSelected = argHelper.ArgumentLoader(nameof(ResidualsVsPredictedPlotSelected), ResidualsVsPredictedPlotSelected);
            this.NormalProbabilityPlotSelected = argHelper.ArgumentLoader(nameof(NormalProbabilityPlotSelected), NormalProbabilityPlotSelected);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
        }

        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(Response))); //4
            arguments.Append(" " + "\"" + ResponseTransformation + "\""); //5
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(Treatment))); //6          

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(EqualVarianceCaseSelected)); //7
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(UnequalVarianceCaseSelected)); //8
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ResidualsVsPredictedPlotSelected)); //9
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(NormalProbabilityPlotSelected)); //10
            arguments.Append(" " + Significance); //11

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