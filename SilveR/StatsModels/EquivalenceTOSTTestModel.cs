﻿using Combinatorics.Collections;
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
    public class EquivalenceTOSTTestModel : AnalysisDataModelBase
    {
        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [Required]
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

        [DisplayName("Residuals vs. predicted plot")]
        public bool PRPlotSelected { get; set; } = true;

        [DisplayName("Normal probability plot")]
        public bool NormalPlotSelected { get; set; }

        [DisplayName("Significance level")]
        public string Significance { get; set; } = "0.05";

        public IEnumerable<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.025", "0.01", "0.001" }; }
        }

        [DisplayName("Effect")]
        public string SelectedEffect { get; set; }

        [DisplayName("Least square (predicted) means")]
        public bool LSMeansSelected { get; set; }

        public enum ComparisonOption { AllPairwise = 0, ComparisonsToControl = 1 }
        [DisplayName("Analysis type")]
        public ComparisonOption ComparisonType { get; set; } = ComparisonOption.AllPairwise;

        [DisplayName("Control group")]
        public string ControlGroup { get; set; }

        public enum EquivalenceBoundsOption { Absolute = 0, Percentage = 1 }
        [DisplayName("Equivalence Bounds type")]
        public EquivalenceBoundsOption EquivalenceBoundsType { get; set; } = EquivalenceBoundsOption.Absolute;

        [DisplayName("Lower bound absolute")]
        public Nullable<decimal> LowerBoundAbsolute { get; set; } = 0;

        [DisplayName("Upper bound absolute")]
        public Nullable<decimal> UpperBoundAbsolute { get; set; } = 0;

        [DisplayName("Lower bound percentage")]
        public Nullable<decimal> LowerBoundPercentage { get; set; } = 0;

        [DisplayName("Upper bound percentage")]
        public Nullable<decimal> UpperBoundPercentage { get; set; } = 0;

        public EquivalenceTOSTTestModel() : base("EquivalenceTOSTTest") { }

        public EquivalenceTOSTTestModel(IDataset dataset)
            : base(dataset, "EquivalenceTOSTTest") { }

        public override ValidationInfo Validate()
        {
            EquivalenceTOSTTestValidator equivalenceTOSTTestValidator = new EquivalenceTOSTTestValidator(this);
            return equivalenceTOSTTestValidator.Validate();
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

            //Generate a "catfact" column from the CatFactors (used only in R)!
            DataColumn catFactor = new DataColumn("catfact");
            dtNew.Columns.Add(catFactor);

            //Need to create a new column for the scatterplot data as we have to combine any interaction effects into one column
            dtNew.CreateCombinedEffectColumn(Treatments, "scatterPlotColumn");

            //If an interaction effect is selected then we need to combine values into single column
            if (!String.IsNullOrEmpty(SelectedEffect))
            {
                //create a new column and add it to the table
                if (SelectedEffect.Contains(" * ")) //then it is an interaction effect so we need to combine values from different columns
                {
                    char[] splitChar = { '*' };
                    string[] effects = SelectedEffect.Split(splitChar, StringSplitOptions.RemoveEmptyEntries); //get the effect names that make up the interaction effect

                    dtNew.CreateCombinedEffectColumn(effects, "mainEffect");
                }
                else //just copy the column selected in the dropdown
                {
                    DataColumn mainEffect = new DataColumn("mainEffect");
                    dtNew.Columns.Add(mainEffect);
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

            arguments.Append(" " + argFormatter.GetFormattedArgument(EquivalenceBoundsType.ToString())); //12

            if (EquivalenceBoundsType == EquivalenceBoundsOption.Absolute)
            {
                arguments.Append(" " + argFormatter.GetFormattedArgument(LowerBoundAbsolute)); //13
                arguments.Append(" " + argFormatter.GetFormattedArgument(UpperBoundAbsolute)); //14
            }
            else
            {
                arguments.Append(" " + argFormatter.GetFormattedArgument(LowerBoundPercentage)); //13
                arguments.Append(" " + argFormatter.GetFormattedArgument(UpperBoundPercentage)); //14
            }

            arguments.Append(" " + argFormatter.GetFormattedArgument(PRPlotSelected)); //15
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalPlotSelected)); //16

            arguments.Append(" " + Significance); //17

            //assemble the effect model
            if (String.IsNullOrEmpty(SelectedEffect)) //18, 19
            {
                arguments.Append(" " + "NULL");
                arguments.Append(" " + "NULL");
            }
            else
            {
                arguments.Append(" " + argFormatter.GetFormattedArgument(GetEffectModel(), true));
                arguments.Append(" " + argFormatter.GetFormattedArgument(SelectedEffect, true));
            }

            arguments.Append(" " + argFormatter.GetFormattedArgument(LSMeansSelected)); //20

            arguments.Append(" " + argFormatter.GetFormattedArgument(ComparisonType.ToString())); //21

            arguments.Append(" " + argFormatter.GetFormattedArgument(ControlGroup, false)); //22

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
            this.EquivalenceBoundsType = (EquivalenceBoundsOption)Enum.Parse(typeof(EquivalenceBoundsOption), argHelper.LoadStringArgument(nameof(EquivalenceBoundsType)), true);
            this.LowerBoundAbsolute = argHelper.LoadNullableDecimalArgument(nameof(LowerBoundAbsolute));
            this.UpperBoundAbsolute = argHelper.LoadNullableDecimalArgument(nameof(UpperBoundAbsolute));
            this.LowerBoundPercentage = argHelper.LoadNullableDecimalArgument(nameof(LowerBoundPercentage));
            this.UpperBoundPercentage = argHelper.LoadNullableDecimalArgument(nameof(UpperBoundPercentage));
            this.PRPlotSelected = argHelper.LoadBooleanArgument(nameof(PRPlotSelected));
            this.NormalPlotSelected = argHelper.LoadBooleanArgument(nameof(NormalPlotSelected));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
            this.SelectedEffect = argHelper.LoadStringArgument(nameof(SelectedEffect));
            this.LSMeansSelected = argHelper.LoadBooleanArgument(nameof(LSMeansSelected));
            this.ComparisonType = (ComparisonOption)Enum.Parse(typeof(ComparisonOption), argHelper.LoadStringArgument(nameof(ComparisonType)), true);
            this.ControlGroup = argHelper.LoadStringArgument(nameof(ControlGroup));
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
            args.Add(ArgumentHelper.ArgumentFactory(nameof(EquivalenceBoundsType), EquivalenceBoundsType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LowerBoundAbsolute), LowerBoundAbsolute));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(UpperBoundAbsolute), UpperBoundAbsolute));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LowerBoundPercentage), LowerBoundPercentage));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(UpperBoundPercentage), UpperBoundPercentage));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PRPlotSelected), PRPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalPlotSelected), NormalPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SelectedEffect), SelectedEffect));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LSMeansSelected), LSMeansSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ComparisonType), ComparisonType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ControlGroup), ControlGroup));

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

            model.Append(String.Join('+', Treatments) + '+');

            //determine the interactions
            List<string> factors = new List<string>(Treatments);
            List<string> fullInteractions = DetermineInteractions(factors);
            foreach (string s in fullInteractions)
            {
                model.Append(s.Replace(" * ", "*") + '+');
            }

            return model.ToString().TrimEnd('+');
        }

        private string GetEffectModel()
        {
            //assemble the effect model
            StringBuilder effectModel = new StringBuilder(Response + "~"); //add in the response

            if (Covariates != null)
            {
                effectModel.Append(String.Join('+', Covariates) + '+');
            }

            if (OtherDesignFactors != null)
            {
                effectModel.Append(String.Join('+', OtherDesignFactors) + '+');
            }

            //complicated business of assembling the other part of the model from the interactions etc...
            string[] splitter = { " * " };
            List<string> interactionEffects = new List<string>(SelectedEffect.Split(splitter, StringSplitOptions.RemoveEmptyEntries));

            foreach (string treat in Treatments)
            {
                if (!interactionEffects.Contains(treat))
                {
                    effectModel.Append(treat.Trim() + "+");
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
                foreach (IList<string> combination in combinations)
                {
                    string interaction = String.Join(" * ", combination);

                    //add the interaction to the list
                    interactions.Add(interaction);
                }
            }

            return interactions;
        }

        //public static List<string> DetermineSelectedEffectsList(List<string> selectedTreatments)
        //{
        //    //assemble a complete list of main and interaction effects
        //    List<string> effects = new List<string>();
        //    effects.AddRange(selectedTreatments);

        //    List<string> interactions = DetermineInteractions(selectedTreatments);

        //    effects.AddRange(interactions);

        //    //if the number of interaction effects is 4 or greater,
        //    //then only the main effects and highest order effect are to be available
        //    if (selectedTreatments.Count >= 4)
        //    {
        //        //remove any effect that is an interaction effect
        //        for (int i = effects.Count - 1; i >= 0; i = i - 1)
        //        {
        //            if (effects[i].Contains("*")) effects.Remove(effects[i]);
        //        }
        //        //add in the highest order interaction again
        //        effects.Add(interactions[interactions.Count - 1].ToString());
        //    }

        //    return effects;
        //}
    }
}