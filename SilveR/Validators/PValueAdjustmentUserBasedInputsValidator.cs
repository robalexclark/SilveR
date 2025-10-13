using SilveR.StatsModels;
using System.Linq;

namespace SilveR.Validators
{
    public class PValueAdjustmentUserBasedInputsValidator : ValidatorBase
    {
        private readonly PValueAdjustmentUserBasedInputsModel pvVariables;
        private static readonly char[] PValueSeparators = new[] { ',', ' ', '\t', '\r', '\n' };

        public PValueAdjustmentUserBasedInputsValidator(PValueAdjustmentUserBasedInputsModel pv)
        {
            pvVariables = pv;
        }

        public override ValidationInfo Validate()
        {
            string[] pValues = pvVariables.PValues.Split(PValueSeparators, System.StringSplitOptions.RemoveEmptyEntries); //split list by supported separators

            if (pValues.Any(x => x == "<0.001"))
            {
                ValidationInfo.AddWarningMessage("You have entered unadjusted p-value(s) of the form <0.001. For the purposes of the numerical calculations this value has been replaced with 0.00099 and hence the adjusted p-values may be unduly conservative.");
            }
            if (pValues.Any(x => x == "<0.0001"))
            {
                ValidationInfo.AddWarningMessage("You have entered unadjusted p-value(s) of the form <0.0001. For the purposes of the numerical calculations this value has been replaced with 0.000099 and hence the adjusted p-values may be unduly conservative.");
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}