using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace SilveR.Validators
{
    //SHARED
    public class HasAtLeastOneEntryAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IEnumerable<object> iEnumerable = (IEnumerable<object>)value;

            if (iEnumerable == null || !iEnumerable.Any())
            {
                return new ValidationResult(validationContext.DisplayName + " requires at least one entry");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }


    public class CheckUsedOnceOnlyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var properties = validationContext.ObjectInstance.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(CheckUsedOnceOnlyAttribute), false));

            List<string> varList = new List<string>();
            foreach (PropertyInfo p in properties.Where(x => x.Name != validationContext.MemberName))
            {
                object propertyValue = p.GetValue(validationContext.ObjectInstance);
                if (propertyValue is String)
                {
                    varList.Add(propertyValue.ToString());
                }
                else if (propertyValue is IEnumerable<string>)
                {
                    varList.AddRange((IEnumerable<string>)propertyValue);
                }
            }

            bool result = true;
            if (value is String)
            {
                string stringVarToCheck = (String)value;
                if (varList.Contains(value))
                {
                    result = false;
                }
            }
            else if (value is List<string>)
            {
                List<string> varsToCheck = (List<string>)value;

                if (varsToCheck.Intersect(varList).Any())
                {
                    result = false;
                }
            }
            else
            {
                throw new ArgumentException("Attempting to check unknown type!");
            }

            if (result)
            {
                return ValidationResult.Success;
            }
            else
            {
                return new ValidationResult(validationContext.MemberName + " have been selected in more than one input category, please change your input options.");
            }
        }
    }


    //GRAPHICAL ANALYSIS
    public class ValidateXAxisAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GraphicalAnalysisModel ga = (GraphicalAnalysisModel)validationContext.ObjectInstance;

            if (String.IsNullOrEmpty(ga.XAxis) && (ga.ScatterplotSelected || ga.BoxplotSelected || ga.SEMPlotSelected || ga.CaseProfilesPlotSelected))
                return new ValidationResult("X-axis variable required for all plots except histogram");
            else
                return ValidationResult.Success;
        }
    }

    public class ValidateCaseIDFactorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GraphicalAnalysisModel ga = (GraphicalAnalysisModel)validationContext.ObjectInstance;

            if (String.IsNullOrEmpty(ga.CaseIDFactor) && ga.CaseProfilesPlotSelected)
                return new ValidationResult("Case ID variable required to produce Case Profiles Plot");
            else
                return ValidationResult.Success;
        }
    }

    //NON-PARAMETRIC
    public class ValidateControlLevelSetAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            NonParametricAnalysisModel model = (NonParametricAnalysisModel)validationContext.ObjectInstance;

            if (model.AnalysisType == NonParametricAnalysisModel.AnalysisOption.CompareToControl && String.IsNullOrEmpty(model.Control))
            {
                return new ValidationResult("Control level is a required variable when comparing to control");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }

    //P-VALUE ADJUSTMENT
    public class CheckPValueAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PValueAdjustmentModel model = (PValueAdjustmentModel)validationContext.ObjectInstance;

            if (String.IsNullOrEmpty(model.PValues))
            {
                return new ValidationResult("P values have not been set");
            }
            else //check to ensure that values in list are all numbers and are all comma separated and >0
            {
                string[] pValues = model.PValues?.Replace(" ", "").Split(','); //split list by comma

                foreach (string p in pValues)//go through list and check that is a number and is greater than 0
                {
                    double number;
                    if (!double.TryParse(p, out number) && p != "<0.001" && p != "<0.0001") //n.b. remove < sign when checking
                    {
                        return new ValidationResult("P values contains non-numeric values detected or values are not comma separated");
                    }
                    else if (number < 0)
                    {
                        return new ValidationResult("All p-values must be greater than zero");
                    }
                    else if (number > 1)
                    {
                        return new ValidationResult("All p-values must be less than 1.");
                    }
                }

                return ValidationResult.Success;
            }
        }
    }

    //SUMMARY STATS
    public class ValidateConfidenceLimitsAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            SummaryStatisticsModel model = (SummaryStatisticsModel)validationContext.ObjectInstance;

            if (model.Significance < 0 || model.Significance > 100)
            {
                return new ValidationResult("You have selected a confidence limit that is less than 1. Note that this value should be entered as percentage and not a fraction.");
            }
            else
                return ValidationResult.Success;
        }
    }

    //MEANS COMPARISON
    public class ValidateGroupMeanAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.ValueType == MeansComparisonModel.ValueTypeOption.Dataset) return ValidationResult.Success;

            if (String.IsNullOrEmpty(model.GroupMean))
            {
                return new ValidationResult("Group Mean is a required variable");
            }
            else if (!double.TryParse(model.GroupMean, out double val))
            {
                return new ValidationResult("Group Mean is not numeric");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }


    //CORRELATION
    public class HasAtLeastTwoEntriesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IEnumerable<object> iEnumerable = (IEnumerable<object>)value;

            if (iEnumerable == null || iEnumerable.Count() < 2)
            {
                return new ValidationResult(validationContext.DisplayName + " requires at least two entries");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }


    public class ValidateStandardDeviationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.ValueType == MeansComparisonModel.ValueTypeOption.Dataset)
            {
                return ValidationResult.Success;
            }
            else if (!String.IsNullOrEmpty(model.StandardDeviation) && !double.TryParse(model.StandardDeviation, out double val))
            {
                return new ValidationResult("Standard Deviation entered is not numeric");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }


    public class ValidateVarianceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.ValueType == MeansComparisonModel.ValueTypeOption.Dataset)
            {
                return ValidationResult.Success;
            }
            else if (!String.IsNullOrEmpty(model.Variance) && !double.TryParse(model.Variance, out double val))
            {
                return new ValidationResult("Variance entered is not numeric");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }


    public class ValidateResponseOrTreatmentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (value == null && model.ValueType == MeansComparisonModel.ValueTypeOption.Dataset)
            {
                return new ValidationResult(validationContext.DisplayName + " is a required variable");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }

    public class ValidateControlGroupAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (String.IsNullOrEmpty(model.ControlGroup) && model.ValueType == MeansComparisonModel.ValueTypeOption.Dataset && model.ChangeType.ToString() == "Percent")
            {
                return new ValidationResult("You have selected % change as expected changes from control, but as you have not defined the control group it is not possible to calculate the % change.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }

    public class ValidatePercentChangesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.ChangeType == MeansComparisonModel.ChangeTypeOption.Absolute)
            {
                return ValidationResult.Success;
            }
            if ((String.IsNullOrEmpty(model.PercentChange) && model.ChangeType == MeansComparisonModel.ChangeTypeOption.Percent))
            {
                return new ValidationResult(validationContext.DisplayName + " is a required variable");
            }
            else
            {
                string percentChange = (string)value;
                char[] splitters = { ',' };
                string[] changes = percentChange.Replace(" ", "").Split(splitters, StringSplitOptions.RemoveEmptyEntries); //split list by comma

                foreach (string s in changes)//go through list and check that is a number and is greater than 0
                {
                    double number;
                    if (!double.TryParse(s, out number))
                    {
                        return new ValidationResult(validationContext.DisplayName + " changes has non-numeric values or the values are not comma separated");
                    }
                    else if (number < 0)
                    {
                        return new ValidationResult(validationContext.DisplayName + " changes has values less than zero");
                    }
                }

                return ValidationResult.Success;
            }
        }
    }

    public class ValidateAbsoluteChangesAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.ChangeType == MeansComparisonModel.ChangeTypeOption.Percent)
            {
                return ValidationResult.Success;
            }
            else if (String.IsNullOrEmpty(model.AbsoluteChange) && model.ChangeType == MeansComparisonModel.ChangeTypeOption.Absolute)
            {
                return new ValidationResult(validationContext.DisplayName + " is a required variable");
            }
            else
            {
                string absoluteChange = (string)value;
                char[] splitters = { ',' };
                string[] changes = absoluteChange.Replace(" ", "").Split(splitters, StringSplitOptions.RemoveEmptyEntries); //split list by comma

                foreach (string s in changes)//go through list and check that is a number and is greater than 0
                {
                    double number;
                    if (!double.TryParse(s, out number))
                    {
                        return new ValidationResult(validationContext.DisplayName + " changes has non-numeric values or the values are not comma separated");
                    }
                    else if (number < 0)
                    {
                        return new ValidationResult(validationContext.DisplayName + " changes has values less than zero");
                    }
                }

                return ValidationResult.Success;
            }
        }
    }


    public class ValidateCustomFromAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.PlottingRangeType == MeansComparisonModel.PlottingRangeTypeOption.SampleSize)
                return ValidationResult.Success;

            bool fromOK = double.TryParse(model.PowerFrom, out double from);

            if (!fromOK)
            {
                return new ValidationResult("Custom From must be a numeric value");
            }
            else if (from < 0)
            {
                return new ValidationResult("Custom From value must be greater than zero.");
            }
            else
            {
                bool toOK = Double.TryParse(model.PowerTo, out double to);

                if (!toOK || from > to)
                {
                    return new ValidationResult("Custom From value must be less than the To value");
                }

                return ValidationResult.Success;
            }
        }
    }


    public class ValidateCustomToAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.PlottingRangeType == MeansComparisonModel.PlottingRangeTypeOption.SampleSize)
                return ValidationResult.Success;

            bool toOK = double.TryParse(model.PowerTo, out double to);

            if (!toOK)
            {
                return new ValidationResult("Custom To must be a numeric value");
            }
            else if (to < 0)
            {
                return new ValidationResult("Custom To value must be greater than zero.");
            }
            else
            {
                bool fromOK = Double.TryParse(model.PowerFrom, out double from);

                if (!fromOK || from > to)
                {
                    return new ValidationResult("Custom To value must be greater than the From value");
                }

                return ValidationResult.Success;
            }
        }
    }

    public class ValidateSampleSizeFromAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.PlottingRangeType == MeansComparisonModel.PlottingRangeTypeOption.Power)
                return ValidationResult.Success;

            bool fromOK = double.TryParse(model.SampleSizeFrom, out double from);

            if (!fromOK)
            {
                return new ValidationResult("Sample Size From must be a numeric value");
            }
            else if (from < 0)
            {
                return new ValidationResult("Sample Size From value must be greater than zero.");
            }
            else
            {
                bool toOK = Double.TryParse(model.SampleSizeTo, out double to);

                if (!toOK || from > to)
                {
                    return new ValidationResult("Sample Size To value must be greater than the From value");
                }

                return ValidationResult.Success;
            }
        }
    }


    public class ValidateSampleSizeToAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.PlottingRangeType == MeansComparisonModel.PlottingRangeTypeOption.Power)
                return ValidationResult.Success;

            bool toOK = double.TryParse(model.SampleSizeTo, out double to);

            if (!toOK)
            {
                return new ValidationResult("Sample Size To must be a numeric value");
            }
            else if (to < 0)
            {
                return new ValidationResult("Sample Size To value must be greater than zero.");
            }
            else
            {
                bool fromOK = Double.TryParse(model.SampleSizeFrom, out double from);

                if (!fromOK || from > to)
                {
                    return new ValidationResult("Sample Size From value must be less than the To value");
                }

                return ValidationResult.Success;
            }
        }
    }


    public class UniqueVariableChecker
    {
        private List<string> varList = new List<string>();

        public void AddVar(string item)
        {
            if (!String.IsNullOrEmpty(item))
            {
                varList.Add(item);
            }
        }

        public void AddVars(IEnumerable<string> items)
        {
            if (items != null)
            {
                varList.AddRange(items);
            }
        }

        public bool DoCheck(object varToCheck)
        {
            if (varToCheck is String)
            {
                string stringVarToCheck = (String)varToCheck;
                return !varList.Contains(varToCheck);
            }
            else if (varToCheck is List<string>)
            {
                List<string> varsToCheck = (List<string>)varToCheck;

                return !varsToCheck.Intersect(varList).Any();
            }
            else
            {
                throw new ArgumentException("Attempting to check unknown type!");
            }
        }
    }
}