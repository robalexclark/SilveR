using SilveR.StatsModels;
using System;
using System.Collections.Generic;

namespace SilveR.Validators
{
    public class DoseResponseAndNonLinearRegesssionAnalysisValidator : ValidatorBase
    {
        private readonly DoseResponseAndNonLinearRegesssionAnalysisModel drnlrVariables;

        public DoseResponseAndNonLinearRegesssionAnalysisValidator(DoseResponseAndNonLinearRegesssionAnalysisModel drnlr)
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
                ValidationInfo.AddErrorMessage("Error: The response variable selected contain non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }
            if (!CheckIsNumeric(drnlrVariables.Dose))
            {
                ValidationInfo.AddErrorMessage("Error: The dose variable selected contain non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }
            if (!CheckIsNumeric(drnlrVariables.QCResponse))
            {
                ValidationInfo.AddErrorMessage("Error: The QC response variable selected contain non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }
            if (!CheckIsNumeric(drnlrVariables.QCDose))
            {
                ValidationInfo.AddErrorMessage("Error: The QC dose variable selected contain non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }
            if (!CheckIsNumeric(drnlrVariables.SamplesResponse))
            {
                ValidationInfo.AddErrorMessage("Error: The sample variable selected contain non-numeric data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            //check that there is more than one value in the response variables
            if (drnlrVariables.AnalysisType == DoseResponseAndNonLinearRegesssionAnalysisModel.AnalysisOption.FourParameter)
            {
                if (CountResponses(drnlrVariables.Response) == 1)
                {
                    ValidationInfo.AddErrorMessage("Error: The response variable contains only one value, please choose another response.");
                    return ValidationInfo;
                }

                if (!String.IsNullOrEmpty(drnlrVariables.QCResponse) && CountResponses(drnlrVariables.QCResponse) == 1)
                {
                    ValidationInfo.AddErrorMessage("Error: The QC response variable contains only one value, please choose another QC response.");
                    return ValidationInfo;
                }

                //check response and doses contain values
                if (!CheckFactorAndResponseNotBlank(drnlrVariables.Dose, drnlrVariables.Response, "dose variable "))
                    return ValidationInfo;
                if (!CheckFactorAndResponseNotBlank(drnlrVariables.QCDose, drnlrVariables.QCResponse, "QC dose variable"))
                    return ValidationInfo;

                int noFixedParameters = 0;
                if (drnlrVariables.MinCoeff.HasValue) noFixedParameters++;
                if (drnlrVariables.MaxCoeff.HasValue) noFixedParameters++;
                if (drnlrVariables.SlopeCoeff.HasValue) noFixedParameters++;
                if (drnlrVariables.EDICCoeff.HasValue) noFixedParameters++;

                int noOfLevels = CountDistinctLevels(drnlrVariables.Dose);

                if (noOfLevels == 4 && noFixedParameters < 1)
                {
                    ValidationInfo.AddErrorMessage("Error: As you only have measured responses at four doses you cannot fit the selected dose response curve. You will need to fix at least one of the parameters.");
                    return ValidationInfo;
                }
                else if (noOfLevels == 3 && noFixedParameters < 2)
                {
                    ValidationInfo.AddErrorMessage("Error: As you only have measured responses at three doses you cannot fit the selected dose response curve. You will need to fix at least two of the parameters.");
                    return ValidationInfo;
                }
                else if (noOfLevels == 2 && noFixedParameters < 3)
                {
                    ValidationInfo.AddErrorMessage("Error: As you only have measured responses at two doses you cannot fit the selected dose response curve. You will need to fix at least three of the parameters.");
                    return ValidationInfo;
                }
                else if (noOfLevels == 1 && noFixedParameters < 4)
                {
                    ValidationInfo.AddErrorMessage("Error: As you only have measured responses at one dose you cannot fit the selected dose response curve.");
                    return ValidationInfo;
                }

                //go through each row in the datatable and check transforms are ok
                CheckTransformations(DataTable, drnlrVariables.ResponseTransformation, drnlrVariables.Response);
                CheckTransformations(DataTable, drnlrVariables.ResponseTransformation, drnlrVariables.QCResponse);
                CheckTransformations(DataTable, drnlrVariables.ResponseTransformation, drnlrVariables.SamplesResponse);

                //check that if fixed parameter is set then start value is not set, else add warning...
                bool warningFoxFixedAndStart = false;
                if (drnlrVariables.MinCoeff.HasValue && drnlrVariables.MinStartValue.HasValue) warningFoxFixedAndStart = true;
                else if (drnlrVariables.MaxCoeff.HasValue && drnlrVariables.MaxStartValue.HasValue) warningFoxFixedAndStart = true;
                else if (drnlrVariables.SlopeCoeff.HasValue && drnlrVariables.SlopeStartValue.HasValue) warningFoxFixedAndStart = true;
                else if (drnlrVariables.EDICCoeff.HasValue && drnlrVariables.EDICStartValue.HasValue) warningFoxFixedAndStart = true;

                if (warningFoxFixedAndStart)
                {
                    ValidationInfo.AddWarningMessage("Warning: You have defined a start value for a parameter that has been fixed. The start value is ignored in the analysis.");
                }

                if (drnlrVariables.MinCoeff.HasValue && drnlrVariables.MaxCoeff.HasValue)
                {
                    if (drnlrVariables.MinCoeff > drnlrVariables.MaxCoeff)
                    {
                        ValidationInfo.AddErrorMessage("Error: The max coefficient is greater than the min coefficient");
                        return ValidationInfo;
                    }
                }

                if (drnlrVariables.MinStartValue.HasValue && drnlrVariables.MaxStartValue.HasValue)
                {
                    if (drnlrVariables.MinStartValue > drnlrVariables.MaxStartValue)
                    {
                        ValidationInfo.AddErrorMessage("Error: The max start value is greater than the min start value");
                        return ValidationInfo;
                    }
                }
            }
            else if (drnlrVariables.AnalysisType == DoseResponseAndNonLinearRegesssionAnalysisModel.AnalysisOption.Equation)
            {
                if (!drnlrVariables.Equation.Contains("x"))
                {
                    ValidationInfo.AddErrorMessage("The formula should be of the form f=f(x) with x lower case");
                    return ValidationInfo;
                }
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}