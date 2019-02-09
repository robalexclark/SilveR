using SilveR.StatsModels;
using System;
using System.Collections.Generic;

namespace SilveR.Validators
{
    public class SummaryStatisticsValidator : ValidatorBase
    {
        private readonly SummaryStatisticsModel ssVariables;

        public SummaryStatisticsValidator(SummaryStatisticsModel ss)
            : base(ss.DataTable)
        {
            ssVariables = ss;
        }

        public override ValidationInfo Validate()
        {
            //first just check to ensure that the user has actually selected something to output!
            if (!ssVariables.Mean && !ssVariables.N && !ssVariables.StandardDeviation && !ssVariables.Variance && !ssVariables.StandardErrorOfMean && !ssVariables.MinAndMax && !ssVariables.MedianAndQuartiles && !ssVariables.CoefficientOfVariation
                && !ssVariables.NormalProbabilityPlot && !ssVariables.CoefficientOfVariation && !ssVariables.ByCategoriesAndOverall)
            {
                ValidationInfo.AddErrorMessage("You have not selected anything to output!");
                return ValidationInfo;
            }

            //Create a list of all variables
            List<string> allVars = new List<string>();
            allVars.AddVariables(ssVariables.Responses);
            allVars.AddVariables(ssVariables.FirstCatFactor);
            allVars.AddVariables(ssVariables.SecondCatFactor);
            allVars.AddVariables(ssVariables.ThirdCatFactor);
            allVars.AddVariables(ssVariables.FourthCatFactor);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            //Create a list of categorical variables selected (i.e. the cat factors)
            List<string> categorical = new List<string>();
            if (!String.IsNullOrEmpty(ssVariables.FirstCatFactor)) categorical.Add(ssVariables.FirstCatFactor);
            if (!String.IsNullOrEmpty(ssVariables.SecondCatFactor)) categorical.Add(ssVariables.SecondCatFactor);
            if (!String.IsNullOrEmpty(ssVariables.ThirdCatFactor)) categorical.Add(ssVariables.ThirdCatFactor);
            if (!String.IsNullOrEmpty(ssVariables.FourthCatFactor)) categorical.Add(ssVariables.FourthCatFactor);


            //Go through each response
            foreach (string response in ssVariables.Responses)
            {
                if (!CheckIsNumeric(response))
                {
                    ValidationInfo.AddErrorMessage("The response variable selected (" + response + ") contains non-numerical data. Please amend the dataset prior to running the analysis.");
                    return ValidationInfo;
                }

                CheckTransformations(DataTable, ssVariables.Transformation, response);

                if (!CheckResponsesPerLevel(categorical, response, ReflectionExtensions.GetPropertyDisplayName<SummaryStatisticsModel>(i => i.Responses)))
                    return ValidationInfo;

                //check response and cat factors contain values
                if (!CheckFactorsAndResponseNotBlank(categorical, response, "categorisation factor"))
                    return ValidationInfo;
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}