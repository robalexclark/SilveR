using SilveR.StatsModels;

namespace SilveR.Validators
{
    public class PValueAdjustmentUserBasedInputsValidator : ValidatorBase
    {
        private readonly PValueAdjustmentUserBasedInputsModel pvVariables;

        public PValueAdjustmentUserBasedInputsValidator(PValueAdjustmentUserBasedInputsModel pv)
        {
            pvVariables = pv;
        }

        public override ValidationInfo Validate()
        {
            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}