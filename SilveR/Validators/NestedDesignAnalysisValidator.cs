using SilveRModel.StatsModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace SilveRModel.Validators
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
            allVars.AddRange(ndaVariables.Treatments);

            if (ndaVariables.OtherDesignFactors != null)
                allVars.AddRange(ndaVariables.OtherDesignFactors);

            allVars.Add(ndaVariables.Response);
            allVars.Add(ndaVariables.Covariate);
            if (!CheckColumnNames(allVars)) return ValidationInfo;

            if (!CheckTreatmentsHaveLevels(ndaVariables.Treatments, true)) return ValidationInfo;

            //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
            if (!CheckResponsesPerLevel(ndaVariables.Treatments, ndaVariables.Response)) return ValidationInfo;

            if (ndaVariables.OtherDesignFactors != null)
                if (!CheckResponsesPerLevel(ndaVariables.OtherDesignFactors, ndaVariables.Response, "other treatment")) return ValidationInfo;

            //First create a list of catogorical variables selected (i.e. as treatments and other factors)
            List<string> categorical = new List<string>();
            categorical.AddRange(ndaVariables.Treatments);

            if (ndaVariables.OtherDesignFactors != null)
                categorical.AddRange(ndaVariables.OtherDesignFactors);

            //do data checks on the treatments/other factors and response
            if (!FactorAndResponseCovariateChecks(categorical, ndaVariables.Response)) return ValidationInfo;

            //do data checks on the treatments/other factors and covariate (if selected)
            if (!String.IsNullOrEmpty(ndaVariables.Covariate))
            {
                if (!FactorAndResponseCovariateChecks(categorical, ndaVariables.Covariate)) return ValidationInfo;
            }

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
                    factorType = "treatment";
                }
                else
                {
                    factorType = "other factor";
                }

                string responseType;
                if (ndaVariables.Response.Contains(continuous))
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
                foreach (DataRow row in DataTable.Rows)
                {
                    CheckTransformations(row, ndaVariables.ResponseTransformation, ndaVariables.Response, "response");

                    CheckTransformations(row, ndaVariables.CovariateTransformation, ndaVariables.Covariate, "covariate");
                }
            }

            //if got here then all checks ok, return true
            return true;
        }
    }
}