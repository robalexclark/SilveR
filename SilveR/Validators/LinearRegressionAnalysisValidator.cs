using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SilveR.Validators
{
    public class LinearRegressionAnalysisValidator : ValidatorBase
    {
        private readonly LinearRegressionAnalysisModel lrVariables;

        public LinearRegressionAnalysisValidator(LinearRegressionAnalysisModel lr)
            : base(lr.DataTable)
        {
            lrVariables = lr;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.AddVariables(lrVariables.Response);
            allVars.AddVariables(lrVariables.ContinuousFactors);
            allVars.AddVariables(lrVariables.CategoricalFactors);
            allVars.AddVariables(lrVariables.OtherDesignFactors);
            allVars.AddVariables(lrVariables.Covariates);

            if (!CheckColumnNames(allVars)) return ValidationInfo;

            if (CountResponses(lrVariables.Response) <= 1)
            {
                ValidationInfo.AddErrorMessage("Error: Unfortunately as there are less than two valid responses in the dataset no analysis has been performed.");
                return ValidationInfo;
            }

            if (!CheckFactorsHaveLevels(lrVariables.CategoricalFactors, true))
                return ValidationInfo;


            //First create a list of categorical variables selected (i.e. as treatments and other factors)
            List<string> categorical = new List<string>();
            categorical.AddVariables(lrVariables.CategoricalFactors);
            categorical.AddVariables(lrVariables.OtherDesignFactors);

            //do data checks on the treatments/other factors and response
            if (categorical.Any() && !FactorAndResponseCovariateChecks(categorical, lrVariables.Response))
                return ValidationInfo;

            //need to seperately check continuous variables are numeric
            foreach (string contFactor in lrVariables.ContinuousFactors)
            {
                if (!CheckIsNumeric(contFactor))
                {
                    ValidationInfo.AddErrorMessage("Error: The continuous variable selected (" + contFactor + ") contain non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                    return ValidationInfo;
                }
            }

            //do data checks on the treatments/other factors and covariate (if selected)
            if (lrVariables.Covariates != null)
            {
                foreach (string covariate in lrVariables.Covariates)
                {
                    if (!FactorAndResponseCovariateChecks(categorical, covariate))
                        return ValidationInfo;
                }
            }

            //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
            if (!CheckResponsesPerLevel(lrVariables.CategoricalFactors, lrVariables.Response, ReflectionExtensions.GetPropertyDisplayName<LinearRegressionAnalysisModel>(i => i.CategoricalFactors)))
                return ValidationInfo;

            if (!CheckResponsesPerLevel(lrVariables.OtherDesignFactors, lrVariables.Response, ReflectionExtensions.GetPropertyDisplayName<LinearRegressionAnalysisModel>(i => i.OtherDesignFactors)))
                return ValidationInfo;

            if (lrVariables.Covariates != null && lrVariables.CategoricalFactors != null && String.IsNullOrEmpty(lrVariables.PrimaryFactor))
            {
                ValidationInfo.AddErrorMessage("You have selected a covariate but no primary factor is selected.");
            }

            CheckTransformations(DataTable, lrVariables.ResponseTransformation, lrVariables.Response);

            if (lrVariables.ContinuousFactors != null)
            {
                foreach (string contFactor in lrVariables.ContinuousFactors)
                {
                    CheckTransformations(DataTable, lrVariables.ContinuousFactorsTransformation, contFactor);
                }
            }

            if (lrVariables.Covariates != null)
            {
                foreach (string covariate in lrVariables.Covariates)
                {
                    CheckTransformations(DataTable, lrVariables.CovariateTransformation, covariate, true);
                }
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }

        private bool FactorAndResponseCovariateChecks(List<string> categoricalFactors, string continuous)
        {
            foreach (string catFactor in categoricalFactors) //go through each categorical factor and do the check on each
            {
                string factorType = null;
                if (lrVariables.CategoricalFactors != null && lrVariables.CategoricalFactors.Contains(catFactor))
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<LinearRegressionAnalysisModel>(i => i.CategoricalFactors);
                }
                else if (lrVariables.OtherDesignFactors != null && lrVariables.OtherDesignFactors.Contains(catFactor))
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<LinearRegressionAnalysisModel>(i => i.OtherDesignFactors);
                }

                string responseType = null;
                if (lrVariables.Response.Contains(continuous))
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<LinearRegressionAnalysisModel>(i => i.Response);
                }
                else if (lrVariables.Covariates != null)
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<LinearRegressionAnalysisModel>(i => i.Covariates);
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
                        ValidationInfo.AddErrorMessage("The " + responseType + " selected (" + lrVariables.Response + ") contain non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
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
                            mess = mess + Environment.NewLine + "Any response that does not have a corresponding covariate will be excluded from the analysis.";
                        }

                        ValidationInfo.AddWarningMessage(mess);
                    }
                }
            }

            //if got here then all checks ok, return true
            return true;
        }
    }
}