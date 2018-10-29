using SilveRModel.StatsModel;
using System;
using System.Collections.Generic;

namespace SilveRModel.Validators
{
    public class MeansComparisonValidator : ValidatorBase
    {
        private readonly MeansComparisonModel paVariables;

        public MeansComparisonValidator(MeansComparisonModel pa)
            : base(pa.DataTable)
        {
            paVariables = pa;
        }

        public override ValidationInfo Validate()
        {
            if (paVariables.ValueType == MeansComparisonModel.ValueTypeOption.Supplied) //only validate for datasets...
            {
                //check that only st dev or variance is set
                if (String.IsNullOrEmpty(paVariables.StandardDeviation) && String.IsNullOrEmpty(paVariables.Variance))
                {
                    ValidationInfo.AddErrorMessage("You need to set either Standard Deviation or the Variance when using supplied values.");
                    return ValidationInfo;
                }
                else if (!String.IsNullOrEmpty(paVariables.StandardDeviation) && !String.IsNullOrEmpty(paVariables.Variance))
                {
                    ValidationInfo.AddErrorMessage("Both the Standard Deviation AND the Variance have been set - you need to set one or the other but not both.");
                    return ValidationInfo;
                }
            }
            else if (paVariables.ValueType == MeansComparisonModel.ValueTypeOption.Dataset) //only validate for datasets...
            {         
                //go through all the column names, if any are numeric then stop the analysis
                List<string> allVars = new List<string>();
                allVars.Add(paVariables.Treatment);
                allVars.Add(paVariables.Response);
                if (!CheckColumnNames(allVars)) return ValidationInfo;

                if (!String.IsNullOrEmpty(paVariables.Treatment))
                {
                    if (!CheckTreatmentsHaveLevels(paVariables.Treatment)) return ValidationInfo;
                }

                //Check that the response does not contain non-numeric 
                if (!CheckIsNumeric(paVariables.Response))
                {
                    ValidationInfo.AddErrorMessage("The response variable selected (" + paVariables.Response + ") contains non-numerical data that cannot be processed. Please check raw data and make sure the data was entered correctly.");
                    return ValidationInfo;
                }

                if (!CheckResponsesPerLevel(paVariables.Treatment, paVariables.Response)) return ValidationInfo;

                if (!String.IsNullOrEmpty(paVariables.Treatment) && !String.IsNullOrEmpty(paVariables.Response)) //if a treat and response is selected...
                {
                    //Check that the number of responses for each level is at least 2
                    Dictionary<string, int> levelResponses = ResponsesPerLevel(paVariables.Treatment, paVariables.Response);
                    foreach (KeyValuePair<string, int> level in levelResponses)
                    {
                        if (level.Value < 2)
                        {
                            //ValidationInfo.AddErrorMessage("There is no replication in one or more of the levels of the treatment factor. Please select another factor.");
                            ValidationInfo.AddErrorMessage("There is no replication in one or more of the levels of the treatment factor (" + paVariables.Treatment + ").  Please amend the dataset prior to running the analysis.");

                            return ValidationInfo;
                        }
                    }

                    //check response and doses contain values
                    if (!CheckFactorAndResponseNotBlank(paVariables.Treatment, paVariables.Response, "treatment factor")) return ValidationInfo;
                }
                else if (String.IsNullOrEmpty(paVariables.Treatment) && !String.IsNullOrEmpty(paVariables.Response))
                //if only a response selected (doing absolute change) then check that more than 1 value is in the dataset!
                {
                    if (NoOfResponses(paVariables.Response) == 1)
                    {
                        ValidationInfo.AddErrorMessage("The response selected (" + paVariables.Response + ") contains only 1 value. Please select another factor.");
                        return ValidationInfo;
                    }
                }
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}