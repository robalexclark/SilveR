using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SilveR.Validators
{
    public class LogisticRegressionAnalysisValidator : ValidatorBase
    {
        private readonly LogisticRegressionAnalysisModel lrVariables;

        public LogisticRegressionAnalysisValidator(LogisticRegressionAnalysisModel sm)
            : base(sm.DataTable)
        {
            lrVariables = sm;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.AddVariables(lrVariables.Treatments);
            allVars.AddVariables(lrVariables.OtherDesignFactors);
            allVars.Add(lrVariables.Response);
            allVars.AddVariables(lrVariables.Covariates);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            if (CountResponses(lrVariables.Response) <= 1)
            {
                ValidationInfo.AddErrorMessage("Unfortunately as there are less than two valid responses in the dataset no analysis has been performed.");
                return ValidationInfo;
            }

            //First create a list of catogorical variables selected (i.e. as treatments and other factors)
            List<string> categorical = new List<string>();
            categorical.AddVariables(lrVariables.Treatments);
            categorical.AddVariables(lrVariables.OtherDesignFactors);
            categorical.AddVariables(lrVariables.ContinuousFactors); //apparently continuous variables here count as categorical
            categorical.AddVariables(lrVariables.Response);

            //check that the factors have at least 2 levels
            if (!CheckFactorsHaveLevels(categorical, true))
                return ValidationInfo;

            //check that the response has two distinct values
            int distinctResponseValues = CountDistinctLevels(lrVariables.Response);
            if(distinctResponseValues != 2)
            {
                ValidationInfo.AddErrorMessage("Response must have 2 distinct values.");
                return ValidationInfo;
            }

            //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
            if (!CheckResponsesPerLevel(lrVariables.Treatments, lrVariables.Response, ReflectionExtensions.GetPropertyDisplayName<LogisticRegressionAnalysisModel>(i => i.Treatments)))
                return ValidationInfo;

            if (!CheckResponsesPerLevel(lrVariables.OtherDesignFactors, lrVariables.Response, ReflectionExtensions.GetPropertyDisplayName<LogisticRegressionAnalysisModel>(i => i.OtherDesignFactors)))
                return ValidationInfo;

            //do data checks on the treatments/other factors and response
            if (!CategoricalAgainstContinuousVariableChecks(categorical, lrVariables.Response, ReflectionExtensions.GetPropertyDisplayName<LogisticRegressionAnalysisModel>(i => i.Response)))
                return ValidationInfo;

            if (lrVariables.Covariates != null)
            {
                foreach (string covariate in lrVariables.Covariates)
                {
                    if (!CategoricalAgainstContinuousVariableChecks(categorical, covariate, ReflectionExtensions.GetPropertyDisplayName<LogisticRegressionAnalysisModel>(i => i.Covariates)))
                        return ValidationInfo;
                }
            }

            //check transformations
            if (lrVariables.ContinuousFactors != null)
            {
                CheckTransformations(lrVariables.ContinuousFactorsTransformation, lrVariables.ContinuousFactors);
            }

            if (lrVariables.Covariates != null)
            {
                CheckTransformations(lrVariables.CovariateTransformation, lrVariables.Covariates, true);
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }

        private bool CategoricalAgainstContinuousVariableChecks(List<string> categorical, string continuous, string displayName)
        {
            foreach (string catFactor in categorical) //go through each categorical factor and do the check on each
            {
                string factorType;
                if (lrVariables.Treatments != null && lrVariables.Treatments.Contains(catFactor))
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<LogisticRegressionAnalysisModel>(i => i.Treatments);
                }
                else
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<LogisticRegressionAnalysisModel>(i => i.OtherDesignFactors);
                }

                string responseType;
                if (lrVariables.Response.Contains(continuous))
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<LogisticRegressionAnalysisModel>(i => i.Response);
                }
                else
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<LogisticRegressionAnalysisModel>(i => i.Covariates);
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
                    if (displayName != "Response")
                    {
                        bool parsedOK = Double.TryParse(continuousRow[i], out double parsedValue);
                        if (!String.IsNullOrEmpty(continuousRow[i]) && !parsedOK)
                        {
                            ValidationInfo.AddErrorMessage("The " + responseType + " (" + continuous + ") contain non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.");
                            return false;
                        }
                    }

                    //Check that there are no responses where the treatments are blank
                    if (String.IsNullOrEmpty(categoricalRow[i]) && !String.IsNullOrEmpty(continuousRow[i]))
                    {
                        ValidationInfo.AddErrorMessage("The " + factorType + " (" + catFactor + ") contains missing data where there are observations present in the " + responseType + ". Please check the input data and make sure the data was entered correctly.");
                        return false;
                    }

                    //check that the "response" contains data for each "treatment" (not fatal)
                    if (!String.IsNullOrEmpty(categoricalRow[i]) && String.IsNullOrEmpty(continuousRow[i]))
                    {
                        string mess = "The " + responseType + " (" + continuous + ") contains missing data.";
                        if (responseType == "Covariate")
                        {
                            mess = mess + " Any response that does not have a corresponding covariate will be excluded from the analysis.";
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