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

        public IEnumerable<string> CovarianceList
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

        public IEnumerable<string> ControlGroupList { get; set; }

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
    }
}