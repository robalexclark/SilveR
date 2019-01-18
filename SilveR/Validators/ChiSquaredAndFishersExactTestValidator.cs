using SilveR.StatsModels;
using System.Collections.Generic;
using System.Linq;

namespace SilveR.Validators
{
    public class ChiSquaredAndFishersExactTestValidator : ValidatorBase
    {
        private readonly ChiSquaredAndFishersExactTestModel csfetVariables;

        public ChiSquaredAndFishersExactTestValidator(ChiSquaredAndFishersExactTestModel csfet)
            : base(csfet.DataTable)
        {
            csfetVariables = csfet;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.Add(csfetVariables.Response);
            allVars.Add(csfetVariables.GroupingFactor);
            allVars.Add(csfetVariables.ResponseCategories);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            if (!CheckIsNumeric(csfetVariables.Response))
            {
                ValidationInfo.AddErrorMessage("The response selected contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            if (!CheckFactorAndResponseNotBlank(csfetVariables.GroupingFactor, csfetVariables.Response, ReflectionExtensions.GetPropertyDisplayName<ChiSquaredAndFishersExactTestModel>(i => i.GroupingFactor))) return ValidationInfo;

            if (!CheckFactorAndResponseNotBlank(csfetVariables.ResponseCategories, csfetVariables.Response, ReflectionExtensions.GetPropertyDisplayName<ChiSquaredAndFishersExactTestModel>(i => i.ResponseCategories))) return ValidationInfo;

            if (csfetVariables.BarnardsTest && (GetLevels(csfetVariables.GroupingFactor).Count() > 2 || GetLevels(csfetVariables.ResponseCategories).Count() > 2))
            {
                ValidationInfo.AddWarningMessage("Grouping factor or the Response categories have more than two levels. Barnard's test can only be performed when there are two levels of the Grouping factor and two Response categories");
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}