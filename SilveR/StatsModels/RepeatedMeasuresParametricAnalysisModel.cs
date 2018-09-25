using Combinatorics;
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
    public class RepeatedMeasuresParametricAnalysisModel : IAnalysisModel
    {
        private const string MAIN_EFFECT_SEPERATOR = "_.._";

        public string ScriptFileName { get { return "RepeatedMeasuresParametricAnalysis"; } }

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

        [HasAtLeastOneEntry]
        [CheckUsedOnceOnly]
        public List<string> Treatments { get; set; }

        [DisplayName("Other design (blocks)")]
        [CheckUsedOnceOnly]
        public List<string> OtherDesignFactors { get; set; }

        [Display(Name = "Repeated factor")]
        [Required]
        [CheckUsedOnceOnly]
        public string RepeatedFactor { get; set; }

        [Display(Name = "Subject factor")]
        [CheckUsedOnceOnly]
        [Required]
        public string Subject { get; set; }

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public List<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        public List<string> CovariancesList
        {
            get { return new List<string>() { "Compound Symmetric", "Unstructured", "Autoregressive(1)" }; }
        }

        public string Covariance { get; set; } = "Compound Symmetric";

        [CheckUsedOnceOnly]
        public string Covariate { get; set; }

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

        [DisplayName("Least Square (predicted) means")]
        public bool LSMeansSelected { get; set; }

        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [DisplayName("Effect")]
        public string SelectedEffect { get; set; }

        public enum ComparisonOption { AllComparisonsWithinSelected = 0, AllPairwiseComparisons = 1 }
        private ComparisonOption comparisonType = ComparisonOption.AllComparisonsWithinSelected;
        public ComparisonOption ComparisonType
        {
            get { return comparisonType; }
            set
            {
                if (comparisonType != value)
                {
                    comparisonType = value;
                }
            }
        }


        public RepeatedMeasuresParametricAnalysisModel() { }

        public RepeatedMeasuresParametricAnalysisModel(Dataset dataset)
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
            RepeatedMeasuresParametricAnalysisValidator repeatedMeasuresParametricAnalysisValidator = new RepeatedMeasuresParametricAnalysisValidator(this);
            return repeatedMeasuresParametricAnalysisValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string col in dtNew.GetVariableNames())
            {
                if (Response != col && !Treatments.Contains(col) && (OtherDesignFactors == null || !OtherDesignFactors.Contains(col)) && RepeatedFactor != col && Subject != col && Covariate != col)
                {
                    dtNew.Columns.Remove(col);
                }
            }

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

            //if the response is blank then remove that row
            dtNew.RemoveBlankRow(Response);


            //Need to create a new column for "between" as we have to combine any interaction effects into one column
            DataColumn between = new DataColumn("between");
            dtNew.Columns.Add(between);
            foreach (DataRow r in dtNew.Rows) //go through each row...
            {
                string combinedEffectValue = null;
                foreach (string s in Treatments) //combine the values from each column into one string
                {
                    combinedEffectValue = combinedEffectValue + " " + r[s.Trim()];
                }

                r["between"] = combinedEffectValue.Trim(); //copy the new value to the new column
            }

            //Need to create a new column for "betweenwithin" as we have to combine any interaction effects into one column
            DataColumn betweenwithin = new DataColumn("betweenwithin");
            dtNew.Columns.Add(betweenwithin);
            foreach (DataRow r in dtNew.Rows) //go through each row...
            {
                string combinedEffectValue = null;

                List<string> factors = new List<string>(Treatments);
                if (!String.IsNullOrEmpty(RepeatedFactor)) factors.Add(RepeatedFactor); //add in time to the factors

                foreach (string s in factors) //combine the values from each column into one string
                {
                    combinedEffectValue = combinedEffectValue + " " + r[s.Trim()];
                }

                r["betweenwithin"] = combinedEffectValue.Trim(); //copy the new value to the new column
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
                            combinedEffectValue = combinedEffectValue + MAIN_EFFECT_SEPERATOR + r[s.Trim()];
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

            if (!String.IsNullOrEmpty(Covariate)) //check that a covariate is selected
            {
                dtNew.TransformColumn(Covariate, CovariateTransformation);
            }

            //Finally, as numeric categorical variables get misinterpreted by r, we need to go through
            //each column and put them in quotes...
            foreach (string treat in Treatments)
            {
                if (dtNew.ColumnIsNumeric(treat))
                {
                    foreach (DataRow row in dtNew.Rows)
                    {
                        row[treat] = "'" + row[treat] + "'";
                    }
                }
            }
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


        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            //first thing to do is to assemble the model (use the GetModel method)
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(GetModel())); //4

            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(RepeatedFactor))); //5

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

            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(PrimaryFactor))); //11

            string treats = null;
            foreach (string s in Treatments) treats = treats + "," + s;
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(treats.TrimStart(','))); //12

            string blocks = null;

            if (OtherDesignFactors != null)
            {
                foreach (string s in OtherDesignFactors) blocks = blocks + "," + s;
            }

            if (String.IsNullOrEmpty(blocks)) //13
                arguments.Append(" " + "NULL");
            else
                arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(blocks.TrimStart(',')));

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ANOVASelected)); //14
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(PRPlotSelected)); //15
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(NormalPlotSelected)); //16

            arguments.Append(" " + Significance); //17

            //assemble the effect model
            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(GetEffectModel()))); //18

            arguments.Append(" " + SelectedEffect); //19

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(LSMeansSelected)); //20

            if (ComparisonType == ComparisonOption.AllComparisonsWithinSelected) arguments.Append(" " + "Reduced"); //21
            else if (ComparisonType == ComparisonOption.AllPairwiseComparisons) arguments.Append(" " + "All"); //21

            return arguments.ToString();
        }


        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader(nameof(Response), Response);
            this.Treatments = argHelper.ArgumentLoader(nameof(Treatments), Treatments);
            this.OtherDesignFactors = argHelper.ArgumentLoader(nameof(OtherDesignFactors), OtherDesignFactors);
            this.RepeatedFactor = argHelper.ArgumentLoader(nameof(RepeatedFactor ), RepeatedFactor);
            this.Subject = argHelper.ArgumentLoader(nameof(Subject ), Subject);
            this.ResponseTransformation = argHelper.ArgumentLoader(nameof(ResponseTransformation), ResponseTransformation);
            this.Covariance = argHelper.ArgumentLoader(nameof(Covariance ), Covariance);
            this.Covariate = argHelper.ArgumentLoader(nameof(Covariate), Covariate);
            this.PrimaryFactor = argHelper.ArgumentLoader(nameof(PrimaryFactor), PrimaryFactor);
            this.CovariateTransformation = argHelper.ArgumentLoader(nameof(CovariateTransformation), CovariateTransformation);
            this.ANOVASelected = argHelper.ArgumentLoader(nameof(ANOVASelected), ANOVASelected);
            this.PRPlotSelected = argHelper.ArgumentLoader(nameof(PRPlotSelected), PRPlotSelected);
            this.NormalPlotSelected = argHelper.ArgumentLoader(nameof(NormalPlotSelected), NormalPlotSelected);
            this.LSMeansSelected = argHelper.ArgumentLoader(nameof(LSMeansSelected), LSMeansSelected);
            this.Significance = argHelper.ArgumentLoader(nameof(Significance), Significance);
            this.SelectedEffect = argHelper.ArgumentLoader(nameof(SelectedEffect), SelectedEffect);
            this.ComparisonType = (ComparisonOption)Enum.Parse(typeof(ComparisonOption), argHelper.ArgumentLoader(nameof(ComparisonType), String.Empty), true);
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory(nameof(Response), Response));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Treatments), Treatments));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(OtherDesignFactors), OtherDesignFactors));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(RepeatedFactor ), RepeatedFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Subject ), Subject));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ResponseTransformation), ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariance ), Covariance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Covariate), Covariate));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PrimaryFactor), PrimaryFactor));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(CovariateTransformation), CovariateTransformation));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ANOVASelected), ANOVASelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(PRPlotSelected), PRPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(NormalPlotSelected), NormalPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(LSMeansSelected), LSMeansSelected));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(Significance), Significance));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(SelectedEffect), SelectedEffect));
            args.Add(ArgumentHelper.ArgumentFactory(nameof(ComparisonType), ComparisonType.ToString()));

            return args;
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

            foreach (string s in Treatments)
                model = model + s + "+";

            if (!String.IsNullOrEmpty(RepeatedFactor))
                model = model + "Timezzz+"; //Time model needs zzz adding on the end for R to be able to recognise Time var

            //determine the interactions
            List<string> factors = new List<string>(Treatments);
            if (!String.IsNullOrEmpty(RepeatedFactor)) factors.Add("Timezzz"); //add in time to the factors PLUS the zzz's
            List<string> fullInteractions = DetermineInteractions(factors);

            foreach (string s in fullInteractions)
                model = model + s.Replace(" * ", "*") + "+";

            model = model.TrimEnd('+');

            return model;
        }

        public string GetEffectModel()
        {
            //assemble the effect model
            string effectModel = Response + "~"; //add in the response

            if (!String.IsNullOrEmpty(Covariate)) //add in a covariate if one is selected
                effectModel = effectModel + Covariate + "+";

            if (OtherDesignFactors != null)
            {
                foreach (string s in OtherDesignFactors) //add in blocking factors if they are selected
                    effectModel = effectModel + s + "+";
            }

            //complicated business of assembling the other part of the model from the interactions etc...
            string[] splitter = { " * " };
            List<string> interactionEffects = new List<string>(SelectedEffect.Split(splitter, StringSplitOptions.RemoveEmptyEntries));

            List<string> factors = new List<string>(Treatments);
            if (!String.IsNullOrEmpty(RepeatedFactor)) factors.Add(RepeatedFactor); //add in time to the factors

            foreach (string s in factors)
            {
                if (!interactionEffects.Contains(s)) //only add on effects if the "mainEffect"/selected effect does not already have it
                {
                    effectModel = effectModel + s + "+";
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

        public static List<string> DetermineSelectedEffectsList(List<string> selectedTreatments, string repeatedFactor)
        {
            //assemble a complete list of main and interaction effects
            List<string> effects = new List<string>();

            //assemble a list on effects - for repeated measures time is a compulsory interaction effect
            List<string> listToCreateInteractionsFrom = new List<string>(selectedTreatments);
            if (!String.IsNullOrEmpty(repeatedFactor)) listToCreateInteractionsFrom.Add(repeatedFactor);
            var interactions = DetermineInteractions(listToCreateInteractionsFrom);

            foreach (string interaction in interactions)
            {
                if (!String.IsNullOrEmpty(repeatedFactor) && interaction.Contains(repeatedFactor))
                    effects.Add(interaction);
            }

            //if the number of interaction effects is 4 or greater,
            //then only the main effects and highest order effect are to be available
            if (selectedTreatments.Count >= 4)
            {
                //remove any effect that is an interaction effect
                for (int i = effects.Count - 1; i >= 0; i = i - 1)
                {
                    if (effects[i].Contains("*")) effects.Remove(effects[i]);
                }
                //add in the highest order interaction again
                effects.Add(interactions[interactions.Count - 1].ToString());
            }

            //if the number of interaction effects is 4 or greater,
            //then only the main effects and highest order effect are to be available
            if (selectedTreatments.Count >= 4)
            {
                //remove any effect that is an interaction effect
                for (int i = effects.Count - 1; i >= 0; i = i - 1)
                {
                    if (effects[i].Contains("*")) effects.Remove(effects[i]);
                }
                //add in the highest order interaction again
                effects.Add(interactions[interactions.Count - 1].ToString());
            }

            return effects;
        }

        public bool VariablesUsedOnceOnly(string memberName)
        {
            object varToBeChecked = this.GetType().GetProperty(memberName).GetValue(this, null);

            if (varToBeChecked != null)
            {
                UniqueVariableChecker checker = new UniqueVariableChecker();

                if (memberName != "Response")
                    checker.AddVar(this.Response);

                if (memberName != "Treatments")
                    checker.AddVars(this.Treatments);

                if (memberName != "RepeatedFactor")
                    checker.AddVar(this.RepeatedFactor);

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