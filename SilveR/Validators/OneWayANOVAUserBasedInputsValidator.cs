using SilveR.StatsModels;
using System;

namespace SilveR.Validators
{
    public class OneWayANOVAUserBasedInputsValidator : ValidatorBase
    {
        private readonly OneWayANOVAUserBasedInputsModel owVariables;

        public OneWayANOVAUserBasedInputsValidator(OneWayANOVAUserBasedInputsModel ow)
        {
            owVariables = ow;
        }

        public override ValidationInfo Validate()
        {
            if (owVariables.EffectSizeEstimate == EffectSizeEstimate.TreatmentMeanSquare)
            {
                if (!owVariables.TreatmentMeanSquare.HasValue)
                {
                    ValidationInfo.AddErrorMessage("Treatment mean square is required");
                    return ValidationInfo;
                }
                else if (!owVariables.NoOfTreatmentGroups.HasValue)
                {
                    ValidationInfo.AddErrorMessage("No of treatment groups is required");
                    return ValidationInfo;
                }
            }
            else if (owVariables.EffectSizeEstimate == EffectSizeEstimate.TreatmentGroupMeans)
            {
                if (String.IsNullOrEmpty(owVariables.Means))
                {
                    ValidationInfo.AddErrorMessage("Means is required");
                    return ValidationInfo;
                }
                else
                {
                    char[] splitters = { ',' };
                    string[] means = owVariables.Means.Replace(" ", "").Split(splitters, StringSplitOptions.RemoveEmptyEntries); //split list by comma

                    foreach (string s in means)//go through list and check that is a number and is greater than 0
                    {
                        double number;
                        if (!double.TryParse(s, out number))
                        {
                            ValidationInfo.AddErrorMessage("Means has non-numeric values or the values are not comma separated");
                            return ValidationInfo;
                        }
                        else if (number < 0)
                        {
                            ValidationInfo.AddErrorMessage("Means has values less than zero");
                            return ValidationInfo;
                        }
                    }
                }
            }

            if (owVariables.VariabilityEstimate == VariabilityEstimate.ResidualMeanSquare && !owVariables.ResidualMeanSquare.HasValue)
            {
                ValidationInfo.AddErrorMessage("Residual mean square is required");
                return ValidationInfo;
            }
            else if (owVariables.VariabilityEstimate == VariabilityEstimate.Variance && !owVariables.Variance.HasValue)
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