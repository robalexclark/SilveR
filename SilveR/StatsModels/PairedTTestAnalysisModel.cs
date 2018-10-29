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
    public class PairedTTestAnalysisModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "PairedTTestAnalysis"; } }

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
        [Required]
        [CheckUsedOnceOnly]
        public string Response { get; set; }

        [DisplayName("Reponse transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public List<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [Display(Name = "Treatment")]
        [Required]
        [CheckUsedOnceOnly]
        public string Treatment { get; set; }

        [Display(Name = "Subject")]
        [Required]
        [CheckUsedOnceOnly]
        public string Subject { get; set; }

        [DisplayName("Other design (block) factors")]
        [CheckUsedOnceOnly]
        public List<string> OtherDesignFactors { get; set; }

        [DisplayName("Covariates")]
        [CheckUsedOnceOnly]
        public List<string> Covariates { get; set; }

        [DisplayName("Covariate transformation")]
        public string CovariateTransformation { get; set; } = "None";

        public List<string> CovarianceList
        {
            get { return new List<string>() { "Compound Symmetric", "Unstructured", "Autoregressive(1)" }; }
        }

        public string Covariance { get; set; } = "Compound Symmetric";


        [DisplayName("ANOVA Table")]
        public bool ANOVASelected { get; set; } = true;

        [DisplayName("Predicted vs. Residuals Plot")]
        public bool PRPlotSelected { get; set; } = true;

        [DisplayName("Normal Probability Plot")]
        public bool NormalPlotSelected { get; set; }

        [DisplayName("Least Square (predicted) means")]
        public bool LSMeansSelected { get; set; }

        [Display(Name = "Significance level")]
        public string Significance { get; set; } = "0.05";

        [DisplayName("All Pairwise Comparisons")]
        public bool AllPairwiseComparisons { get; set; }


        [DisplayName("Control group")]
        public string ControlGroup { get; set; }

        public List<string> ControlGroupList { get; set; }

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }


        public PairedTTestAnalysisModel() { }

        public PairedTTestAnalysisModel(Dataset dataset)
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
            PairedTTestAnalysisValidator pairedTTestAnalysisValidator = new PairedTTestAnalysisValidator(this);
            return pairedTTestAnalysisValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && Treatment != columnName && (OtherDesignFactors == null || !OtherDesignFactors.Contains(columnName)) && Subject != columnName && (Covariates == null || !Covariates.Contains(columnName)))
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Now do transformations...
            dtNew.TransformColumn(Response, ResponseTransformation);

            if (Covariates != null)
            {
                foreach (string covariate in Covariates)
                {
                    dtNew.TransformColumn(covariate, CovariateTransformation);
                }
            }

            //Finally, as numeric categorical variables get misinterpreted by r, we need to go through
            //each column and put them in quotes...
            if (OtherDesignFactors != null)
            {
                foreach (string odf in OtherDesignFactors)
                {
                    if (dtNew.ColumnIsNumeric(odf))
                    {
                        foreach (DataRow row in dtNew.Rows)
                        {
                            row[odf] = "'" + row[odf] + "'";
                        }
                    }
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
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Subject), Subject));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OtherDesignFactors), OtherDesignFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariates), Covariates));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CovariateTransformation), CovariateTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariance), Covariance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ANOVASelected), ANOVASelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PRPlotSelected), PRPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalPlotSelected), NormalPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LSMeansSelected), LSMeansSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(AllPairwiseComparisons), AllPairwiseComparisons));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ControlGroup), ControlGroup));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }

        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.ResponseTransformation = argHelper.ArgumentLoader(nameof(ResponseTransformation), ResponseTransformation);
            this.Treatment = argHelper.ArgumentLoader(nameof(Treatment), Treatment);
            this.Subject = argHelper.ArgumentLoader(nameof(Subject), Subject);
            this.OtherDesignFactors = argHelper.ArgumentLoader(nameof(OtherDesignFactors), OtherDesignFactors);
            this.Covariates = argHelper.ArgumentLoader(nameof(Covariates), Covariates);
            this.Covariance = argHelper.ArgumentLoader(nameof(Covariance), Covariance);
            this.CovariateTransformation = argHelper.ArgumentLoader(nameof(CovariateTransformation), CovariateTransformation);
            this.ANOVASelected = argHelper.ArgumentLoader(nameof(ANOVASelected), ANOVASelected);
            this.PRPlotSelected = argHelper.ArgumentLoader(nameof(PRPlotSelected), PRPlotSelected);
            this.NormalPlotSelected = argHelper.ArgumentLoader(nameof(NormalPlotSelected), NormalPlotSelected);
            this.LSMeansSelected = argHelper.ArgumentLoader(nameof(LSMeansSelected), LSMeansSelected);
            this.AllPairwiseComparisons = argHelper.ArgumentLoader(nameof(AllPairwiseComparisons), AllPairwiseComparisons);
            this.ControlGroup = argHelper.ArgumentLoader(nameof(ControlGroup), ControlGroup);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
        }

        public string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            //first thing to do is to assemble the model (use the GetModel method)
            arguments.Append(" " + argFormatter.GetFormattedArgument(GetModel(), true)); //4

            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatment, true)); //5

            arguments.Append(" " + argFormatter.GetFormattedArgument(Subject, true)); //6

            //assemble a model for the covariate plot (if a covariate has been chosen)...
            arguments.Append(" " + argFormatter.GetFormattedArgument(Covariates)); //7

            arguments.Append(" " + argFormatter.GetFormattedArgument(Covariance)); //8

            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation)); //9

            arguments.Append(" " + argFormatter.GetFormattedArgument(CovariateTransformation)); //10

            arguments.Append(" " + argFormatter.GetFormattedArgument(OtherDesignFactors)); //11

            arguments.Append(" " + argFormatter.GetFormattedArgument(ANOVASelected)); //12
            arguments.Append(" " + argFormatter.GetFormattedArgument(PRPlotSelected)); //13
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalPlotSelected)); //14
            arguments.Append(" " + argFormatter.GetFormattedArgument(AllPairwiseComparisons)); //15

            arguments.Append(" " + argFormatter.GetFormattedArgument(ControlGroup, true)); //16

            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance)); //17

            arguments.Append(" " + argFormatter.GetFormattedArgument(LSMeansSelected)); //18

            return arguments.ToString();
        }

        private string GetModel()
        {
            //assemble the model from the information in the treatment, other factors, response and covariate boxes
            string model = Response + "~";

            if (Covariates != null)
                foreach (string covariate in Covariates)
                    model = model + covariate + "+";

            if (OtherDesignFactors != null)
            {
                foreach (string otherDesign in OtherDesignFactors)
                    model = model + otherDesign + "+";
            }

            if (!String.IsNullOrEmpty(Treatment))
                model = model + "Timezzz+"; //Time model needs zzz adding on the end for R to be able to recognise Time var

            model = model.TrimEnd('+');

            return model;
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

                if (memberName != "Subject")
                    checker.AddVar(this.Subject);

                if (memberName != "OtherDesignFactors")
                    checker.AddVars(this.OtherDesignFactors);

                if (memberName != "Covariates")
                    checker.AddVars(this.Covariates);

                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}