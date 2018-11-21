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
    public class PairedTTestAnalysisModel : AnalysisModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [DisplayName("Reponse transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Treatment")]
        public string Treatment { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Subject")]
        public string Subject { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Other design (block) factors")]
        public IEnumerable<string> OtherDesignFactors { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Covariates")]
        public IEnumerable<string> Covariates { get; set; }

        [DisplayName("Covariate transformation")]
        public string CovariateTransformation { get; set; } = "None";

        public IEnumerable<string> CovariancesList
        {
            get { return new List<string>() { "Compound Symmetric", "Unstructured", "Autoregressive(1)" }; }
        }

        [DisplayName("Covariance")]
        public string Covariance { get; set; } = "Compound Symmetric";


        [DisplayName("ANOVA Table")]
        public bool ANOVASelected { get; set; } = true;

        [DisplayName("Predicted vs. Residuals Plot")]
        public bool PRPlotSelected { get; set; } = true;

        [DisplayName("Normal Probability Plot")]
        public bool NormalPlotSelected { get; set; }

        [DisplayName("Least Square (predicted) means")]
        public bool LSMeansSelected { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        [DisplayName("All Pairwise Comparisons")]
        public bool AllPairwiseComparisons { get; set; }


        [DisplayName("Control group")]
        public string ControlGroup { get; set; }

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }


        public PairedTTestAnalysisModel() : this(null) { }

        public PairedTTestAnalysisModel(IDataset dataset)
            : base(dataset, "PairedTTestAnalysis") { }



        public override ValidationInfo Validate()
        {
            PairedTTestAnalysisValidator pairedTTestAnalysisValidator = new PairedTTestAnalysisValidator(this);
            return pairedTTestAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && Treatment != columnName && (OtherDesignFactors == null || !OtherDesignFactors.Contains(columnName)) && Subject != columnName && (Covariates == null || !Covariates.Contains(columnName)))
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //ensure that all data is trimmed
            //dtNew.TrimAllDataInDataTable();

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
                    if (dtNew.CheckIsNumeric(odf))
                    {
                        foreach (DataRow row in dtNew.Rows)
                        {
                            row[odf] = "'" + row[odf] + "'";
                        }
                    }
                }
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

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.Treatment = argHelper.LoadStringArgument(nameof(Treatment));
            this.Subject = argHelper.LoadStringArgument(nameof(Subject));
            this.OtherDesignFactors = argHelper.LoadIEnumerableArgument(nameof(OtherDesignFactors));
            this.Covariates = argHelper.LoadIEnumerableArgument(nameof(Covariates));
            this.Covariance = argHelper.LoadStringArgument(nameof(Covariance));
            this.CovariateTransformation = argHelper.LoadStringArgument(nameof(CovariateTransformation));
            this.ANOVASelected = argHelper.LoadBooleanArgument(nameof(ANOVASelected));
            this.PRPlotSelected = argHelper.LoadBooleanArgument(nameof(PRPlotSelected));
            this.NormalPlotSelected = argHelper.LoadBooleanArgument(nameof(NormalPlotSelected));
            this.LSMeansSelected = argHelper.LoadBooleanArgument(nameof(LSMeansSelected));
            this.AllPairwiseComparisons = argHelper.LoadBooleanArgument(nameof(AllPairwiseComparisons));
            this.ControlGroup = argHelper.LoadStringArgument(nameof(ControlGroup));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
        }

        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            //first thing to do is to assemble the model (use the GetModel method)
            arguments.Append(" " + argFormatter.GetFormattedArgument(GetModel(), true)); //4

            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatment, true)); //5

            arguments.Append(" " + argFormatter.GetFormattedArgument(Subject, true)); //6

            //assemble a model for the covariate plot (if a covariate has been chosen)...
            arguments.Append(" " + argFormatter.GetFormattedArgument(Covariates)); //7

            arguments.Append(" " + argFormatter.GetFormattedArgument(Covariance, false)); //8

            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false)); //9

            arguments.Append(" " + argFormatter.GetFormattedArgument(CovariateTransformation, false)); //10

            arguments.Append(" " + argFormatter.GetFormattedArgument(OtherDesignFactors)); //11

            arguments.Append(" " + argFormatter.GetFormattedArgument(ANOVASelected)); //12
            arguments.Append(" " + argFormatter.GetFormattedArgument(PRPlotSelected)); //13
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalPlotSelected)); //14
            arguments.Append(" " + argFormatter.GetFormattedArgument(AllPairwiseComparisons)); //15

            arguments.Append(" " + argFormatter.GetFormattedArgument(ControlGroup, true)); //16

            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //17

            arguments.Append(" " + argFormatter.GetFormattedArgument(LSMeansSelected)); //18

            return arguments.ToString().Trim();
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
    }
}