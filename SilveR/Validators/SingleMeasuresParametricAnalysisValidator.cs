using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SilveR.Validators
{
    public class SingleMeasuresParametricAnalysisValidator : ValidatorBase
    {
        private readonly SingleMeasuresParametricAnalysisModel smVariables;

        public SingleMeasuresParametricAnalysisValidator(SingleMeasuresParametricAnalysisModel sm)
            : base(sm.DataTable)
        {
            smVariables = sm;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.AddVariables(smVariables.Treatments);
            allVars.AddVariables(smVariables.OtherDesignFactors);
            allVars.Add(smVariables.Response);
            allVars.AddVariables(smVariables.Covariates);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            if (!CheckFactorsHaveLevels(smVariables.Treatments, true))
                return ValidationInfo;

            //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
            if (!CheckResponsesPerLevel(smVariables.Treatments, smVariables.Response, ReflectionExtensions.GetPropertyDisplayName<SingleMeasuresParametricAnalysisModel>(i => i.Treatments)))
                return ValidationInfo;

            if (!CheckResponsesPerLevel(smVariables.OtherDesignFactors, smVariables.Response, ReflectionExtensions.GetPropertyDisplayName<SingleMeasuresParametricAnalysisModel>(i => i.OtherDesignFactors)))
                return ValidationInfo;

            //First create a list of catogorical variables selected (i.e. as treatments and other factors)
            List<string> categorical = new List<string>();
            categorical.AddVariables(smVariables.Treatments);
            categorical.AddVariables(smVariables.OtherDesignFactors);

            //do data checks on the treatments/other factors and response
            if (!FactorAndResponseCovariateChecks(categorical, smVariables.Response))
                return ValidationInfo;

            //do data checks on the treatments/other factors and covariate (if selected)
            if (smVariables.Covariates != null)
            {
                foreach (string covariate in smVariables.Covariates)
                {
                    if (!FactorAndResponseCovariateChecks(categorical, covariate))
                        return ValidationInfo;
                }
            }

            //check that the effect selected is the highest order interaction possible from selected factors, else output warning
            CheckEffectSelectedIsHighestOrderInteraction();

            if (smVariables.Covariates != null && String.IsNullOrEmpty(smVariables.PrimaryFactor))
            {
                ValidationInfo.AddErrorMessage("You have selected a covariate but no primary factor is selected.");
            }

            if (!String.IsNullOrEmpty(smVariables.ComparisonsBackToControl) && String.IsNullOrEmpty(smVariables.ControlGroup))
            {
                ValidationInfo.AddErrorMessage("You have selected to compare back to a control but no control group is selected.");
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }

        private bool FactorAndResponseCovariateChecks(List<string> categorical, string continuous)
        {
            foreach (string catFactor in categorical) //go through each categorical factor and do the check on each
            {
                string factorType;
                if (smVariables.Treatments.Contains(catFactor))
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<SingleMeasuresParametricAnalysisModel>(i => i.Treatments);
                }
                else
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<SingleMeasuresParametricAnalysisModel>(i => i.OtherDesignFactors);
                }

                string responseType;
                if (smVariables.Response.Contains(continuous))
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<SingleMeasuresParametricAnalysisModel>(i => i.Response);
                }
                else
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<SingleMeasuresParametricAnalysisModel>(i => i.Covariates);
                }

                //Now that the whole column checks have been done, ensure that the treatment and response for each row is ok
                List<string> categoricalRow = new List<string>();
                List<string> continuousRow = new List<string>();

                foreach (DataRow row in DataTable.Rows) //assemble a list of the categorical data and the continuous data...
                {
                    categoricalRow.Add(row[catFactor].ToString());
                    continuousRow.Add(row[continuous].ToString());
                }

                for (int i = 0; i < DataTable.Rows.Count; i++) //use for loop cos its easier to compare the indexes of the cat and cont rows
                {
                    //Check that the "response" does not contain non-numeric data
                    bool parsedOK = Double.TryParse(continuousRow[i], out double parsedValue);
                    if (!String.IsNullOrEmpty(continuousRow[i]) && !parsedOK)
                    {
                        ValidationInfo.AddErrorMessage("The " + responseType + " selected (" + continuous + ") contain non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                        return false;
                    }

                    //Check that there are no responses where the treatments are blank
                    if (String.IsNullOrEmpty(categoricalRow[i]) && !String.IsNullOrEmpty(continuousRow[i]))
                    {
                        ValidationInfo.AddErrorMessage("The " + factorType + " selected (" + catFactor + ") contains missing data where there are observations present in the " + responseType + " variable. Please check the raw data and make sure the data was entered correctly.");
                        return false;
                    }

                    //check that the "response" contains data for each "treatment" (not fatal)
                    if (!String.IsNullOrEmpty(categoricalRow[i]) && String.IsNullOrEmpty(continuousRow[i]))
                    {
                        string mess = "The " + responseType + " selected contains missing data.";
                        if (responseType == "covariate")
                        {
                            mess = mess + Environment.NewLine + " Any response that does not have a corresponding covariate will be excluded from the analysis.";
                        }

                        ValidationInfo.AddWarningMessage(mess);
                    }
                }

                //check transformations
                CheckTransformations(DataTable, smVariables.ResponseTransformation, smVariables.Response);

                if (smVariables.Covariates != null)
                {
                    foreach (string covariate in smVariables.Covariates)
                    {
                        CheckTransformations(DataTable, smVariables.CovariateTransformation, covariate, true);
                    }
                }
            }

            //if got here then all checks ok, return true
            return true;
        }

        private void CheckEffectSelectedIsHighestOrderInteraction()
        {
            if (!String.IsNullOrEmpty(smVariables.SelectedEffect))
            {
                string[] splittedEffect = smVariables.SelectedEffect.Split('*');

                if (splittedEffect.GetLength(0) < smVariables.Treatments.Count())
                {
                    string mainEffectOrInteraction;

                    if (splittedEffect.GetLength(0) == 1) //then only one factor
                    {
                        mainEffectOrInteraction = "a main effect";
                    }
                    else // then an interaction
                    {
                        mainEffectOrInteraction = "an interaction";
                    }

                    ValidationInfo.AddWarningMessage("You have selected to plot/compare levels of " + mainEffectOrInteraction +
                        " in the presence of a higher order interaction(s). This should only be carried out if the higher order interaction(s) are not statistically significant. In the following we have removed these interaction(s) from the model prior to making the comparisons. The actual model fitted is " +
                        smVariables.GetEffectModel().Replace("mainEffect", " selected effect"));
                }
            }
        }
    }
}