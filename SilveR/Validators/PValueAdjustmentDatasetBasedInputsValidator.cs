using SilveR.StatsModels;

namespace SilveR.Validators
{
    public class PValueAdjustmentDatasetBasedInputsValidator : ValidatorBase
    {
        private readonly PValueAdjustmentDatasetBasedInputsModel pvVariables;

        public PValueAdjustmentDatasetBasedInputsValidator(PValueAdjustmentDatasetBasedInputsModel pv)
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