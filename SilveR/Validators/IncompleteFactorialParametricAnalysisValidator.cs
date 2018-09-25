using SilveRModel.StatsModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace SilveRModel.Validators
{
    public class IncompleteFactorialParametricAnalysisValidator : ValidatorBase
    {
        private readonly IncompleteFactorialParametricAnalysisModel ifpVariables;

        public IncompleteFactorialParametricAnalysisValidator(IncompleteFactorialParametricAnalysisModel ifp)
            : base(ifp.DataTable)
        {
            ifpVariables = ifp;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.AddRange(ifpVariables.Treatments);

            if (ifpVariables.OtherDesignFactors != null)
                allVars.AddRange(ifpVariables.OtherDesignFactors);

            allVars.Add(ifpVariables.Response);

            if (ifpVariables.Covariates != null)
                allVars.AddRange(ifpVariables.Covariates);

            if (!CheckColumnNames(allVars)) return ValidationInfo;

            if (!CheckTreatmentsHaveLevels(ifpVariables.Treatments, true)) return ValidationInfo;

            //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
            if (!CheckResponsesPerLevel(ifpVariables.Treatments, ifpVariables.Response)) return ValidationInfo;

            if (ifpVariables.OtherDesignFactors != null)
                if (!CheckResponsesPerLevel(ifpVariables.OtherDesignFactors, ifpVariables.Response, "other treatment")) return ValidationInfo;

            //First create a list of catogorical variables selected (i.e. as treatments and other factors)
            List<string> categorical = new List<string>();
            categorical.AddRange(ifpVariables.Treatments);

            if (ifpVariables.OtherDesignFactors != null)
                categorical.AddRange(ifpVariables.OtherDesignFactors);

            //do data checks on the treatments/other factors and response
            if (!FactorAndResponseCovariateChecks(categorical, ifpVariables.Response)) return ValidationInfo;

            //do data checks on the treatments/other factors and covariate (if selected)
            if (ifpVariables.Covariates != null)
            {
                foreach (string covariate in ifpVariables.Covariates)
                {
                    if (!FactorAndResponseCovariateChecks(categorical, covariate)) return ValidationInfo;
                }
            }

            //check that the effect selected is the highest order interaction possible from selected factors, else output warning
            CheckEffectSelectedIsHighestOrderInteraction();

            //
            if (ifpVariables.Covariates != null && String.IsNullOrEmpty(ifpVariables.PrimaryFactor))
            {
                ValidationInfo.AddErrorMessage("You have selected a covariate but no primary factor is selected.");
            }

            if (!String.IsNullOrEmpty(ifpVariables.ComparisonsBackToControl) && String.IsNullOrEmpty(ifpVariables.ControlGroup))
            {
                ValidationInfo.AddErrorMessage("You have selected to compare back to a control but no control group is selected");
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }

        private bool FactorAndResponseCovariateChecks(List<string> categorical, string continuous)
        {
            foreach (string catFactor in categorical) //go through each categorical factor and do the check on each
            {
                string factorType;
                if (ifpVariables.Treatments.Contains(catFactor))
                {
                    factorType = "treatment";
                }
                else
                {
                    factorType = "other factor";
                }

                string responseType;
                if (ifpVariables.Response.Contains(continuous))
                {
                    responseType = "response";
                }
                else
                {
                    responseType = "covariate";
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
                    double parsedValue;
                    bool parsedOK = Double.TryParse(continuousRow[i], out parsedValue);
                    if (!String.IsNullOrEmpty(continuousRow[i]) && !parsedOK)
                    {
                        ValidationInfo.AddErrorMessage("The " + responseType + " (" + ifpVariables.Response + ") selected contain non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                        return false;
                    }

                    //Check that there are no responses where the treatments are blank
                    if (String.IsNullOrEmpty(categoricalRow[i]) && !String.IsNullOrEmpty(continuousRow[i]))
                    {
                        ValidationInfo.AddErrorMessage("The " + factorType + " (" + catFactor + ") selected contains missing data where there are observations present in the " + responseType + " variable. Please check the raw data and make sure the data was entered correctly.");

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
                foreach (DataRow row in DataTable.Rows)
                {
                    CheckTransformations(row, ifpVariables.ResponseTransformation, ifpVariables.Response, "response");

                    if (ifpVariables.Covariates != null)
                    {
                        foreach (string covariate in ifpVariables.Covariates)
                        {
                            CheckTransformations(row, ifpVariables.CovariateTransformation, covariate, "covariate");
                        }
                    }
                }
            }

            //if got here then all checks ok, return true
            return true;
        }

        private void CheckEffectSelectedIsHighestOrderInteraction()
        {
            if (!String.IsNullOrEmpty(ifpVariables.SelectedEffect))
            {
                string[] splittedEffect = ifpVariables.SelectedEffect.Split('*');

                if (splittedEffect.GetLength(0) < ifpVariables.Treatments.Count)
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
                        ifpVariables.GetEffectModel().Replace("mainEffect", " selected effect"));
                }
            }
        }
    }
}