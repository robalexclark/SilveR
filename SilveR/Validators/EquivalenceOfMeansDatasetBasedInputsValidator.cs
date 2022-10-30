using SilveR.StatsModels;
using System;
using System.Collections.Generic;

namespace SilveR.Validators
{
    public class EquivalenceOfMeansPowerAnalysisDatasetBasedInputsValidator : ValidatorBase
    {
        private readonly EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel mcVariables;

        public EquivalenceOfMeansPowerAnalysisDatasetBasedInputsValidator(EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel mc)
            : base(mc.DataTable)
        {
            mcVariables = mc;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.Add(mcVariables.Treatment);
            allVars.Add(mcVariables.Response);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            if (!CheckFactorsHaveLevels(mcVariables.Treatment))
                return ValidationInfo;

            //Check that the response does not contain non-numeric 
            if (!CheckIsNumeric(mcVariables.Response))
            {
                ValidationInfo.AddErrorMessage("The Response (" + mcVariables.Response + ") contains non-numerical data that cannot be processed. Please check input data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            //Check that the number of responses for each level is at least 2 
            if (CountResponses(mcVariables.Response) < 2)
            {
                ValidationInfo.AddErrorMessage("The response selected (" + mcVariables.Response + ") contains only 1 value. Please select another response.");
                return ValidationInfo;
            }

            if (mcVariables.Treatment != null && !CheckResponsesPerLevel(mcVariables.Treatment, mcVariables.Response, ReflectionExtensions.GetPropertyDisplayName<EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel>(i => i.Treatment)))
            {
                return ValidationInfo;
            }

            if (!String.IsNullOrEmpty(mcVariables.Treatment) && !String.IsNullOrEmpty(mcVariables.Response)) //if a treat and response is selected...
            {
                //Check that the number of responses for each level is at least 2
                Dictionary<string, int> levelResponses = ResponsesPerLevel(mcVariables.Treatment, mcVariables.Response);
                foreach (KeyValuePair<string, int> level in levelResponses)
                {
                    if (level.Value < 2)
                    {
                        ValidationInfo.AddErrorMessage("There is no replication in one or more of the levels of the Treatment factor (" + mcVariables.Treatment + "). Please amend the dataset prior to running the analysis.");
                        return ValidationInfo;
                    }
                }

                //check response and doses contain values
                if (!CheckFactorAndResponseNotBlank(mcVariables.Treatment, mcVariables.Response, "Treatment factor"))
                    return ValidationInfo;
            }
            else if (mcVariables.Treatment == null && mcVariables.Response != null && CountResponses(mcVariables.Response) == 1) //if only a response selected (doing absolute change) then check that more than 1 value is in the dataset!
            {
                ValidationInfo.AddErrorMessage("The Response (" + mcVariables.Response + ") contains only 1 value. Please select another response.");
                return ValidationInfo;
            }


            if (mcVariables.EquivalenceBoundsType == EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage && String.IsNullOrEmpty(mcVariables.ControlGroup))
            {
                ValidationInfo.AddErrorMessage("You have selected % change from control as the acceptance bound, but as you have not defined the control group it is not possible to calculate the % change.");
                return ValidationInfo;
            }

            if (mcVariables.LowerBoundAbsolute > mcVariables.UpperBoundAbsolute)
            {
                ValidationInfo.AddErrorMessage("The lower bound selected is higher than the upper bound, please check the bounds as the lower bound should be less than the upper bound.");
            }

            if (mcVariables.EquivalenceBoundsType == EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Absolute && (mcVariables.LowerBoundAbsolute.HasValue == false && mcVariables.UpperBoundAbsolute.HasValue == false))
            {
                ValidationInfo.AddErrorMessage("Absolute selected but bounds not entered.");
            }
            else if (mcVariables.EquivalenceBoundsType == EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel.EquivalenceBoundsOption.Percentage && (mcVariables.LowerBoundPercentageChange.HasValue == false && mcVariables.UpperBoundPercentageChange.HasValue == false))
            {
                ValidationInfo.AddErrorMessage("Percentage equivelence bounds selected but no bounds entered.");
            }



            if (mcVariables.PlottingRangeType == PlottingRangeTypeOption.SampleSize)
            {
                if (!mcVariables.SampleSizeFrom.HasValue || !mcVariables.SampleSizeTo.HasValue)
                {
                    ValidationInfo.AddErrorMessage("Sample Size From and To must be set");
                    return ValidationInfo;
                }
                else if (mcVariables.SampleSizeFrom <= 1)
                {
                    ValidationInfo.AddErrorMessage("The sample size selected must be greater than 1.");
                    return ValidationInfo;
                }
                else if (mcVariables.SampleSizeFrom > mcVariables.SampleSizeTo)
                {
                    ValidationInfo.AddErrorMessage("Sample Size To value must be greater than the From value.");
                    return ValidationInfo;
                }
            }
            else if (mcVariables.PlottingRangeType == PlottingRangeTypeOption.Power)
            {
                if (!mcVariables.PowerFrom.HasValue || !mcVariables.PowerTo.HasValue)
                {
                    ValidationInfo.AddErrorMessage("Power From and To must be set");
                    return ValidationInfo;
                }
                else if (mcVariables.PowerFrom > mcVariables.PowerTo)
                {
                    ValidationInfo.AddErrorMessage("Power To value must be greater than the From value.");
                    return ValidationInfo;
                }
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}