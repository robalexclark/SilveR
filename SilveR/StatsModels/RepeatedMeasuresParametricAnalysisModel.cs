using Combinatorics;
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
    public class RepeatedMeasuresParametricAnalysisModel : AnalysisDataModelBase
    {
        private const string MAIN_EFFECT_SEPERATOR = "_.._";

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Response")]
        public string Response { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Treatment factors")]
        public IEnumerable<string> Treatments { get; set; }

        [CheckUsedOnceOnly]
        [DisplayName("Other design (block) factors")]
        public IEnumerable<string> OtherDesignFactors { get; set; }

        [Required]
        [CheckUsedOnceOnly]
        [DisplayName("Repeated factor")]
        public string RepeatedFactor { get; set; }

        [CheckUsedOnceOnly]
        [Required]
        [DisplayName("Subject factor")]
        public string Subject { get; set; }

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public IEnumerable<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        public IEnumerable<string> CovariancesList
        {
            get { return new List<string>() { "Compound Symmetric", "Unstructured", "Autoregressive(1)" }; }
        }

        [DisplayName("Covariance")]
        public string Covariance { get; set; } = "Compound Symmetric";

        [CheckUsedOnceOnly]
        [DisplayName("Covariates")]
        public IEnumerable<string> Covariates { get; set; }

        [DisplayName("Primary factor")]
        public string PrimaryFactor { get; set; }

        [DisplayName("Covariate transformation")]
        public string CovariateTransformation { get; set; } = "None";

        [DisplayName("Table of overall effect tests")]
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

        public enum ComparisonOption { None = 0, AllComparisonsWithinSelected = 1, AllPairwiseComparisons = 2 }
        [DisplayName("Comparison type")]
        public ComparisonOption ComparisonType { get; set; } = ComparisonOption.None;

        [DisplayName("Generate comparisons dataset")]
        public bool GenerateComparisonsDataset { get; set; }

        public RepeatedMeasuresParametricAnalysisModel() : base("RepeatedMeasuresParametricAnalysis") { }

        public RepeatedMeasuresParametricAnalysisModel(IDataset dataset)
            : base(dataset, "RepeatedMeasuresParametricAnalysis") { }

        public override ValidationInfo Validate()
        {
            RepeatedMeasuresParametricAnalysisValidator repeatedMeasuresParametricAnalysisValidator = new RepeatedMeasuresParametricAnalysisValidator(this);
            return repeatedMeasuresParametricAnalysisValidator.Validate();
        }

        public override string[] ExportData()
        {
            DataTable dtNew = DataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && !Treatments.Contains(columnName) && (OtherDesignFactors == null || !OtherDesignFactors.Contains(columnName)) && RepeatedFactor != columnName && Subject != columnName && (Covariates == null || !Covariates.Contains(columnName)))
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);

            //Need to create a new column for "between" as we have to combine any interaction effects into one column
            dtNew.CreateCombinedEffectColumn(Treatments, "between");

            //Need to create a new column for "betweenwithin" as we have to combine any interaction effects into one column
            List<string> treatmentsAndRepeatedFactors = new List<string>(Treatments);
            if (RepeatedFactor != null)
                treatmentsAndRepeatedFactors.Add(RepeatedFactor); //add in time to the factors

            dtNew.CreateCombinedEffectColumn(treatmentsAndRepeatedFactors, "betweenwithin");

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
                        StringBuilder combinedEffectValue = new StringBuilder();
                        foreach (string s in effects) //combine the values from each column into one string
                        {
                            combinedEffectValue.Append(MAIN_EFFECT_SEPERATOR + r[s.Trim()]);
                        }

                        r["mainEffect"] = combinedEffectValue.ToString().Trim(); //copy the new value to the new column
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

            arguments.Append(" " + argFormatter.GetFormattedArgument(RepeatedFactor, true)); //5

            arguments.Append(" " + argFormatter.GetFormattedArgument(Subject, true)); //6

            //assemble a model for the covariate plot (if a covariate has been chosen)...
            arguments.Append(" " + argFormatter.GetFormattedArgument(Covariates)); //7

            arguments.Append(" " + argFormatter.GetFormattedArgument(Covariance, false)); //8

            arguments.Append(" " + argFormatter.GetFormattedArgument(ResponseTransformation, false)); //9

            arguments.Append(" " + argFormatter.GetFormattedArgument(CovariateTransformation, false)); //10

            arguments.Append(" " + argFormatter.GetFormattedArgument(PrimaryFactor, true)); //11

            arguments.Append(" " + argFormatter.GetFormattedArgument(Treatments)); //12

            arguments.Append(" " + argFormatter.GetFormattedArgument(OtherDesignFactors)); //13

            arguments.Append(" " + argFormatter.GetFormattedArgument(ANOVASelected)); //14
            arguments.Append(" " + argFormatter.GetFormattedArgument(PRPlotSelected)); //15
            arguments.Append(" " + argFormatter.GetFormattedArgument(NormalPlotSelected)); //16

            arguments.Append(" " + argFormatter.GetFormattedArgument(Significance, false)); //17

            //assemble the effect model
            arguments.Append(" " + argFormatter.GetFormattedArgument(GetEffectModel(), true)); //18

            arguments.Append(" " + argFormatter.GetFormattedArgument(SelectedEffect, true)); //19

            arguments.Append(" " + argFormatter.GetFormattedArgument(LSMeansSelected)); //20

            arguments.Append(" " + argFormatter.GetFormattedArgument(ComparisonType.ToString(), false)); //21

            arguments.Append(" " + argFormatter.GetFormattedArgument(GenerateComparisonsDataset)); //22

            return arguments.ToString().Trim();
        }


        public override void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.LoadStringArgument(nameof(Response));
            this.Treatments = argHelper.LoadIEnumerableArgument(nameof(Treatments));
            this.OtherDesignFactors = argHelper.LoadIEnumerableArgument(nameof(OtherDesignFactors));
            this.RepeatedFactor = argHelper.LoadStringArgument(nameof(RepeatedFactor));
            this.Subject = argHelper.LoadStringArgument(nameof(Subject));
            this.ResponseTransformation = argHelper.LoadStringArgument(nameof(ResponseTransformation));
            this.Covariance = argHelper.LoadStringArgument(nameof(Covariance));
            this.Covariates = argHelper.LoadIEnumerableArgument(nameof(Covariates));
            this.PrimaryFactor = argHelper.LoadStringArgument(nameof(PrimaryFactor));
            this.CovariateTransformation = argHelper.LoadStringArgument(nameof(CovariateTransformation));
            this.ANOVASelected = argHelper.LoadBooleanArgument(nameof(ANOVASelected));
            this.PRPlotSelected = argHelper.LoadBooleanArgument(nameof(PRPlotSelected));
            this.NormalPlotSelected = argHelper.LoadBooleanArgument(nameof(NormalPlotSelected));
            this.Significance = argHelper.LoadStringArgument(nameof(Significance));
            this.SelectedEffect = argHelper.LoadStringArgument(nameof(SelectedEffect));
            this.LSMeansSelected = argHelper.LoadBooleanArgument(nameof(LSMeansSelected));
            this.ComparisonType = (ComparisonOption)Enum.Parse(typeof(ComparisonOption), argHelper.LoadStringArgument(nameof(ComparisonType)), true);
            this.GenerateComparisonsDataset = argHelper.LoadBooleanArgument(nameof(GenerateComparisonsDataset));
        }

        public override IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatments), Treatments));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OtherDesignFactors), OtherDesignFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(RepeatedFactor), RepeatedFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Subject), Subject));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariance), Covariance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariates), Covariates));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PrimaryFactor), PrimaryFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CovariateTransformation), CovariateTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ANOVASelected), ANOVASelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PRPlotSelected), PRPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalPlotSelected), NormalPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SelectedEffect), SelectedEffect));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LSMeansSelected), LSMeansSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ComparisonType), ComparisonType.ToString()));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(GenerateComparisonsDataset), GenerateComparisonsDataset));

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

            model.Append("Timezzz+"); //Time model needs zzz adding on the end for R to be able to recognise Time var

            //determine the interactions
            List<string> factors = new List<string>(Treatments);
            factors.Add("Timezzz"); //add in time to the factors PLUS the zzz's

            List<string> fullInteractions = DetermineInteractions(factors);

            foreach (string s in fullInteractions)
            {
                model.Append(s.Replace(" * ", "*") + "+");
            }

            return model.ToString().TrimEnd('+');
        }

        public string GetEffectModel()
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

            List<string> factors = new List<string>(Treatments);
            factors.Add(RepeatedFactor); //add in time to the factors

            foreach (string s in factors)
            {
                if (!interactionEffects.Contains(s)) //only add on effects if the "mainEffect"/selected effect does not already have it
                {
                    effectModel.Append(s + "+");
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

        public static List<string> DetermineSelectedEffectsList(List<string> selectedTreatments, string repeatedFactor)
        {
            if (String.IsNullOrEmpty(repeatedFactor))
                throw new ArgumentException("repeatedFactor should not be null/empty!");

            //assemble a complete list of main and interaction effects
            List<string> effects = new List<string>();

            //assemble a list on effects - for repeated measures time is a compulsory interaction effect
            List<string> listToCreateInteractionsFrom = new List<string>(selectedTreatments);
            listToCreateInteractionsFrom.Add(repeatedFactor);

            //add in the interactions
            List<string> interactions = DetermineInteractions(listToCreateInteractionsFrom);
            foreach (string interaction in interactions)
            {
                if (interaction.Contains(repeatedFactor))
                    effects.Add(interaction);
            }

            return effects;
        }
    }
}