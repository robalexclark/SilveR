﻿using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SilveR.Validators
{
    public class EquivalenceTOSTTestValidator : ValidatorBase
    {
        private readonly EquivalenceTOSTTestModel smVariables;

        public EquivalenceTOSTTestValidator(EquivalenceTOSTTestModel sm)
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
            
            //First create a list of catogorical variables selected (i.e. as treatments and other factors)
            List<string> categorical = new List<string>();
            categorical.AddVariables(smVariables.Treatments);
            categorical.AddVariables(smVariables.OtherDesignFactors);

            //check that the factors have at least 2 levels
            if (!CheckFactorsHaveLevels(categorical, true))
                return ValidationInfo;

            //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
            if (!CheckResponsesPerLevel(smVariables.Treatments, smVariables.Response, ReflectionExtensions.GetPropertyDisplayName<EquivalenceTOSTTestModel>(i => i.Treatments)))
                return ValidationInfo;

            if (!CheckResponsesPerLevel(smVariables.OtherDesignFactors, smVariables.Response, ReflectionExtensions.GetPropertyDisplayName<EquivalenceTOSTTestModel>(i => i.OtherDesignFactors)))
                return ValidationInfo;

            //do data checks on the treatments/other factors and response
            if (!CategoricalAgainstContinuousVariableChecks(categorical, smVariables.Response))
                return ValidationInfo;

            //check transformations
            CheckTransformations(smVariables.ResponseTransformation, smVariables.Response);

            if (smVariables.Covariates != null)
            {
                CheckTransformations(smVariables.CovariateTransformation, smVariables.Covariates, true);
            }

            //do data checks on the treatments/other factors and covariate (if selected)
            if (smVariables.Covariates != null)
            {
                foreach (string covariate in smVariables.Covariates)
                {
                    if (!CategoricalAgainstContinuousVariableChecks(categorical, covariate))
                        return ValidationInfo;
                }
            }

            if (smVariables.Covariates != null && String.IsNullOrEmpty(smVariables.PrimaryFactor))
            {
                ValidationInfo.AddErrorMessage("You have selected a covariate but no primary factor is selected.");
            }

            if (smVariables.ComparisonsBackToControl && String.IsNullOrEmpty(smVariables.ControlGroup))
            {
                ValidationInfo.AddErrorMessage("You have selected to compare back to a control but no control group is selected.");
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }

        private bool CategoricalAgainstContinuousVariableChecks(List<string> categorical, string continuous)
        {
            foreach (string catFactor in categorical) //go through each categorical factor and do the check on each
            {
                string factorType;
                if (smVariables.Treatments.Contains(catFactor))
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<EquivalenceTOSTTestModel>(i => i.Treatments);
                }
                else
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<EquivalenceTOSTTestModel>(i => i.OtherDesignFactors);
                }

                string responseType;
                if (smVariables.Response.Contains(continuous))
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<EquivalenceTOSTTestModel>(i => i.Response);
                }
                else
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<EquivalenceTOSTTestModel>(i => i.Covariates);
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
                        ValidationInfo.AddErrorMessage("The " + responseType + " (" + continuous + ") contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.");
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