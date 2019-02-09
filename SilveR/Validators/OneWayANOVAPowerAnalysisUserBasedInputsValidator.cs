using SilveR.StatsModels;
using System;

namespace SilveR.Validators
{
    public class OneWayANOVAPowerAnalysisUserBasedInputsValidator : ValidatorBase
    {
        private readonly OneWayANOVAPowerAnalysisUserBasedInputsModel owVariables;

        public OneWayANOVAPowerAnalysisUserBasedInputsValidator(OneWayANOVAPowerAnalysisUserBasedInputsModel ow)
        {
            owVariables = ow;
        }

        public override ValidationInfo Validate()
        {
            char[] splitters = { ',' };
            string[] means = owVariables.Means.Replace(" ", "").Split(splitters, StringSplitOptions.None); //split list by comma

            foreach (string s in means)//go through list and check that is a number and is greater than 0
            {
                if (String.IsNullOrWhiteSpace(s))
                {
                    ValidationInfo.AddErrorMessage("The list of means contains missing values, please remove any blank entries between the comma separated means");
                    return ValidationInfo;
                }
                else
                {
                    double number;
                    if (!double.TryParse(s, out number))
                    {
                        ValidationInfo.AddErrorMessage("Means has non-numeric values or the values are not comma separated");
                        return ValidationInfo;
                    }
                }
            }

            if (owVariables.VariabilityEstimate == VariabilityEstimate.Variance && !owVariables.Variance.HasValue)
            {
                ValidationInfo.AddErrorMessage("Variance is required");
                return ValidationInfo;
            }
            else if (owVariables.VariabilityEstimate == VariabilityEstimate.StandardDeviation && !owVariables.StandardDeviation.HasValue)
            {
                ValidationInfo.AddErrorMessage("Standard deviation is required");
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
                    ValidationInfo.AddErrorMessage("Sample Size To value must be greater than the From value");
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
                    ValidationInfo.AddErrorMessage("Power To value must be greater than the From value");
                    return ValidationInfo;
                }
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}