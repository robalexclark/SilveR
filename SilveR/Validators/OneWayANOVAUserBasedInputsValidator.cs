using SilveR.StatsModels;

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
            //check that only st dev or variance is set
            //if (!owVariables.StandardDeviation.HasValue && !owVariables.Variance.HasValue)
            //{
            //    ValidationInfo.AddErrorMessage("You need to set either Standard Deviation or the Variance.");
            //    return ValidationInfo;
            //}

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}