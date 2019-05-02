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
    public class LinearRegressionAnalysisModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Categorical factors")]
        public IEnumerable<string> CategoricalFactors { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Other design (block) factors")]
        public IEnumerable<string> OtherDesignFactors { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Continuous factors")]
        public IEnumerable<string> ContinuousFactors { get; set; }

        [DisplayName("Continuous factors transformation")]
        public string ContinuousFactorsTransformation { get; set; } = "None";

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [CheckUsedOnceOnly]
        [DisplayName("Covariates")]
        public IEnumerable<string> Covariates { get; set; }

        [DisplayName("Primary factor")]
        public string PrimaryFactor { get; set; }

        [DisplayName("Covariate transformation")]
        public string CovariateTransformation { get; set; } = "None";


        [DisplayName("ANOVA table")]
        public bool ANOVASelected { get; set; } = true;

        [DisplayName("Coefficients")]
        public bool Coefficients { get; set; } = true;

        [DisplayName("Adjusted R-squared")]
        public bool AdjustedRSquared { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [DisplayName("Residuals vs predicted plot")]
        public bool ResidualsVsPredictedPlot { get; set; }

        [DisplayName("Normal probability plot")]
        public bool NormalProbabilityPlot { get; set; }

        [DisplayName("Cook's distance plot")]
        public bool CooksDistancePlot { get; set; }

        [DisplayName("Leverage plot")]
        public bool LeveragePlot { get; set; }


        public LinearRegressionAnalysisModel() : base("LinearRegressionAnalysis") { }

        public LinearRegressionAnalysisModel(IDataset dataset)
            : base(dataset, "LinearRegressionAnalysis") { }


        public override ValidationInfo Validate()
        {
            LinearRegressionAnalysisValidator linearRegressionAnalysisValidator = new LinearRegressionAnalysisValidator(this);
            return linearRegressionAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && !ContinuousFactors.Contains(columnName) && (CategoricalFactors == null || !CategoricalFactors.Contains(columnName)) && (OtherDesignFactors == null || !OtherDesignFactors.Contains(columnName)) && (Covariates == null || !Covariates.Contains(columnName)))
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

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
            if (CategoricalFactors != null)
            {
                foreach (string factor in CategoricalFactors)
                {
                    if (dtNew.CheckIsNumeric(factor))
                    {
                        foreach (DataRow row in dtNew.Rows)
                        {
                            row[factor] = "'" + row[factor] + "'";
                        }
                    }
                }
            }

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


        public override string GetCommandLineArguments()
        {
            ArgumentFormatter argFormatter = new ArgumentFormatter();
            StringBuilder arguments = new StringBuilder();

            //first thing to do is to assemble the model (use the GetModel method)
            arguments.Append(" " + argFormatter.GetFormattedArgument(GetModel(), true)); //4

            //assemble a model for the covariate plot (if a covariate has been chosen)...
            arguments.Append(" " + argFormatter.GetFormattedArgument(Covariates)); //5

            //get transforms
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false)); //6

            arguments.Append(" " + argFormatter.GetFormattedArgument(CovariateTransformation, false)); //7

            arguments.Append(" " + argFormatter.GetFormattedArgument(PrimaryFactor, true)); //8

            arguments.Append(" " + argFormatter.GetFormattedArgument(CategoricalFactors)); //9

            arguments.Append(" " + argFormatter.GetFormattedArgument(ContinuousFactors)); //10

            arguments.Append(" " + argFormatter.GetFormattedArgument(ContinuousFactorsTransformation, false));//11

            arguments.Append(" " + argFormatter.GetFormattedArgument(OtherDesignFactors)); //12

            arguments.Append(" " + argFormatter.GetFormattedArgument(ANOVASelected)); //13
            arguments.Append(" " + argFormatter.GetFormattedArgument(Coefficients)); //14
            arguments.Append(" " + argFormatter.GetFormattedArgument(AdjustedRSquared)); //15

            arguments.Append(" " + Significance); //16

            arguments.Append(" " + argFormatter.GetFormattedArgument(ResidualsVsPredictedPlot)); //17

            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalProbabilityPlot)); //18

            arguments.Append(" " + argFormatter.GetFormattedArgument(CooksDistancePlot)); //19

            arguments.Append(" " + argFormatter.GetFormattedArgument(LeveragePlot)); //20

            return arguments.ToString().Trim();
        }

        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.CategoricalFactors = argHelper.LoadIEnumerableArgument(nameof(CategoricalFactors));
            this.OtherDesignFactors = argHelper.LoadIEnumerableArgument(nameof(OtherDesignFactors));
            this.ContinuousFactors = argHelper.LoadIEnumerableArgument(nameof(ContinuousFactors));
            this.ContinuousFactorsTransformation = argHelper.LoadStringArgument(nameof(ContinuousFactorsTransformation));
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.Covariates = argHelper.LoadIEnumerableArgument(nameof(Covariates));
            this.PrimaryFactor = argHelper.LoadStringArgument(nameof(PrimaryFactor));
            this.CovariateTransformation = argHelper.LoadStringArgument(nameof(CovariateTransformation));
            this.ANOVASelected = argHelper.LoadBooleanArgument(nameof(ANOVASelected));
            this.Coefficients = argHelper.LoadBooleanArgument(nameof(Coefficients));
            this.AdjustedRSquared = argHelper.LoadBooleanArgument(nameof(AdjustedRSquared));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
            this.ResidualsVsPredictedPlot = argHelper.LoadBooleanArgument(nameof(ResidualsVsPredictedPlot));
            this.NormalProbabilityPlot = argHelper.LoadBooleanArgument(nameof(NormalProbabilityPlot));
            this.CooksDistancePlot = argHelper.LoadBooleanArgument(nameof(CooksDistancePlot));
            this.LeveragePlot = argHelper.LoadBooleanArgument(nameof(LeveragePlot));
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CategoricalFactors), CategoricalFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OtherDesignFactors), OtherDesignFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ContinuousFactors), ContinuousFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ContinuousFactorsTransformation), ContinuousFactorsTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariates), Covariates));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PrimaryFactor), PrimaryFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CovariateTransformation), CovariateTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ANOVASelected), ANOVASelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Coefficients), Coefficients));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(AdjustedRSquared), AdjustedRSquared));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResidualsVsPredictedPlot), ResidualsVsPredictedPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalProbabilityPlot), NormalProbabilityPlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CooksDistancePlot), CooksDistancePlot));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LeveragePlot), LeveragePlot));

            return args;
        }



        private string GetModel()
        {
            //assemble the model from the information in the treatment, other factors, response and covariate boxes
            string model = Response + "~";

            if (CategoricalFactors != null)
            {
                model = model + String.Join('+', CategoricalFactors) + '+';
            }

            if (OtherDesignFactors != null)
            {
                model = model + String.Join('+', OtherDesignFactors);
            }

            return model.TrimEnd('+'); //trim end because might have cat factors but no other design factors
        }
    }
}