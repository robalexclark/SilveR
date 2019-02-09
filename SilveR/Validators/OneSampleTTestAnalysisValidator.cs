using SilveR.StatsModels;

namespace SilveR.Validators
{
    public class OneSampleTTestAnalysisValidator : ValidatorBase
    {
        private readonly OneSampleTTestAnalysisModel osttVariables;

        public OneSampleTTestAnalysisValidator(OneSampleTTestAnalysisModel ostt)
            : base(ostt.DataTable)
        {
            osttVariables = ostt;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            if (!CheckColumnNames(osttVariables.Responses))
                return ValidationInfo;

            //Go through each response
            foreach (string response in osttVariables.Responses)
            {
                if (!CheckIsNumeric(response))
                {
                    ValidationInfo.AddErrorMessage("The response variable selected (" + response + ") contains non-numerical data. Please amend the dataset prior to running the analysis.");
                    return ValidationInfo;
                }

                if (CountResponses(response) <= 1)
                {
                    ValidationInfo.AddErrorMessage("Error: There is no replication in the response variable (" + response + "). Please select another factor.");
                    return ValidationInfo;
                }

                CheckTransformations(DataTable, osttVariables.ResponseTransformation, response);
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}