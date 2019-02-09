using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SilveR.Validators
{
    public class NestedDesignAnalysisValidator : ValidatorBase
    {
        private readonly NestedDesignAnalysisModel ndaVariables;

        public NestedDesignAnalysisValidator(NestedDesignAnalysisModel nda)
            : base(nda.DataTable)
        {
            ndaVariables = nda;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.AddVariables(ndaVariables.Treatments);
            allVars.AddVariables(ndaVariables.OtherDesignFactors);
            allVars.AddVariables(ndaVariables.Response);
            allVars.AddVariables(ndaVariables.Covariates);

            if (!CheckColumnNames(allVars)) return ValidationInfo;

            if (!CheckFactorsHaveLevels(ndaVariables.Treatments, true)) return ValidationInfo;

            //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
            if (!CheckResponsesPerLevel(ndaVariables.Treatments, ndaVariables.Response, ReflectionExtensions.GetPropertyDisplayName<NestedDesignAnalysisModel>(i => i.Treatments)))
                return ValidationInfo;

            if (!CheckResponsesPerLevel(ndaVariables.OtherDesignFactors, ndaVariables.Response, ReflectionExtensions.GetPropertyDisplayName<NestedDesignAnalysisModel>(i => i.OtherDesignFactors)))
                return ValidationInfo;

            //First create a list of catogorical variables selected (i.e. as treatments and other factors)
            List<string> categorical = new List<string>();
            categorical.AddVariables(ndaVariables.Treatments);
            categorical.AddVariables(ndaVariables.OtherDesignFactors);

            //do data checks on the treatments/other factors and response
            if (!FactorAndResponseCovariateChecks(categorical, ndaVariables.Response))
                return ValidationInfo;

            //if get here then no errors so return true
            return ValidationInfo;
        }

        private bool FactorAndResponseCovariateChecks(List<string> categorical, string continuous)
        {
            foreach (string catFactor in categorical) //go through each categorical factor and do the check on each
            {
                string factorType;
                if (ndaVariables.Treatments.Contains(catFactor))
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<SingleMeasuresParametricAnalysisModel>(i => i.Treatments);
                }
                else
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<SingleMeasuresParametricAnalysisModel>(i => i.OtherDesignFactors);
                }

                string responseType;
                if (ndaVariables.Response.Contains(continuous))
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
                        ValidationInfo.AddErrorMessage("The " + responseType + " (" + ndaVariables.Response + ") selected contain non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
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
                CheckTransformations(DataTable, ndaVariables.ResponseTransformation, ndaVariables.Response);

                if (ndaVariables.Covariates != null)
                {
                    foreach (string covariate in ndaVariables.Covariates)
                    {
                        CheckTransformations(DataTable, ndaVariables.CovariateTransformation, covariate, true);
                    }
                }
            }

            //if got here then all checks ok, return true
            return true;
        }
    }
}