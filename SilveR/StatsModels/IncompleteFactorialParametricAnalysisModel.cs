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
    public class IncompleteFactorialParametricAnalysisModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [HasAtLeastOneEntry]
        [CheckUsedOnceOnly]
        [DisplayName("Treatment factors")]
        public IEnumerable<string> Treatments { get; set; }

        [DisplayName("Other design (block) factors")]
        [CheckUsedOnceOnly]
        public IEnumerable<string> OtherDesignFactors { get; set; }

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

        [DisplayName("Predicted vs. residuals plot")]
        public bool PRPlotSelected { get; set; } = true;

        [DisplayName("Normal probability plot")]
        public bool NormalPlotSelected { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [DisplayName("Effect")]
        public string SelectedEffect { get; set; }

        [DisplayName("Least square (predicted) means")]
        public bool LSMeansSelected { get; set; }

        [DisplayName("All pairwise comparisons")]
        public string AllPairwise { get; set; }

        public IEnumerable<string> PairwiseTestList
        {
            get { return new List<string>() { String.Empty, "Unadjusted (LSD)", "Holm", "Hochberg", "Hommel", "Bonferroni", "Benjamini-Hochberg" }; }
        }


        public IncompleteFactorialParametricAnalysisModel() : base("IncompleteFactorialParametricAnalysis") { }

        public IncompleteFactorialParametricAnalysisModel(IDataset dataset)
            : base(dataset, "IncompleteFactorialParametricAnalysis") { }


        public override ValidationInfo Validate()
        {
            IncompleteFactorialParametricAnalysisValidator singleMeasuresParametricAnalysisValidator = new IncompleteFactorialParametricAnalysisValidator(this);
            return singleMeasuresParametricAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && !Treatments.Contains(columnName) && (OtherDesignFactors == null || !OtherDesignFactors.Contains(columnName)) && (Covariates == null || !Covariates.Contains(columnName)))
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Generate a "catfact" column from the CatFactors!
            DataColumn catFactor = new DataColumn("catfact");
            dtNew.Columns.Add(catFactor);


            //Need to create a new column for the scatterplot data as we have to combine any interaction effects into one column
            DataColumn scatterPlot = new DataColumn("scatterPlotColumn");
            dtNew.Columns.Add(scatterPlot);
            foreach (DataRow r in dtNew.Rows) //go through each row...
            {
                string combinedEffectValue = null;
                foreach (string s in Treatments) //combine the values from each column into one string
                {
                    combinedEffectValue = combinedEffectValue + " " + r[s.Trim()];
                }

                r["scatterPlotColumn"] = combinedEffectValue.Trim(); //copy the new value to the new column
            }

            //If an interaction effect is selected then we need to combine values into single column
            if (!String.IsNullOrEmpty(SelectedEffect))
            {
                //create a new column and add it to the table
                DataColumn mainEffect = new DataColumn("mainEffect");
                dtNew.Columns.Add(mainEffect);

                if (SelectedEffect.Contains(" * ")) //then it is an interaction effect so we need to combine values from different columns
                {
                    char[] splitChar = { '*' };
                    string[] effects = SelectedEffect.Split(splitChar, StringSplitOptions.RemoveEmptyEntries); //get the effect names that make up the interaction effect

                    foreach (DataRow r in dtNew.Rows) //go through each row...
                    {
                        string combinedEffectValue = null;
                        foreach (string s in effects) //combine the values from each column into one string
                        {
                            combinedEffectValue = combinedEffectValue + " " + r[s.Trim()];
                        }

                        r["mainEffect"] = combinedEffectValue.Trim(); //copy the new value to the new column
                    }
                }
                else //just copy the column selected in the dropdown
                {
                    foreach (DataRow r in dtNew.Rows)
                    {
                        r["mainEffect"] = r[SelectedEffect].ToString();
                    }
                }
            }

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

            //assemble a model for the covariate plot (if a covariate has been chosen)...
            arguments.Append(" " + argFormatter.GetFormattedArgument(Covariates)); //6

            //get transforms
            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false)); //7

            arguments.Append(" " + argFormatter.GetFormattedArgument(CovariateTransformation, false)); //8

            arguments.Append(" " + argFormatter.GetFormattedArgument(PrimaryFactor, true)); //9

            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatments)); //10

            arguments.Append(" " + argFormatter.GetFormattedArgument(OtherDesignFactors)); //11

            arguments.Append(" " + argFormatter.GetFormattedArgument(ANOVASelected)); //12
            arguments.Append(" " + argFormatter.GetFormattedArgument(PRPlotSelected)); //13
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalPlotSelected)); //14

            arguments.Append(" " + Significance); //15

            //assemble the effect model
            if (String.IsNullOrEmpty(SelectedEffect)) //16, 17
            {
                arguments.Append(" " + "NULL");
                arguments.Append(" " + "NULL");
            }
            else
            {
                arguments.Append(" " + argFormatter.GetFormattedArgument(GetEffectModel(), true));
                arguments.Append(" " + argFormatter.GetFormattedArgument(SelectedEffect, true));
            }

            arguments.Append(" " + argFormatter.GetFormattedArgument(LSMeansSelected)); //18

            arguments.Append(" " + argFormatter.GetFormattedArgument(AllPairwise, false)); //19

            return arguments.ToString().Trim();
        }


        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.Treatments = argHelper.LoadIEnumerableArgument(nameof(Treatments));
            this.OtherDesignFactors = argHelper.LoadIEnumerableArgument(nameof(OtherDesignFactors));
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.Covariates = argHelper.LoadIEnumerableArgument(nameof(Covariates));
            this.PrimaryFactor = argHelper.LoadStringArgument(nameof(PrimaryFactor));
            this.CovariateTransformation = argHelper.LoadStringArgument(nameof(CovariateTransformation));
            this.ANOVASelected = argHelper.LoadBooleanArgument(nameof(ANOVASelected));
            this.PRPlotSelected = argHelper.LoadBooleanArgument(nameof(PRPlotSelected));
            this.NormalPlotSelected = argHelper.LoadBooleanArgument(nameof(NormalPlotSelected));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
            this.SelectedEffect = argHelper.LoadStringArgument(nameof(SelectedEffect));
            this.LSMeansSelected = argHelper.LoadBooleanArgument(nameof(LSMeansSelected));
            this.AllPairwise = argHelper.LoadStringArgument(nameof(AllPairwise));
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatments), Treatments));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OtherDesignFactors), OtherDesignFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariates), Covariates));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PrimaryFactor), PrimaryFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CovariateTransformation), CovariateTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ANOVASelected), ANOVASelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PRPlotSelected), PRPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalPlotSelected), NormalPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SelectedEffect), SelectedEffect));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LSMeansSelected), LSMeansSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(AllPairwise), AllPairwise));

            return args;
        }


        private string GetModel()
        {
            //assemble the model from the information in the treatment, other factors, response and covariate boxes
            string model = Response + "~";

            if (Covariates != null)
            {
                foreach (string covariate in Covariates)
                {
                    model = model + covariate + "+";
                }
            }

            if (OtherDesignFactors != null)
            {
                foreach (string otherDesign in OtherDesignFactors)
                {
                    model = model + otherDesign + "+";
                }
            }

            foreach (string treat in Treatments)
            {
                model = model + treat + "+";
            }

            //determine the interactions
            List<string> factors = new List<string>(Treatments);
            IEnumerable<string> fullInteractions = DetermineInteractions(factors);
            foreach (string s in fullInteractions)
            {
                model = model + s.Replace(" * ", "*") + "+";
            }

            model = model.TrimEnd('+');

            return model;
        }

        public string GetEffectModel()
        {
            //assemble the effect model
            string effectModel = Response + "~"; //add in the response

            if (Covariates != null)
            {
                foreach (string covariate in Covariates) //add in a covariate if one is selected
                    effectModel = effectModel + covariate + "+";
            }

            if (OtherDesignFactors != null)
            {
                foreach (string otherDesign in OtherDesignFactors) //add in blocking factors if they are selected
                    effectModel = effectModel + otherDesign + "+";
            }

            //complicated business of assembling the other part of the model from the interactions etc...
            string[] splitter = { " * " };
            List<string> interactionEffects = new List<string>(SelectedEffect.Split(splitter, StringSplitOptions.RemoveEmptyEntries));

            foreach (string treat in Treatments)
            {
                if (!interactionEffects.Contains(treat))
                {
                    effectModel = effectModel + treat.Trim() + "+";
                }
            }

            return effectModel + "mainEffect"; //where maineffect is the combined effect column of the selected effect in the dataset
        }

        public static List<string> DetermineInteractions(List<string> listToCreateInteractionsFrom)
        {
            List<string> interactions = new List<string>();

            //for each factor, determine the combinations
            for (int i = 2; i <= listToCreateInteractionsFrom.Count; i++)
            {
                Combinations<string> combinations = new Combinations<string>(listToCreateInteractionsFrom, i, GenerateOption.WithoutRepetition);

                //for each set of combinations we need to assemble the string, with each factor separated by a *
                foreach (IList<string> c in combinations)
                {
                    string interaction = null;

                    foreach (string s in c)
                    {
                        interaction = interaction + " * " + s;
                    }

                    interaction = interaction.Remove(0, 3); //remove the first *

                    //add the interaction to the list
                    interactions.Add(interaction);
                }
            }

            return interactions;
        }

        public static List<string> DetermineSelectedEffectsList(List<string> selectedTreatments)
        {
            //get a list and add in the highest order interaction
            List<string> effects = new List<string>();

            List<string> interactions = DetermineInteractions(selectedTreatments);

            if (interactions.Any())
            {
                effects.Add(interactions.Last());
            }

            return effects;
        }
    }
}