using SilveR.StatsModels;

namespace SilveR.Validators
{
    public class EquivalenceOfMeansPowerAnalysisUserBasedInputsValidator : ValidatorBase
    {
        private readonly EquivalenceOfMeansPowerAnalysisUserBasedInputsModel mcVariables;

        public EquivalenceOfMeansPowerAnalysisUserBasedInputsValidator(EquivalenceOfMeansPowerAnalysisUserBasedInputsModel mc)
        {
            mcVariables = mc;
        }

        public override ValidationInfo Validate()
        {
            if (mcVariables.DeviationType == DeviationType.StandardDeviation && !mcVariables.StandardDeviation.HasValue)
            {
                ValidationInfo.AddErrorMessage("Standard Deviation is required");
                return ValidationInfo;
            }
            else if (mcVariables.DeviationType == DeviationType.Variance && !mcVariables.Variance.HasValue)
            {
                ValidationInfo.AddErrorMessage("Variance is required");
                return ValidationInfo;
            }

            if (mcVariables.LowerBoundAbsolute > mcVariables.UpperBoundAbsolute)
            {
                ValidationInfo.AddErrorMessage("The lower bound selected is higher than the upper bound, please check the bounds as the lower bound should be less than the upper bound.");
            }

            if (mcVariables.EquivalenceBoundsType == EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Absolute && (mcVariables.LowerBoundAbsolute.HasValue == false && mcVariables.UpperBoundAbsolute.HasValue == false))
            {
                ValidationInfo.AddErrorMessage("Absolute selected but bounds not entered.");
            }
            else if (mcVariables.EquivalenceBoundsType == EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Percentage && (mcVariables.LowerBoundPercentageChange.HasValue == false && mcVariables.UpperBoundPercentageChange.HasValue == false))
            {
                ValidationInfo.AddErrorMessage("Percentage equivelence bounds selected but no bounds entered.");
            }
            else if (mcVariables.EquivalenceBoundsType == EquivalenceOfMeansPowerAnalysisUserBasedInputsModel.EquivalenceBoundsOption.Percentage && mcVariables.ObservedDifference.HasValue == false)
            {
                ValidationInfo.AddErrorMessage("You have selected percentage changes from observed difference, but as you have not defined the observed difference it is not possible to calculate percentage changes.");
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