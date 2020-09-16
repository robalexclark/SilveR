using SilveR.StatsModels;
using System.Collections.Generic;

namespace SilveR.Validators
{
    public class SingleMeasuresToRepeatedMeasuresDataTransformationValidator : ValidatorBase
    {
        private readonly SingleMeasuresToRepeatedMeasuresDataTransformationModel srtVariables;

        public SingleMeasuresToRepeatedMeasuresDataTransformationValidator(SingleMeasuresToRepeatedMeasuresDataTransformationModel srt)
            : base(srt.DataTable)
        {
            srtVariables = srt;
        }

        public override ValidationInfo Validate()
        {
            //Create a list of all variables
            List<string> allVars = new List<string>();
            allVars.AddRange(srtVariables.Responses);
            allVars.Add(srtVariables.SubjectFactor);
            allVars.AddRange(srtVariables.SelectedVariables);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            foreach (string response in srtVariables.Responses)
            {
                if (!CheckIsNumeric(response))
                {
                    ValidationInfo.AddErrorMessage("The Response (" + response + ") contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.");
                    return ValidationInfo;
                }

                if (!CheckFactorsHaveLevels(response))
                    return ValidationInfo;
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}