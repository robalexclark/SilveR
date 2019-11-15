using SilveR.StatsModels;
using System;
using System.Collections.Generic;

namespace SilveR.Validators
{
    public class DoseResponseAndNonLinearRegressionAnalysisValidator : ValidatorBase
    {
        private readonly DoseResponseAndNonLinearRegressionAnalysisModel drnlrVariables;

        public DoseResponseAndNonLinearRegressionAnalysisValidator(DoseResponseAndNonLinearRegressionAnalysisModel drnlr)
            : base(drnlr.DataTable)
        {
            drnlrVariables = drnlr;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.Add(drnlrVariables.Response);
            allVars.Add(drnlrVariables.Dose);
            allVars.Add(drnlrVariables.QCResponse);
            allVars.Add(drnlrVariables.QCDose);
            allVars.Add(drnlrVariables.SamplesResponse);
            allVars.Add(drnlrVariables.EquationYAxis);
            allVars.Add(drnlrVariables.EquationXAxis);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            //Check that all the columns are numeric
            if (!CheckIsNumeric(drnlrVariables.Response))
            {
                ValidationInfo.AddErrorMessage("The Response (" + drnlrVariables.Response + ") contains non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }
            if (!CheckIsNumeric(drnlrVariables.Dose))
            {
                ValidationInfo.AddErrorMessage("The Dose (" + drnlrVariables.Dose + ") contains non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }
            if (!CheckIsNumeric(drnlrVariables.QCResponse))
            {
                ValidationInfo.AddErrorMessage("The QC Response (" + drnlrVariables.QCResponse + ") contains non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }
            if (!CheckIsNumeric(drnlrVariables.QCDose))
            {
                ValidationInfo.AddErrorMessage("The QC Dose (" + drnlrVariables.QCDose + ") contains non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }
            if (!CheckIsNumeric(drnlrVariables.SamplesResponse))
            {
                ValidationInfo.AddErrorMessage("The Sample (" + drnlrVariables.SamplesResponse + ") contains non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            //check that there is more than one value in the response variables
            if (drnlrVariables.AnalysisType == DoseResponseAndNonLinearRegressionAnalysisModel.AnalysisOption.FourParameter)
            {
                if (CountResponses(drnlrVariables.Response) == 1)
                {
                    ValidationInfo.AddErrorMessage("The Response (" + drnlrVariables.Response + ") contains only one value, please choose another response.");
                    return ValidationInfo;
                }

                if (!String.IsNullOrEmpty(drnlrVariables.QCResponse) && CountResponses(drnlrVariables.QCResponse) == 1)
                {
                    ValidationInfo.AddErrorMessage("The QC Response (" + drnlrVariables.Response + ") contains only one value, please choose another QC response.");
                    return ValidationInfo;
                }

                //check response and doses contain values
                if (!CheckFactorAndResponseNotBlank(drnlrVariables.Dose, drnlrVariables.Response, ReflectionExtensions.GetPropertyDisplayName<DoseResponseAndNonLinearRegressionAnalysisModel>(i => i.Dose)))
                    return ValidationInfo;
                if (!CheckFactorAndResponseNotBlank(drnlrVariables.QCDose, drnlrVariables.QCResponse, ReflectionExtensions.GetPropertyDisplayName<DoseResponseAndNonLinearRegressionAnalysisModel>(i => i.QCDose)))
                    return ValidationInfo;

                int noFixedParameters = 0;
                if (drnlrVariables.MinCoeff.HasValue) noFixedParameters++;
                if (drnlrVariables.MaxCoeff.HasValue) noFixedParameters++;
                if (drnlrVariables.SlopeCoeff.HasValue) noFixedParameters++;
                if (drnlrVariables.EDICCoeff.HasValue) noFixedParameters++;

                int noOfLevels = CountDistinctLevels(drnlrVariables.Dose);

                if (noOfLevels == 4 && noFixedParameters < 1)
                {
                    ValidationInfo.AddErrorMessage("As you only have measured responses at four doses you cannot fit the selected dose response curve. You will need to fix at least one of the parameters.");
                    return ValidationInfo;
                }
                else if (noOfLevels == 3 && noFixedParameters < 2)
                {
                    ValidationInfo.AddErrorMessage("As you only have measured responses at three doses you cannot fit the selected dose response curve. You will need to fix at least two of the parameters.");
                    return ValidationInfo;
                }
                else if (noOfLevels == 2 && noFixedParameters < 3)
                {
                    ValidationInfo.AddErrorMessage("As you only have measured responses at two doses you cannot fit the selected dose response curve. You will need to fix at least three of the parameters.");
                    return ValidationInfo;
                }
                else if (noOfLevels == 1 && noFixedParameters < 4)
                {
                    ValidationInfo.AddErrorMessage("As you only have measured responses at one dose you cannot fit the selected dose response curve.");
                    return ValidationInfo;
                }

                //go through each row in the datatable and check transforms are ok
                CheckTransformations(drnlrVariables.ResponseTransformation, drnlrVariables.Response);
                CheckTransformations(drnlrVariables.ResponseTransformation, drnlrVariables.QCResponse);
                CheckTransformations(drnlrVariables.ResponseTransformation, drnlrVariables.SamplesResponse);

                //check that if fixed parameter is set then start value is not set, else add warning...
                bool warningForFixedAndStart = false;
                if (drnlrVariables.MinCoeff.HasValue && drnlrVariables.MinStartValue.HasValue) warningForFixedAndStart = true;
                else if (drnlrVariables.MaxCoeff.HasValue && drnlrVariables.MaxStartValue.HasValue) warningForFixedAndStart = true;
                else if (drnlrVariables.SlopeCoeff.HasValue && drnlrVariables.SlopeStartValue.HasValue) warningForFixedAndStart = true;
                else if (drnlrVariables.EDICCoeff.HasValue && drnlrVariables.EDICStartValue.HasValue) warningForFixedAndStart = true;

                if (warningForFixedAndStart)
                {
                    ValidationInfo.AddWarningMessage("You have defined a start value for a parameter that has been fixed. The start value is ignored in the analysis.");
                }

                if (drnlrVariables.MinCoeff > drnlrVariables.MaxCoeff)
                {
                    ValidationInfo.AddErrorMessage("The max coefficient is greater than the min coefficient.");
                    return ValidationInfo;
                }

                if (drnlrVariables.MinStartValue > drnlrVariables.MaxStartValue)
                {
                    ValidationInfo.AddErrorMessage("The max start value is greater than the min start value.");
                    return ValidationInfo;
                }
            }
            else if (drnlrVariables.AnalysisType == DoseResponseAndNonLinearRegressionAnalysisModel.AnalysisOption.Equation && !drnlrVariables.Equation.Contains("x"))
            {
                ValidationInfo.AddErrorMessage("The formula should be of the form f=y(x) with x lower case.");
                return ValidationInfo;
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}