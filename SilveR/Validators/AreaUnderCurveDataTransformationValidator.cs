using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static SilveR.StatsModels.AreaUnderCurveDataTransformationModel;

namespace SilveR.Validators
{
    public class AreaUnderCurveDataTransformationValidator : ValidatorBase
    {
        private readonly AreaUnderCurveDataTransformationModel aucVariables;

        public AreaUnderCurveDataTransformationValidator(AreaUnderCurveDataTransformationModel auc)
            : base(auc.DataTable)
        {
            aucVariables = auc;
        }

        public override ValidationInfo Validate()
        {
            if (aucVariables.SelectedInputFormat == InputFormatType.RepeatedMeasuresFormat)
            {
                if (aucVariables.Response == null)
                {
                    ValidationInfo.AddErrorMessage("Response is required.");
                    return ValidationInfo;
                }
                else if (aucVariables.SubjectFactor == null)
                {
                    ValidationInfo.AddErrorMessage("Subject Factor is required.");
                    return ValidationInfo;
                }
                else if (aucVariables.TimeFactor == null)
                {
                    ValidationInfo.AddErrorMessage("Time Factor is required.");
                    return ValidationInfo;
                }
            }
            else if (aucVariables.SelectedInputFormat == InputFormatType.SingleMeasuresFormat)
            {
                if (aucVariables.Responses == null)
                {
                    ValidationInfo.AddErrorMessage("Responses are required.");
                    return ValidationInfo;
                }
                else if (aucVariables.NumericalTimePoints == null)
                {
                    ValidationInfo.AddErrorMessage("Numerical Timepoints are required.");
                    return ValidationInfo;
                }
            }


            //Create a list of all variables
            List<string> allVars = new List<string>();

            if (aucVariables.Response != null)
                allVars.Add(aucVariables.Response);

            if (aucVariables.SubjectFactor != null)
                allVars.Add(aucVariables.SubjectFactor);

            if (aucVariables.TimeFactor != null)
                allVars.Add(aucVariables.TimeFactor);

            if (aucVariables.Responses != null)
                allVars.AddRange(aucVariables.Responses);


            if (aucVariables.SelectedVariables != null)
                allVars.AddRange(aucVariables.SelectedVariables);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            if (aucVariables.Response != null)
            {
                if (!CheckIsNumeric(aucVariables.Response))
                {
                    ValidationInfo.AddErrorMessage("The Response (" + aucVariables.Response + ") contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.");
                    return ValidationInfo;
                }

                if (!CheckFactorsHaveLevels(aucVariables.Response))
                    return ValidationInfo;
            }

            if (aucVariables.TimeFactor != null)
            {
                if (!CheckIsNumeric(aucVariables.TimeFactor))
                {
                    ValidationInfo.AddErrorMessage("The TimeFactor (" + aucVariables.TimeFactor + ") contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.");
                    return ValidationInfo;
                }
            }


            if (aucVariables.Responses != null)
            {
                foreach (string response in aucVariables.Responses)
                {
                    if (!CheckIsNumeric(response))
                    {
                        ValidationInfo.AddErrorMessage("The Response (" + response + ") contains non-numeric data that cannot be processed. Please check the data and make sure it was entered correctly.");
                        return ValidationInfo;
                    }

                    if (!CheckFactorsHaveLevels(response))
                        return ValidationInfo;
                }
            }


            //check that the number of numerical timepoint are the same as the number of responses
            if (aucVariables.Responses != null)
            {
                string[] split = aucVariables.NumericalTimePoints?.Split(',');

                if (aucVariables.Responses.Count() != split.Length)
                {
                    ValidationInfo.AddErrorMessage("The number of numerical timepoints has to equal the number of response variables selected.");
                    return ValidationInfo;
                }

                double lastNo = Double.MinValue;
                foreach (var s in split)
                {
                    bool isNumeric = Double.TryParse(s, out double numValue);
                    if (!isNumeric)
                    {
                        ValidationInfo.AddErrorMessage("One or more of the numerical timepoints is not numeric.");
                        return ValidationInfo;
                    }
                    else if (numValue < lastNo)
                    {
                        ValidationInfo.AddErrorMessage("The timepoints are not monotonically increasing.");
                        return ValidationInfo;

                    }

                    lastNo = numValue;
                }
            }

            if (aucVariables.SelectedInputFormat == InputFormatType.RepeatedMeasuresFormat)
            {
                if (aucVariables.AUCOutput == AUCOutputType.AUCFromInitialTimepoint && aucVariables.TimeFactor.Length == 1)
                {
                    ValidationInfo.AddErrorMessage("Only one timepoint found.");
                    return ValidationInfo;
                }

                if (!CheckFactorAndResponseNotBlank(aucVariables.SubjectFactor, aucVariables.Response, "Subject factor"))
                    return ValidationInfo;

                if (!CheckFactorAndResponseNotBlank(aucVariables.TimeFactor, aucVariables.Response, "Time factor"))
                    return ValidationInfo;

            }
            else if (aucVariables.SelectedInputFormat == InputFormatType.SingleMeasuresFormat)
            {
                if (aucVariables.Responses.Count() == 1)
                {
                    ValidationInfo.AddErrorMessage("Only one response found.");
                    return ValidationInfo;
                }
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}