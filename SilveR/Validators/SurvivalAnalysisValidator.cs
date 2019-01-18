using SilveR.StatsModels;
using System;
using System.Collections.Generic;

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

            //Create a list of categorical variables selected (i.e. the cat factors)
            List<string> categorical = new List<string>();
            if (!String.IsNullOrEmpty(samVariables.Treatment)) categorical.Add(samVariables.Treatment);
            if (!String.IsNullOrEmpty(samVariables.Censorship)) categorical.Add(samVariables.Censorship);

            if (!CheckIsNumeric(samVariables.Response))
            {
                ValidationInfo.AddErrorMessage("The response selected (" + samVariables.Response + ") contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            foreach (string catFactor in categorical) //go through each categorical factor and do the check on each
            {
                //Check that each level has replication
                Dictionary<string, int> levelResponses = ResponsesPerLevel(catFactor, samVariables.Response);
                foreach (KeyValuePair<string, int> level in levelResponses)
                {
                    if (level.Value == 0)
                    {
                        ValidationInfo.AddErrorMessage("Error: There are no observations recorded on the levels of one of the factors. Please amend the dataset prior to running the analysis.");
                        return ValidationInfo;
                    }
                    else if (level.Value < 2)
                    {
                        ValidationInfo.AddErrorMessage("Error: There is no replication in one or more of the levels of the categorical factor (" + catFactor + ").  Please amend the dataset prior to running the analysis.");
                        return ValidationInfo;
                    }
                }

                //check response and cat factors contain values
                if (!CheckFactorAndResponseNotBlank(catFactor, samVariables.Response, "categorisation factor"))
                    return ValidationInfo;
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}