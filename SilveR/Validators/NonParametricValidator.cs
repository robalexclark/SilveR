using System.Collections.Generic;
using SilveRModel.StatsModel;

namespace SilveRModel.Validators
{
    public class NonParametricsValidator : ValidatorBase
    {
        private readonly NonParametricModel npVariables;

        public NonParametricsValidator(NonParametricModel np)
            : base(np.DataTable)
        {
            npVariables = np;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.Add(npVariables.Treatment);
            allVars.Add(npVariables.Response);
            if (!CheckColumnNames(allVars)) return ValidationInfo;

            if (!CheckIsNumeric(npVariables.Response))
            {
                ValidationInfo.AddErrorMessage("The response selected contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            if (!CheckTreatmentsHaveLevels(npVariables.Treatment)) return ValidationInfo;

            if (!CheckResponsesPerLevel(npVariables.Treatment, npVariables.Response)) return ValidationInfo;

            //check response and treatments contain values
            if (!CheckResponseAndTreatmentsNotBlank(npVariables.Response, npVariables.Treatment, "treatment factor")) return ValidationInfo;

            //if only two levels and all treats or to control selected, then need to only do KW
            if (CountDistinctLevels(npVariables.Treatment) == 2 && npVariables.AnalysisType != NonParametricModel.AnalysisOption.MannWhitney)
            {
                string message = "The treatment factor selected has only two levels so a Mann-Whitney test will be presented.";
                ValidationInfo.AddWarningMessage(message);
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}