using SilveR.StatsModels;

namespace SilveR.Validators
{
    public class PValueAdjustmentUserBasedInputsValidator : ValidatorBase
    {
        public PValueAdjustmentUserBasedInputsValidator()
        {
        }

        public override ValidationInfo Validate()
        {
            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}