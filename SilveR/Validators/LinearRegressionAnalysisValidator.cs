﻿using SilveR.StatsModels;
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

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            if (CountResponses(lrVariables.Response) <= 1)
            {
                ValidationInfo.AddErrorMessage("Unfortunately as there are less than two valid responses in the dataset no analysis has been performed.");
                return ValidationInfo;
            }

            if (!CheckFactorsHaveLevels(lrVariables.CategoricalFactors, true))
                return ValidationInfo;

            if (!CheckFactorsHaveLevels(lrVariables.OtherDesignFactors, true))
                return ValidationInfo;

            //First create a list of categorical variables selected (i.e. as treatments and other factors)
            List<string> categorical = new List<string>();
            categorical.AddVariables(lrVariables.CategoricalFactors);
            categorical.AddVariables(lrVariables.OtherDesignFactors);

            //do data checks on the treatments/other factors and response
            if (categorical.Any() && !CategoricalAgainstContinuousVariableChecks(categorical, lrVariables.Response))
                return ValidationInfo;

            //need to seperately check continuous variables are numeric
            foreach (string contFactor in lrVariables.ContinuousFactors)
            {
                if (!CheckIsNumeric(contFactor))
                {
                    ValidationInfo.AddErrorMessage("The continuous variable (" + contFactor + ") contains non-numeric data which cannot be processed. Please check the input data and make sure the data was entered correctly.");
                    return ValidationInfo;
                }
            }

            //need to do a check on continuous variables vs response
            if (lrVariables.ContinuousFactors != null && !ContinuousAgainstResponseVariableChecks(lrVariables.ContinuousFactors, lrVariables.Response))
                return ValidationInfo;

            if (lrVariables.Covariates != null && !ContinuousAgainstResponseVariableChecks(lrVariables.Covariates, lrVariables.Response))
                return ValidationInfo;


            //do data checks on the treatments/other factors and covariate (if selected)
            if (lrVariables.Covariates != null)
            {
                foreach (string covariate in lrVariables.Covariates)
                {
                    if (!CategoricalAgainstContinuousVariableChecks(categorical, covariate))
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

            CheckTransformations(lrVariables.ResponseTransformation, lrVariables.Response);

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

        private bool CategoricalAgainstContinuousVariableChecks(List<string> categoricalFactors, string continuous)
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
                    //Check that the "response" does not contains non-numeric data
                    bool parsedOK = Double.TryParse(continuousRow[i], out double parsedValue);
                    if (!String.IsNullOrEmpty(continuousRow[i]) && !parsedOK)
                    {
                        ValidationInfo.AddErrorMessage("The " + responseType + " (" + lrVariables.Response + ") contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.");
                        return false;
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
                        string mess = null;
                        if (responseType == "Response")
                        {
                            mess = "The Response (" + continuous + ") contains missing data.";
                            mess = mess + " Any treatment that does not have a corresponding response will be excluded from the analysis.";
                        }
                        else if (responseType == "Covariate")
                        {
                            mess = "The Covariate (" + continuous + ") contains missing data.";
                            mess = mess + " Any response that does not have a corresponding covariate will be excluded from the analysis.";
                        }

                        ValidationInfo.AddWarningMessage(mess);
                    }
                }
            }

            //if got here then all checks ok, return true
            return true;
        }

        private bool ContinuousAgainstResponseVariableChecks(IEnumerable<string> continuousFactors, string response)
        {
            foreach (string contFactor in continuousFactors) //go through each continuous factor and do the check on each
            {
                string factorType = null;
                if (lrVariables.ContinuousFactors != null && lrVariables.ContinuousFactors.Contains(contFactor))
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<LinearRegressionAnalysisModel>(i => i.ContinuousFactors);
                }
                else if (lrVariables.Covariates != null && lrVariables.Covariates.Contains(contFactor))
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<LinearRegressionAnalysisModel>(i => i.Covariates);
                }

                //Now that the whole column checks have been done, ensure that the treatment and response for each row is ok
                List<string> continuousRow = new List<string>();
                List<string> responseRow = new List<string>();

                foreach (DataRow row in DataTable.Rows) //assemble a list of the categorical data and the continuous data...
                {
                    continuousRow.Add(row[contFactor].ToString());
                    responseRow.Add(row[response].ToString());
                }

                for (int i = 0; i < DataTable.Rows.Count; i++) //use for loop cos its easier to compare the indexes of the cat and cont rows
                {
                    //Check that there are no responses where the treatments are blank
                    if (String.IsNullOrEmpty(continuousRow[i]) && !String.IsNullOrEmpty(responseRow[i]))
                    {
                        ValidationInfo.AddWarningMessage("The " + factorType + " (" + contFactor + ") contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.");
                    }
                }
            }

            //if got here then all checks ok, return true
            return true;
        }
    }
}