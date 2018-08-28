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
        public string Response { get; set; }

        [DisplayName("Response Transformation")]
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

        [DisplayName("Other Design Factors")]
        [CheckUsedOnceOnly]
        public List<string> OtherDesignFactors { get; set; }

        [CheckUsedOnceOnly]
        public string Covariate { get; set; }

        [DisplayName("Covariate Transformation")]
        public string CovariateTransformation { get; set; } = "None";

        public List<string> CovarianceList
        {
            get { return new List<string>() { "Compound Symmetric", "Unstructured", "Autoregressive(1)" }; }
        }


        public string Covariance = "Compound Symmetric";


        [DisplayName("ANOVA Table")]
        public bool ANOVASelected { get; set; } = true;

        [DisplayName("Predicted vs. Residuals Plot")]
        public bool PRPlotSelected { get; set; } = true;

        [DisplayName("Normal Probability Plot")]
        public bool NormalPlotSelected { get; set; }

        [DisplayName("Least Square (predicted) means")]
        public bool LSMeansSelected { get; set; }

        [Display(Name = "Significance")]
        public string Significance { get; set; } = "0.05";

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
            foreach (string col in dtNew.GetVariableNames())
            {
                if (Response != col && !Treatment.Contains(col) && OtherDesignFactors != null && !OtherDesignFactors.Contains(col) && Subject != col && Covariate != col)
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

            if (!String.IsNullOrEmpty(Covariate)) //check that a covariate is selected
            {
                dtNew.TransformColumn(Covariate, CovariateTransformation);
            }

            //Finally, as numeric categorical variables get misinterpreted by r, we need to go through
            //each column and put them in quotes...
            if (OtherDesignFactors != null)
            {
                foreach (string treat in OtherDesignFactors)
                {
                    if (dtNew.CheckIsNumeric(treat))
                    {
                        foreach (DataRow row in dtNew.Rows)
                        {
                            row[treat] = "'" + row[treat] + "'";
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
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariate), Covariate));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CovariateTransformation), CovariateTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariance), Covariance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ANOVASelected), ANOVASelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PRPlotSelected), PRPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalPlotSelected), NormalPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LSMeansSelected), LSMeansSelected));
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
            this.Covariate = argHelper.ArgumentLoader(nameof(Covariate), Covariate);
            this.Covariance = argHelper.ArgumentLoader(nameof(Covariance), Covariance);
            this.CovariateTransformation = argHelper.ArgumentLoader(nameof(CovariateTransformation), CovariateTransformation);
            this.ANOVASelected = argHelper.ArgumentLoader(nameof(ANOVASelected), ANOVASelected);
            this.PRPlotSelected = argHelper.ArgumentLoader(nameof(PRPlotSelected), PRPlotSelected);
            this.NormalPlotSelected = argHelper.ArgumentLoader(nameof(NormalPlotSelected), NormalPlotSelected);
            this.LSMeansSelected = argHelper.ArgumentLoader(nameof(LSMeansSelected), LSMeansSelected);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
        }

        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            //first thing to do is to assemble the model (use the GetModel method)
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(GetModel())); //4

            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(Treatment))); //5

            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(Subject))); //6

            //assemble a model for the covariate plot (if a covariate has been chosen)...
            if (String.IsNullOrEmpty(Covariate)) //7
            {
                arguments.Append(" " + "NULL");
            }
            else
            {
                string covariateModel = Response + "~" + Covariate;
                arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(covariateModel));
            }

            arguments.Append(" " + "\"" + Covariance + "\""); //8

            arguments.Append(" " + "\"" + ResponseTransformation + "\""); //9

            arguments.Append(" " + "\"" + CovariateTransformation + "\""); //10

            string blocks = null;
            if (OtherDesignFactors != null)
            {
                foreach (string s in OtherDesignFactors) blocks = blocks + "," + s;
            }

            if (String.IsNullOrEmpty(blocks)) //11
                arguments.Append(" " + "NULL");
            else
                arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(blocks.TrimStart(',')));

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ANOVASelected)); //12
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(PRPlotSelected)); //13
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(NormalPlotSelected)); //14

            arguments.Append(" " + Significance); //15

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(LSMeansSelected)); //16

            arguments.Append(" " + "N");

            return arguments.ToString();
        }

        private string GetModel()
        {
            //assemble the model from the information in the treatment, other factors, response and covariate boxes
            string model = Response + "~";

            if (!String.IsNullOrEmpty(Covariate))
                model = model + Covariate + "+";

            if (OtherDesignFactors != null)
            {
                foreach (string s in OtherDesignFactors)
                    model = model + s + "+";
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

                if (memberName != "Covariate")
                    checker.AddVar(this.Covariate);

                return checker.DoCheck(varToBeChecked);
            }
            else
            {
                return true;
            }
        }
    }
}