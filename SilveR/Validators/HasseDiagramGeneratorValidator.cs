using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SilveR.Validators
{
    public class HasseDiagramGeneratorValidator : ValidatorBase
    {
        private readonly HasseDiagramGeneratorModel hasseDiagramGeneratorVariables;

        public HasseDiagramGeneratorValidator(HasseDiagramGeneratorModel hasseDiagramGenerator)
            : base(hasseDiagramGenerator?.DataTable)
        {
            ArgumentNullException.ThrowIfNull(hasseDiagramGenerator);
            hasseDiagramGeneratorVariables = hasseDiagramGenerator;
        }

        public override ValidationInfo Validate()
        {
            IEnumerable<string> fixedFactors = hasseDiagramGeneratorVariables.FixedFactors ?? Enumerable.Empty<string>();
            IEnumerable<string> randomFactors = hasseDiagramGeneratorVariables.RandomFactors ?? Enumerable.Empty<string>();

            if (!fixedFactors.Any() && !randomFactors.Any())
            {
                ValidationInfo.AddErrorMessage("Enter at least one fixed or random factor.");
                return ValidationInfo;
            }

            if (fixedFactors.Intersect(randomFactors).Any())
            {
                ValidationInfo.AddErrorMessage("A factor cannot be both fixed and random.");
                return ValidationInfo;
            }

            List<string> selectedFactors = fixedFactors.Concat(randomFactors).ToList();
            if (!CheckColumnNames(selectedFactors))
                return ValidationInfo;

            if (!ValidateFactors(fixedFactors, "fixed"))
                return ValidationInfo;

            ValidateFactors(randomFactors, "random");
            return ValidationInfo;
        }

        private bool ValidateFactors(IEnumerable<string> factors, string factorType)
        {
            foreach (string factor in factors)
            {
                int distinctLevels = CountDistinctLevels(factor);
                if (distinctLevels == 0)
                {
                    if (!CheckFactorsHaveLevels(factor, true))
                        return false;
                }
                else if (distinctLevels == 1)
                {
                    ValidationInfo.AddErrorMessage("The " + factorType + " factor " + factor + " contains only identical elements. If this is correct this variable should be removed prior to running the analysis.");
                    return false;
                }

                if (CheckIsNumeric(factor))
                {
                    ValidationInfo.AddWarningMessage("The " + factorType + " factor " + factor + " has numerical levels. Note this factor will be treated as categoric in this assessment.");
                }
            }

            return true;
        }
    }
}
