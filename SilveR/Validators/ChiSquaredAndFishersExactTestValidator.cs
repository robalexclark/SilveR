using SilveRModel.StatsModel;
using System.Collections.Generic;

namespace SilveRModel.Validators
{
    public class ChiSquaredAndFishersExactTestValidator : ValidatorBase
    {
        private readonly ChiSquaredAndFishersExactTestModel csfetVariables;

        public ChiSquaredAndFishersExactTestValidator(ChiSquaredAndFishersExactTestModel csfet)
            : base(csfet.DataTable)
        {
            csfetVariables = csfet;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.Add(csfetVariables.Response);
            allVars.Add(csfetVariables.GroupingFactor);
            allVars.Add(csfetVariables.ResponseCategories);
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