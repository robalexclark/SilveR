using SilveRModel.StatsModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace SilveRModel.Validators
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
            allVars.AddRange(ssVariables.Responses);
            allVars.Add(ssVariables.FirstCatFactor);
            allVars.Add(ssVariables.SecondCatFactor);
            allVars.Add(ssVariables.ThirdCatFactor);
            allVars.Add(ssVariables.FourthCatFactor);

            //check each variable is only used once
            //var variableCounts = allVars.GroupBy(g => g).Select(x => new { VarName = x.Key, Count = x.Count() });
            //if (variableCounts.Any(x => x.VarName != null && x.Count > 1))
            //{
            //    IEnumerable<string> dodgyVars = variableCounts.Where(x => x.VarName != null && x.Count > 1).Select(x => x.VarName);
            //    ValidationInfo.AddErrorMessage("The following variable(s) (" + String.Join(", ", dodgyVars) + " are selected more than once. Please amend your selections prior to running the analysis.");
            //    return ValidationInfo;
            //}

            if (!CheckColumnNames(allVars)) return ValidationInfo;

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
                    ValidationInfo.AddErrorMessage("The response variable (" + response + ") selected contains non-numerical data. Please amend the dataset prior to running the analysis.");
                    return ValidationInfo;
                }

                foreach (DataRow row in DataTable.Rows)
                {
                    CheckTransformations(row, ssVariables.Transformation, response, "response");
                }

                foreach (string catFactor in categorical) //go through each categorical factor and do the check on each
                {
                    //Check that each level has replication
                    Dictionary<string, int> levelResponses = ResponsesPerLevel(catFactor, response);
                    foreach (KeyValuePair<string, int> level in levelResponses)
                    {
                        if (level.Value == 0)
                        {
                            ValidationInfo.AddErrorMessage("There are no observations recorded on the levels of one of the factors. Please amend the dataset prior to running the analysis.");
                            return ValidationInfo;
                        }
                        else if (level.Value < 2)
                        {
                            ValidationInfo.AddErrorMessage("There is no replication in one or more of the levels of the categorical factor (" + catFactor + ").  Please amend the dataset prior to running the analysis.");
                            return ValidationInfo;
                        }
                    }

                    //check response and cat factors contain values
                    if (!CheckResponseAndTreatmentsNotBlank(response, catFactor, "categorisation factor"))
                        return ValidationInfo;
                }
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}