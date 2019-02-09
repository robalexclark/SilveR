using SilveR.StatsModels;
using System;

namespace SilveR.Validators
{
    public class ComparisonOfMeansPowerAnalysisUserBasedInputsValidator : ValidatorBase
    {
        private readonly ComparisonOfMeansPowerAnalysisUserBasedInputsModel mcVariables;

        public ComparisonOfMeansPowerAnalysisUserBasedInputsValidator(ComparisonOfMeansPowerAnalysisUserBasedInputsModel mc)
        {
            mcVariables = mc;
        }

        public override ValidationInfo Validate()
        {
            if (mcVariables.DeviationType == DeviationType.StandardDeviation && !mcVariables.StandardDeviation.HasValue)
            {
                ValidationInfo.AddErrorMessage("Standard Deviation is required");
                return ValidationInfo;
            }
            else if (mcVariables.DeviationType == DeviationType.Variance && !mcVariables.Variance.HasValue)
            {
                ValidationInfo.AddErrorMessage("Variance is required");
                return ValidationInfo;
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

                    foreach (string s in changes)//go through list and check that is a number and is greater than 0
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