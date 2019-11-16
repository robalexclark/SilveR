using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace SilveR.Validators
{
    public class OneWayANOVAPowerAnalysisDatasetBasedInputsValidator : ValidatorBase
    {
        private readonly OneWayANOVAPowerAnalysisDatasetBasedInputsModel owVariables;

        public OneWayANOVAPowerAnalysisDatasetBasedInputsValidator(OneWayANOVAPowerAnalysisDatasetBasedInputsModel ow)
            : base(ow.DataTable)
        {
            owVariables = ow;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.Add(owVariables.Treatment);
            allVars.Add(owVariables.Response);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            if (!CheckFactorsHaveLevels(owVariables.Treatment))
                return ValidationInfo;

            //Check that the response does not contain non-numeric 
            if (!CheckIsNumeric(owVariables.Response))
            {
                ValidationInfo.AddErrorMessage("The Response (" + owVariables.Response + ") contains non-numerical data that cannot be processed. Please check input data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            if (owVariables.Treatment != null && !CheckResponsesPerLevel(owVariables.Treatment, owVariables.Response, ReflectionExtensions.GetPropertyDisplayName<OneWayANOVAPowerAnalysisDatasetBasedInputsModel>(i => i.Treatment)))
                return ValidationInfo;

            if (owVariables.Treatment != null && owVariables.Response != null) //if a treat and response is selected...
            {
                //Check that the number of responses for each level is at least 2
                Dictionary<string, int> levelResponses = ResponsesPerLevel(owVariables.Treatment, owVariables.Response);
                foreach (KeyValuePair<string, int> level in levelResponses)
                {
                    if (level.Value < 2)
                    {
                        ValidationInfo.AddErrorMessage("There is no replication in one or more of the levels of the Treatment factor (" + owVariables.Treatment + ").  Please amend the dataset prior to running the analysis.");
                        return ValidationInfo;
                    }
                }

                //check response and doses contain values
                if (!CheckFactorAndResponseNotBlank(owVariables.Treatment, owVariables.Response, ReflectionExtensions.GetPropertyDisplayName<OneWayANOVAPowerAnalysisDatasetBasedInputsModel>(i => i.Treatment)))
                    return ValidationInfo;
            }
            else if (owVariables.Response != null && CountResponses(owVariables.Response) == 1) //if only a response selected (doing absolute change) then check that more than 1 value is in the dataset!
            {
                ValidationInfo.AddErrorMessage("The Response (" + owVariables.Response + ") contains only 1 value. Please select another factor.");
                return ValidationInfo;
            }

            if (owVariables.PlottingRangeType == PlottingRangeTypeOption.SampleSize)
            {
                if (!owVariables.SampleSizeFrom.HasValue || !owVariables.SampleSizeTo.HasValue)
                {
                    ValidationInfo.AddErrorMessage("Sample Size From and To must be set");
                    return ValidationInfo;
                }
                else if (owVariables.SampleSizeFrom > owVariables.SampleSizeTo)
                {
                    ValidationInfo.AddErrorMessage("Sample Size To value must be greater than the From value.");
                    return ValidationInfo;
                }
            }
            else if (owVariables.PlottingRangeType == PlottingRangeTypeOption.Power)
            {
                if (!owVariables.PowerFrom.HasValue || !owVariables.PowerTo.HasValue)
                {
                    ValidationInfo.AddErrorMessage("Power From and To must be set");
                    return ValidationInfo;
                }
                else if (owVariables.PowerFrom > owVariables.PowerTo)
                {
                    ValidationInfo.AddErrorMessage("Power To value must be greater than the From value.");
                    return ValidationInfo;
                }
            }

            //check transformations
            CheckTransformations(owVariables.ResponseTransformation, owVariables.Response);

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}