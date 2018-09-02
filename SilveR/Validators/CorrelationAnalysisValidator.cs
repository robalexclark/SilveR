using SilveRModel.StatsModel;
using System;
using System.Collections.Generic;
using System.Data;

namespace SilveRModel.Validators
{
    public class CorrelationAnalysisValidator : ValidatorBase
    {
        private readonly CorrelationAnalysisModel caVariables;

        public CorrelationAnalysisValidator(CorrelationAnalysisModel ca)
            : base(ca.DataTable)
        {
            caVariables = ca;
        }

        public override ValidationInfo Validate()
        {
            //first just check to ensure that the user has actually selected something to output!
            if (!caVariables.Estimate && !caVariables.Statistic && !caVariables.PValue && !caVariables.Scatterplot && !caVariables.Matrixplot && !caVariables.ByCategoriesAndOverall)
            {
                ValidationInfo.AddErrorMessage("You have not selected anything to output!");
                return ValidationInfo;
            }

            //Create a list of all variables
            List<string> allVars = new List<string>();
            allVars.AddRange(caVariables.Responses);
            allVars.Add(caVariables.FirstCatFactor);
            allVars.Add(caVariables.SecondCatFactor);
            allVars.Add(caVariables.ThirdCatFactor);
            allVars.Add(caVariables.FourthCatFactor);


            if (!CheckColumnNames(allVars)) return ValidationInfo;

            //Create a list of categorical variables selected (i.e. the cat factors)
            List<string> categorical = new List<string>();
            if (!String.IsNullOrEmpty(caVariables.FirstCatFactor)) categorical.Add(caVariables.FirstCatFactor);
            if (!String.IsNullOrEmpty(caVariables.SecondCatFactor)) categorical.Add(caVariables.SecondCatFactor);
            if (!String.IsNullOrEmpty(caVariables.ThirdCatFactor)) categorical.Add(caVariables.ThirdCatFactor);
            if (!String.IsNullOrEmpty(caVariables.FourthCatFactor)) categorical.Add(caVariables.FourthCatFactor);

            //Go through each response
            foreach (string response in caVariables.Responses)
            {
                if (!CheckIsNumeric(response))
                {
                    ValidationInfo.AddErrorMessage("The response variable (" + response + ") selected contains non-numerical data. Please amend the dataset prior to running the analysis.");
                    return ValidationInfo;
                }

                foreach (DataRow row in DataTable.Rows)
                {
                    CheckTransformations(row, caVariables.Transformation, response, "response");
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