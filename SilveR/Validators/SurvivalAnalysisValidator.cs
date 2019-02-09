using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace SilveR.Validators
{
    public class SurvivalAnalysisValidator : ValidatorBase
    {
        private readonly SurvivalAnalysisModel samVariables;

        public SurvivalAnalysisValidator(SurvivalAnalysisModel sam)
            : base(sam.DataTable)
        {
            samVariables = sam;
        }

        public override ValidationInfo Validate()
        {
            //Create a list of all variables
            List<string> allVars = new List<string>();
            allVars.Add(samVariables.Response);
            allVars.Add(samVariables.Treatment);
            allVars.Add(samVariables.Censorship);
            if (!CheckColumnNames(allVars)) return ValidationInfo;


            if (!CheckIsNumeric(samVariables.Response))
            {
                ValidationInfo.AddErrorMessage("The response selected (" + samVariables.Response + ") contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            if (!CheckFactorsHaveLevels(samVariables.Treatment))
                return ValidationInfo;

            if (!CheckResponsesPerLevel(samVariables.Treatment, samVariables.Response, "treatment factor"))
                return ValidationInfo;

            //check response and treatments contain values
            if (!CheckFactorAndResponseNotBlank(samVariables.Treatment, samVariables.Response, "treatment factor"))
                return ValidationInfo;

            foreach (DataRow row in DataTable.Rows)
            {
                if (String.IsNullOrEmpty(row[samVariables.Censorship].ToString()) && !String.IsNullOrEmpty(row[samVariables.Response].ToString()))
                {
                    ValidationInfo.AddErrorMessage("Error: There is a missing value in the censorship variable when there is a corresponding response. Please amend the dataset prior to running the analysis.");
                    return ValidationInfo;
                }
                else if (!String.IsNullOrEmpty(row[samVariables.Censorship].ToString()) && row[samVariables.Censorship].ToString() != "0" && row[samVariables.Censorship].ToString() != "1")
                {
                    ValidationInfo.AddErrorMessage("Error: The censorship variable contains values other than 0 and 1. Please amend the dataset prior to running the analysis.");
                    return ValidationInfo;
                }
                else if (String.IsNullOrEmpty(row[samVariables.Response].ToString()) && !String.IsNullOrEmpty(row[samVariables.Treatment].ToString()) && !String.IsNullOrEmpty(row[samVariables.Censorship].ToString()))
                {
                    ValidationInfo.AddWarningMessage("The response selected (" + samVariables.Response + ") contains missing data.");
                }
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}