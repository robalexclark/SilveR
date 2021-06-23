using Combinatorics.Collections;
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
    public class LogisticRegressionAnalysisModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [Required]
        [DisplayName("Positive result")]
        public string PositiveResult { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Treatment factors")]
        public IEnumerable<string> Treatments { get; set; }

        [DisplayName("Other design (block) factors")]
        [CheckUsedOnceOnly]
        public IEnumerable<string> OtherDesignFactors { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Continuous factors")]
        public IEnumerable<string> ContinuousFactors { get; set; }

        [DisplayName("Continuous factors transformation")]
        public string ContinuousFactorsTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [CheckUsedOnceOnly]
        [DisplayName("Covariates")]
        public IEnumerable<string> Covariates { get; set; }

        [DisplayName("Covariate transformation")]
        public string CovariateTransformation { get; set; } = "None";

        [DisplayName("Residuals vs. predicted plot")]
        public bool TableOfOverallEffectTests { get; set; } = true;

        [DisplayName("Odds ratio")]
        public bool OddsRatio { get; set; } = true;

        [DisplayName("Model prediction assessment")]
        public bool ModelPredictionAssessment { get; set; } = true;

        [DisplayName("Plot of model predicted")]
        public bool PlotOfModelPredicted { get; set; } = true;

        [DisplayName("Table of model predictions")]
        public bool TableOfModelPredictions { get; set; } = true;

        [DisplayName("Goodness of fit test")]
        public bool GoodnessOfFitTest { get; set; } = true;

        [DisplayName("ROC Curve")]
        public bool ROCCurve { get; set; } = true;

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.025", "0.01", "0.001" }; }
        }


        public LogisticRegressionAnalysisModel() : base("LogisticRegressionAnalysis") { }

        public LogisticRegressionAnalysisModel(IDataset dataset)
            : base(dataset, "LogisticRegressionAnalysis") { }

        public override ValidationInfo Validate()
        {
            LogisticRegressionAnalysisValidator logisticRegressionAnalysisValidator = new LogisticRegressionAnalysisValidator(this);
            return logisticRegressionAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && (Treatments == null || !Treatments.Contains(columnName)) && (ContinuousFactors == null || !ContinuousFactors.Contains(columnName)) && (OtherDesignFactors == null || !OtherDesignFactors.Contains(columnName)) && (Covariates == null || !Covariates.Contains(columnName)))
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Need to create a new column for the scatterplot data as we have to combine any interaction effects into one column
            if (Treatments != null)
            {
                dtNew.CreateCombinedEffectColumn(Treatments, "scatterPlotColumn");
            }

            //Now do transformations...
            if (ContinuousFactors != null)
            {
                foreach (string continuousFactor in ContinuousFactors)
                {
                    dtNew.TransformColumn(continuousFactor, ContinuousFactorsTransformation);
                }
            }

            if (Covariates != null)
            {
                foreach (string covariate in Covariates)
                {
                    dtNew.TransformColumn(covariate, CovariateTransformation);
                }
            }

            //Finally, as numeric categorical variables get misinterpreted by r, we need to go through
            //each column and put them in quotes...
            if (Treatments != null)
            {
                foreach (string treat in Treatments)
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

            string scatterplotModel = argFormatter.GetFormattedArgument(Response + "~scatterPlotColumn", true);
            arguments.Append(" " + scatterplotModel); //5

            arguments.Append(" " + argFormatter.GetFormattedArgument(PositiveResult)); //6

            //assemble a model for the covariate plot (if a covariate has been chosen)...
            arguments.Append(" " + argFormatter.GetFormattedArgument(Covariates)); //7

            //get transforms
            arguments.Append(" " + argFormatter.GetFormattedArgument(CovariateTransformation, false)); //8

            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatments)); //9

            arguments.Append(" " + argFormatter.GetFormattedArgument(ContinuousFactors)); //10

            arguments.Append(" " + argFormatter.GetFormattedArgument(ContinuousFactorsTransformation, false)); //11

            arguments.Append(" " + argFormatter.GetFormattedArgument(OtherDesignFactors)); //12

            arguments.Append(" " + argFormatter.GetFormattedArgument(TableOfOverallEffectTests)); //13

            arguments.Append(" " + argFormatter.GetFormattedArgument(OddsRatio)); //14

            arguments.Append(" " + argFormatter.GetFormattedArgument(ModelPredictionAssessment)); //15

            arguments.Append(" " + argFormatter.GetFormattedArgument(PlotOfModelPredicted)); //16

            arguments.Append(" " + argFormatter.GetFormattedArgument(TableOfModelPredictions)); //17

            arguments.Append(" " + argFormatter.GetFormattedArgument(GoodnessOfFitTest)); //18

            arguments.Append(" " + argFormatter.GetFormattedArgument(ROCCurve)); //19

            arguments.Append(" " + Significance); //20

            return arguments.ToString().Trim();
        }


        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.PositiveResult = argHelper.LoadStringArgument(nameof(PositiveResult));
            this.Covariates = argHelper.LoadIEnumerableArgument(nameof(Covariates));
            this.CovariateTransformation = argHelper.LoadStringArgument(nameof(CovariateTransformation));
            this.Treatments = argHelper.LoadIEnumerableArgument(nameof(Treatments));
            this.ContinuousFactors = argHelper.LoadIEnumerableArgument(nameof(ContinuousFactors));
            this.ContinuousFactorsTransformation = argHelper.LoadStringArgument(nameof(ContinuousFactorsTransformation));
            this.OtherDesignFactors = argHelper.LoadIEnumerableArgument(nameof(OtherDesignFactors));
            this.TableOfOverallEffectTests = argHelper.LoadBooleanArgument(nameof(TableOfOverallEffectTests));
            this.OddsRatio = argHelper.LoadBooleanArgument(nameof(OddsRatio));
            this.ModelPredictionAssessment = argHelper.LoadBooleanArgument(nameof(ModelPredictionAssessment));
            this.PlotOfModelPredicted = argHelper.LoadBooleanArgument(nameof(PlotOfModelPredicted));
            this.TableOfModelPredictions = argHelper.LoadBooleanArgument(nameof(TableOfModelPredictions));
            this.GoodnessOfFitTest = argHelper.LoadBooleanArgument(nameof(GoodnessOfFitTest));
            this.ROCCurve = argHelper.LoadBooleanArgument(nameof(ROCCurve));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PositiveResult), PositiveResult));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariates), Covariates));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CovariateTransformation), CovariateTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatments), Treatments));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ContinuousFactors), ContinuousFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ContinuousFactorsTransformation), ContinuousFactorsTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OtherDesignFactors), OtherDesignFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(TableOfOverallEffectTests), TableOfOverallEffectTests));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OddsRatio), OddsRatio));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ModelPredictionAssessment), ModelPredictionAssessment));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PlotOfModelPredicted), PlotOfModelPredicted));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(TableOfModelPredictions), TableOfModelPredictions));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(GoodnessOfFitTest), GoodnessOfFitTest));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ROCCurve), ROCCurve));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));

            return args;
        }


        private string GetModel()
        {
            //assemble the model from the information in the treatment, other factors, response and covariate boxes
            StringBuilder model = new StringBuilder(Response + "~");

            if (Covariates != null)
            {
                model.Append(String.Join('+', Covariates) + '+');
            }

            if (OtherDesignFactors != null)
            {
                model.Append(String.Join('+', OtherDesignFactors) + '+');
            }

            if (ContinuousFactors != null)
            {
                model.Append(String.Join('+', ContinuousFactors) + '+');
            }

            if (Treatments != null)
            {
                model.Append(String.Join('+', Treatments) + '+');

                //determine the interactions

                List<string> factors = new List<string>(Treatments);
                List<string> fullInteractions = DetermineInteractions(factors);
                foreach (string s in fullInteractions)
                {
                    model.Append(s.Replace(" * ", "*") + '+');
                }
            }

            return model.ToString().TrimEnd('+');
        }


        public List<string> DetermineInteractions(List<string> listToCreateInteractionsFrom)
        {
            List<string> interactions = new List<string>();

            //for each factor, determine the combinations
            for (int i = 2; i <= listToCreateInteractionsFrom.Count; i++)
            {
                Combinations<string> combinations = new Combinations<string>(listToCreateInteractionsFrom, i, GenerateOption.WithoutRepetition);

                //for each set of combinations we need to assemble the string, with each factor separated by a *
                foreach (IList<string> combination in combinations)
                {
                    string interaction = String.Join(" * ", combination);

                    //add the interaction to the list
                    interactions.Add(interaction);
                }
            }

            return interactions;
        }
    }
}