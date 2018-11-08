using SilveR.StatsModels;

namespace SilveR.Validators
{
    public class PValueAdjustmentValidator : ValidatorBase
    {
        private readonly PValueAdjustmentModel pvVariables;

        public PValueAdjustmentValidator(PValueAdjustmentModel pv)
            : base(pv.DataTable)
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