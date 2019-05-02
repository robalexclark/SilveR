using SilveR.StatsModels;

namespace SilveR.Validators
{
    public class PValueAdjustmentDatasetBasedInputsValidator : ValidatorBase
    {
        public PValueAdjustmentDatasetBasedInputsValidator()
        {
        }

        public override ValidationInfo Validate()
        {
            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}