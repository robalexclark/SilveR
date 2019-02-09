using SilveR.StatsModels;
using System;
using System.Collections.Generic;

namespace SilveR.Validators
{
    public class ComparisonOfMeansPowerAnalysisDatasetBasedInputsValidator : ValidatorBase
    {
        private readonly ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel mcVariables;

        public ComparisonOfMeansPowerAnalysisDatasetBasedInputsValidator(ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel mc)
            : base(mc.DataTable)
        {
            mcVariables = mc;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.Add(mcVariables.Treatment);
            allVars.Add(mcVariables.Response);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            if (!CheckFactorsHaveLevels(mcVariables.Treatment))
                return ValidationInfo;

            //Check that the response does not contain non-numeric 
            if (!CheckIsNumeric(mcVariables.Response))
            {
                ValidationInfo.AddErrorMessage("The response variable selected (" + mcVariables.Response + ") contains non-numerical data that cannot be processed. Please check raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            if (!String.IsNullOrEmpty(mcVariables.Treatment))
            {
                if (!CheckResponsesPerLevel(mcVariables.Treatment, mcVariables.Response, ReflectionExtensions.GetPropertyDisplayName<ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel>(i => i.Treatment)))
                    return ValidationInfo;
            }

            if (!String.IsNullOrEmpty(mcVariables.Treatment) && !String.IsNullOrEmpty(mcVariables.Response)) //if a treat and response is selected...
            {
                //Check that the number of responses for each level is at least 2
                Dictionary<string, int> levelResponses = ResponsesPerLevel(mcVariables.Treatment, mcVariables.Response);
                foreach (KeyValuePair<string, int> level in levelResponses)
                {
                    if (level.Value < 2)
                    {
                        ValidationInfo.AddErrorMessage("There is no replication in one or more of the levels of the treatment factor (" + mcVariables.Treatment + ").  Please amend the dataset prior to running the analysis.");
                        return ValidationInfo;
                    }
                }

                //check response and doses contain values
                if (!CheckFactorAndResponseNotBlank(mcVariables.Treatment, mcVariables.Response, "treatment factor")) return ValidationInfo;
            }
            else if (String.IsNullOrEmpty(mcVariables.Treatment) && !String.IsNullOrEmpty(mcVariables.Response))
            //if only a response selected (doing absolute change) then check that more than 1 value is in the dataset!
            {
                if (CountResponses(mcVariables.Response) == 1)
                {
                    ValidationInfo.AddErrorMessage("The response selected (" + mcVariables.Response + ") contains only 1 value. Please select another factor.");
                    return ValidationInfo;
                }
            }

            if (mcVariables.ChangeType == ChangeTypeOption.Percent)
            {
                if (String.IsNullOrEmpty(mcVariables.PercentChange))
                {
                    ValidationInfo.AddErrorMessage("Percent changes is required");
                    return ValidationInfo;
                }
                else
                {
                    char[] splitters = { ',' };
                    string[] changes = mcVariables.PercentChange.Replace(" ", "").Split(splitters, StringSplitOptions.None); //split list by comma

                    foreach (string s in changes)
                    {
                        if (String.IsNullOrWhiteSpace(s))
                        {
                            ValidationInfo.AddErrorMessage("The list of percent changes contains missing values, please remove any blank entries between the comma separated values");
                            return ValidationInfo;
                        }
                        else
                        {
                            double number;
                            if (!double.TryParse(s, out number))
                            {
                                ValidationInfo.AddErrorMessage("Percent changes has non-numeric values or the values are not comma separated");
                                return ValidationInfo;
                            }
                            else if (number < 0)
                            {
                                ValidationInfo.AddErrorMessage("Percent changes has values less than zero");
                                return ValidationInfo;
                            }
                        }
                    }
                }
            }
            else if (mcVariables.ChangeType == ChangeTypeOption.Absolute)
            {
                if (String.IsNullOrEmpty(mcVariables.AbsoluteChange))
                {
                    ValidationInfo.AddErrorMessage("Absolute changes is required");
                    return ValidationInfo;
                }
                else
                {
                    char[] splitters = { ',' };
                    string[] changes = mcVariables.AbsoluteChange.Replace(" ", "").Split(splitters, StringSplitOptions.None); //split list by comma

                    foreach (string s in changes)//go through list and check that is a number and is greater than 0
                    {
                        if (String.IsNullOrWhiteSpace(s))
                        {
                            ValidationInfo.AddErrorMessage("The list of absolute changes contains missing values, please remove any blank entries between the comma separated values");
                            return ValidationInfo;
                        }
                        else
                        {
                            double number;
                            if (!double.TryParse(s, out number))
                            {
                                ValidationInfo.AddErrorMessage("Absolute changes has non-numeric values or the values are not comma separated");
                                return ValidationInfo;
                            }
                            else if (number < 0)
                            {
                                ValidationInfo.AddErrorMessage("Absolute changes has values less than zero");
                                return ValidationInfo;
                            }
                        }
                    }
                }
            }

            if (mcVariables.PlottingRangeType == PlottingRangeTypeOption.SampleSize)
            {
                if (!mcVariables.SampleSizeFrom.HasValue || !mcVariables.SampleSizeTo.HasValue)
                {
                    ValidationInfo.AddErrorMessage("Sample Size From and To must be set");
                    return ValidationInfo;
                }
                else if (mcVariables.SampleSizeFrom > mcVariables.SampleSizeTo)
                {
                    ValidationInfo.AddErrorMessage("Sample Size To value must be greater than the From value");
                    return ValidationInfo;
                }
            }
            else if (mcVariables.PlottingRangeType == PlottingRangeTypeOption.Power)
            {
                if (!mcVariables.PowerFrom.HasValue || !mcVariables.PowerTo.HasValue)
                {
                    ValidationInfo.AddErrorMessage("Power From and To must be set");
                    return ValidationInfo;
                }
                else if (mcVariables.PowerFrom > mcVariables.PowerTo)
                {
                    ValidationInfo.AddErrorMessage("Power To value must be greater than the From value");
                    return ValidationInfo;
                }
            }


            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}