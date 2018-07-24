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
    public class SingleMeasuresModel : IAnalysisModel
    {
        public string ScriptFileName { get { return "SingleMeasureGLM"; } }

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

        [DisplayName("Response transformation")]
        public string ResponseTransformation { get; set; } = "None";

        public List<string> TransformationsList
        {
            get { return new List<string>() { "None", "Log10", "Loge", "Square Root", "ArcSine", "Rank" }; }
        }

        [CheckUsedOnceOnly]
        public string Covariate { get; set; }

        [DisplayName("Primary factor")]
        [ValidatePrimaryFactorSetWhenCovariateSelected]
        public string PrimaryFactor { get; set; }

        [DisplayName("Covariate transformation")]
        public string CovariateTransformation { get; set; } = "None";

        [DisplayName("ANOVA table")]
        public bool ANOVASelected { get; set; } = true;

        [DisplayName("Predicted vs. residuals plot")]
        public bool PRPlotSelected { get; set; } = true;

        [DisplayName("Normal probability plot")]
        public bool NormalPlotSelected { get; set; }

        [DisplayName("Standardised effects")]
        public bool StandardisedEffectsSelected { get; set; }

        public string Significance { get; set; } = "0.05";

        public List<string> SignificancesList
        {
            get { return new List<string>() { "0.1", "0.05", "0.01", "0.001" }; }
        }

        [DisplayName("Effect")]
        public string SelectedEffect { get; set; }

        [DisplayName("Least Square (predicted) means")]
        public bool LSMeansSelected { get; set; }

        [DisplayName("All pairwise comparisons")]
        public string AllPairwise { get; set; }

        public List<string> PostHocTestList
        {
            get { return new List<string>() { String.Empty, "Unadjusted (LSD)", "Tukey", "Holm", "Hochberg", "Hommel", "Bonferonni", "Benjamini-Hochberg" }; }
        }

        [DisplayName("Comparisons back to control")]
        public string ComparisonsBackToControl { get; set; }

        [DisplayName("Control group")]
        [ValidateControlLevelSetWhenComparingToControl]
        public string ControlGroup { get; set; }

        public List<string> ControlGroupList { get; set; }


        public SingleMeasuresModel() { }

        public SingleMeasuresModel(Dataset dataset)
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
            SingleMeasuresValidator singleMeasuresValidator = new SingleMeasuresValidator(this);
            return singleMeasuresValidator.Validate();
        }

        public string[] ExportData()
        {
            DataTable dtNew = dataTable.CopyForExport();

            //Get the response, treatment and covariate columns by removing all other columns from the new datatable
            foreach (string columnName in dtNew.GetVariableNames())
            {
                if (Response != columnName && !Treatments.Contains(columnName) && OtherDesignFactors != null && !OtherDesignFactors.Contains(columnName) && Covariate != columnName)
                {
                    dtNew.Columns.Remove(columnName);
                }
            }

            //ensure that all data is trimmed
            dtNew.TrimAllDataInDataTable();

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


        public string GetCommandLineArguments()
        {
            StringBuilder arguments = new StringBuilder();

            //first thing to do is to assemble the model (use the GetModel method)
            arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(GetModel())); //4

            string scatterplotModel = ArgumentConverters.ConvertIllegalChars(Response) + "~scatterPlotColumn";
            arguments.Append(" " + scatterplotModel); //5

            //assemble a model for the covariate plot (if a covariate has been chosen)...
            if (String.IsNullOrEmpty(Covariate)) //6
            {
                arguments.Append(" " + "NULL");
            }
            else
            {
                string covariateModel = ArgumentConverters.ConvertIllegalChars(Response) + "~" + ArgumentConverters.ConvertIllegalChars(Covariate);
                arguments.Append(" " + covariateModel);
            }

            //get transforms
            arguments.Append(" " + "\"" + ResponseTransformation + "\""); //7

            arguments.Append(" " + "\"" + CovariateTransformation + "\""); //8

            arguments.Append(" " + ArgumentConverters.GetNULLOrText(ArgumentConverters.ConvertIllegalChars(PrimaryFactor))); //9

            string treats = null;
            foreach (string treat in Treatments) treats = treats + "," + ArgumentConverters.ConvertIllegalChars(treat);
            arguments.Append(" " + treats.TrimStart(',')); //10

            string blocks = null;
            if (OtherDesignFactors != null)
            {
                foreach (string otherDesign in OtherDesignFactors) blocks = blocks + "," + ArgumentConverters.ConvertIllegalChars(otherDesign);
            }

            if (String.IsNullOrEmpty(blocks)) //11
                arguments.Append(" " + "NULL");
            else
                arguments.Append(" " + blocks.TrimStart(','));

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(ANOVASelected)); //12
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(PRPlotSelected)); //13
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(NormalPlotSelected)); //14
            arguments.Append(" " + ArgumentConverters.GetYesOrNo(StandardisedEffectsSelected)); //15

            arguments.Append(" " + Significance); //16

            //assemble the effect model
            if (String.IsNullOrEmpty(SelectedEffect)) //17, 18
            {
                arguments.Append(" " + "NULL");
                arguments.Append(" " + "NULL");
            }
            else
            {
                arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(GetEffectModel()));
                arguments.Append(" " + ArgumentConverters.ConvertIllegalChars(SelectedEffect));
            }

            arguments.Append(" " + ArgumentConverters.GetYesOrNo(LSMeansSelected)); //19

            if (!String.IsNullOrEmpty(AllPairwise))
                arguments.Append(" " + "\"" + AllPairwise + "\""); //20
            else
                arguments.Append(" " + "NULL");

            if (!String.IsNullOrEmpty(ComparisonsBackToControl))
                arguments.Append(" " + "\"" + ComparisonsBackToControl + "\""); //21
            else
                arguments.Append(" " + "NULL");

            arguments.Append(" " + "\"" + ControlGroup + "\""); //22

            arguments.Append(" " + "N"); //23


            return arguments.ToString();
        }



        public void LoadArguments(IEnumerable<Argument> arguments)
        {
            ArgumentHelper argHelper = new ArgumentHelper(arguments);

            this.Response = argHelper.ArgumentLoader("Response", Response);
            this.Treatments = argHelper.ArgumentLoader("Treatments", Treatments);
            this.OtherDesignFactors = argHelper.ArgumentLoader("OtherDesignFactors", OtherDesignFactors);
            this.ResponseTransformation = argHelper.ArgumentLoader("ResponseTransformation", ResponseTransformation);
            this.Covariate = argHelper.ArgumentLoader("Covariate", Covariate);
            this.PrimaryFactor = argHelper.ArgumentLoader("PrimaryFactor", PrimaryFactor);
            this.CovariateTransformation = argHelper.ArgumentLoader("CovariateTransformation", CovariateTransformation);
            this.ANOVASelected = argHelper.ArgumentLoader("ANOVASelected", ANOVASelected);
            this.PRPlotSelected = argHelper.ArgumentLoader("PRPlotSelected", PRPlotSelected);
            this.NormalPlotSelected = argHelper.ArgumentLoader("NormalPlotSelected", NormalPlotSelected);
            this.StandardisedEffectsSelected = argHelper.ArgumentLoader("StandardisedEffectsSelected", StandardisedEffectsSelected);
            this.Significance = argHelper.ArgumentLoader("Significance", Significance);
            this.SelectedEffect = argHelper.ArgumentLoader("SelectedEffect", SelectedEffect);
            this.LSMeansSelected = argHelper.ArgumentLoader("LSMeansSelected", LSMeansSelected);
            this.AllPairwise = argHelper.ArgumentLoader("AllPairwise", AllPairwise);
            this.ComparisonsBackToControl = argHelper.ArgumentLoader("ComparisonsBackToControl", ComparisonsBackToControl);
            this.ControlGroup = argHelper.ArgumentLoader("ControlGroup", ControlGroup);
        }

        public IEnumerable<Argument> GetArguments()
        {
            List<Argument> args = new List<Argument>();

            args.Add(ArgumentHelper.ArgumentFactory("Response", Response));
            args.Add(ArgumentHelper.ArgumentFactory("Treatments", Treatments));
            args.Add(ArgumentHelper.ArgumentFactory("OtherDesignFactors", OtherDesignFactors));
            args.Add(ArgumentHelper.ArgumentFactory("ResponseTransformation", ResponseTransformation));
            args.Add(ArgumentHelper.ArgumentFactory("Covariate", Covariate));
            args.Add(ArgumentHelper.ArgumentFactory("PrimaryFactor", PrimaryFactor));
            args.Add(ArgumentHelper.ArgumentFactory("CovariateTransformation", CovariateTransformation));
            args.Add(ArgumentHelper.ArgumentFactory("ANOVASelected", ANOVASelected));
            args.Add(ArgumentHelper.ArgumentFactory("PRPlotSelected", PRPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory("NormalPlotSelected", NormalPlotSelected));
            args.Add(ArgumentHelper.ArgumentFactory("StandardisedEffectsSelected", StandardisedEffectsSelected));
            args.Add(ArgumentHelper.ArgumentFactory("Significance", Significance));
            args.Add(ArgumentHelper.ArgumentFactory("SelectedEffect", SelectedEffect));
            args.Add(ArgumentHelper.ArgumentFactory("LSMeansSelected", LSMeansSelected));
            args.Add(ArgumentHelper.ArgumentFactory("AllPairwise", AllPairwise));
            args.Add(ArgumentHelper.ArgumentFactory("ComparisonsBackToControl", ComparisonsBackToControl));
            args.Add(ArgumentHelper.ArgumentFactory("ControlGroup", ControlGroup));

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
                foreach (string otherDesign in OtherDesignFactors)
                    model = model + otherDesign + "+";
            }

            foreach (string treat in Treatments)
                model = model + treat + "+";

            //determine the interactions
            List<string> factors = new List<string>(Treatments);
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
            //assemble a complete list of main and interaction effects
            List<string> effects = new List<string>();
            effects.AddRange(selectedTreatments);

            var interactions = DetermineInteractions(selectedTreatments);

            effects.AddRange(interactions);

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