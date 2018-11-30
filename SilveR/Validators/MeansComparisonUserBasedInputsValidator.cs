using SilveR.StatsModels;

namespace SilveR.Validators
{
    public class MeansComparisonUserBasedInputsValidator : ValidatorBase
    {
        private readonly MeansComparisonUserBasedInputsModel mcVariables;

        public MeansComparisonUserBasedInputsValidator(MeansComparisonUserBasedInputsModel mc)
        {
            mcVariables = mc;
        }

        public override ValidationInfo Validate()
        {
            //check that only st dev or variance is set
            if (!mcVariables.StandardDeviation.HasValue && !mcVariables.Variance.HasValue)
            {
                ValidationInfo.AddErrorMessage("You need to set either Standard Deviation or the Variance.");
                return ValidationInfo;
            }
            //else if (!mcVariables.StandardDeviation.HasValue && !mcVariables.Variance.HasValue)
            //{
            //    ValidationInfo.AddErrorMessage("Both the Standard Deviation AND the Variance have been set - you need to set one or the other but not both.");
            //    return ValidationInfo;
            //}

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}