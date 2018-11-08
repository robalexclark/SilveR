using SilveR.StatsModels;
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
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.Add(samVariables.Response);
            allVars.Add(samVariables.Grouping);
            allVars.Add(samVariables.Censorship);
            if (!CheckColumnNames(allVars)) return ValidationInfo;

            //if (!CheckIsNumeric(csfetVariables.Response))
            //{
            //    ValidationInfo.AddErrorMessage("The response selected contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.");
            //    return ValidationInfo;
            //}

            //if (!CheckTreatmentsHaveLevels(csfetVariables.Treatment)) return ValidationInfo;

            //if (!CheckResponsesPerLevel(csfetVariables.Treatment, csfetVariables.Response)) return ValidationInfo;

            ////check response and treatments contain values
            //if (!CheckResponseAndTreatmentsNotBlank(csfetVariables.Response, csfetVariables.Treatment, "treatment factor")) return ValidationInfo;

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}