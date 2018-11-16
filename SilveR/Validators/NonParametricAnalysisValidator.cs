using SilveR.StatsModels;
using System;
using System.Collections.Generic;

namespace SilveR.Validators
{
    public class NonParametricAnalysisValidator : ValidatorBase
    {
        private readonly NonParametricAnalysisModel npVariables;

        public NonParametricAnalysisValidator(NonParametricAnalysisModel np)
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
            allVars.Add(npVariables.OtherDesignFactor);

            if (!CheckColumnNames(allVars)) return ValidationInfo;

            if (!CheckIsNumeric(npVariables.Response))
            {
                ValidationInfo.AddErrorMessage("The response selected (" + npVariables.Response + ") contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            if (!CheckFactorsHaveLevels(npVariables.Treatment)) return ValidationInfo;

            if (!CheckResponsesPerLevel(npVariables.Treatment, npVariables.Response, ReflectionExtensions.GetPropertyDisplayName<NonParametricAnalysisModel>(i => i.Treatment)))
                return ValidationInfo;

            //check response and treatments contain values
            if (!CheckFactorAndResponseNotBlank(npVariables.Treatment, npVariables.Response, ReflectionExtensions.GetPropertyDisplayName<NonParametricAnalysisModel>(i => i.Treatment))
                return ValidationInfo;

            //if only two levels and all treats or to control selected, then need to only do KW
            if (CountDistinctLevels(npVariables.Treatment) == 2 && String.IsNullOrEmpty(npVariables.OtherDesignFactor) && npVariables.AnalysisType != NonParametricAnalysisModel.AnalysisOption.MannWhitney)
            {
                string message = "The treatment factor selected has only two levels so a Mann-Whitney test will be presented.";
                ValidationInfo.AddWarningMessage(message);
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}