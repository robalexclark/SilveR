using SilveRModel.StatsModel;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace SilveRModel.Validators
{
    //SHARED
    public class HasAtLeastOneEntry : ValidationAttribute
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

    public class CheckUsedOnceOnly : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IAnalysisModel model = (IAnalysisModel)validationContext.ObjectInstance;

            bool result = model.VariablesUsedOnceOnly(validationContext.MemberName);

            if (!result)
            {
                return new ValidationResult(validationContext.MemberName + " is selected more than once. Please amend the dataset prior to running the analysis.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }


    //GRAPHICAL ANALYSIS
    public class ValidateXAxis : ValidationAttribute
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

    public class ValidateCaseIDFactor : ValidationAttribute
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
    public class ValidateControlLevelSet : ValidationAttribute
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
    public class CheckPValue : ValidationAttribute
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
                        return new ValidationResult("All p-values must be greater than zero");
                    }
                }

                return ValidationResult.Success;
            }
        }
    }

    //SUMMARY STATS
    public class ValidateConfidenceLimits : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            SummaryStatisticsModel model = (SummaryStatisticsModel)validationContext.ObjectInstance;

            if (model.Significance < 0 || model.Significance > 100)
            {
                return new ValidationResult("You have selected a confidence limit that is less than 0 or greater than 100. Values should be entered as percentages and not fractions.");
            }
            else
                return ValidationResult.Success;
        }
    }

    //MEANS COMPARISON
    public class ValidateGroupMean : ValidationAttribute
    {
        private bool IsNumeric(string value)
        {
            double val;
            return double.TryParse(value, out val);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.ValueType == MeansComparisonModel.ValueTypeOption.Dataset) return ValidationResult.Success;

            if (String.IsNullOrEmpty(model.GroupMean))
                return new ValidationResult("Group Mean is a required variable");
            else if (!IsNumeric(model.GroupMean))
                return new ValidationResult("Group Mean is not numeric");
            else
                return ValidationResult.Success;
        }
    }


    public class ValidateStandardDeviation : ValidationAttribute
    {
        private bool IsNumeric(string value)
        {
            double val;
            return double.TryParse(value, out val);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.ValueType == MeansComparisonModel.ValueTypeOption.Dataset)
            {
                return ValidationResult.Success;
            }
            else if (!String.IsNullOrEmpty(model.StandardDeviation) && !IsNumeric(model.StandardDeviation))
                return new ValidationResult("Standard Deviation entered is not numeric");
            else
                return ValidationResult.Success;
        }
    }


    public class ValidateVariance : ValidationAttribute
    {
        private bool IsNumeric(string value)
        {
            double val;
            return double.TryParse(value, out val);
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.ValueType == MeansComparisonModel.ValueTypeOption.Dataset)
            {
                return ValidationResult.Success;
            }
            else if (!String.IsNullOrEmpty(model.Variance) && !IsNumeric(model.Variance))
                return new ValidationResult("Variance entered is not numeric");
            else
                return ValidationResult.Success;
        }
    }


    public class ValidateResponseOrTreatment : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (value == null && model.ValueType == MeansComparisonModel.ValueTypeOption.Dataset)
                return new ValidationResult(validationContext.DisplayName + " is a required variable");
            else
                return ValidationResult.Success;
        }
    }

    public class ValidateControlGroup : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (String.IsNullOrEmpty(model.ControlGroup) && model.ValueType == MeansComparisonModel.ValueTypeOption.Dataset && model.ChangeType.ToString() == "Percent")
                return new ValidationResult("You have selected % change as expected changes from control, but as you have not defined the control group it is not possible to calculate the % change.");
            else
                return ValidationResult.Success;
        }
    }

    public class ValidatePercentChanges : ValidationAttribute
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

    public class ValidateAbsoluteChanges : ValidationAttribute
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


    public class ValidateCustomFrom : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.PlottingRangeType == MeansComparisonModel.PlottingRangeTypeOption.SampleSize)
                return ValidationResult.Success;

            double from;
            bool fromOK = double.TryParse(model.PowerFrom, out from);

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
                double to;
                bool toOK = Double.TryParse(model.PowerTo, out to);

                if (!toOK || from > to)
                {
                    return new ValidationResult("Custom From value must be less than the To value");
                }

                return ValidationResult.Success;
            }
        }
    }


    public class ValidateCustomTo : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.PlottingRangeType == MeansComparisonModel.PlottingRangeTypeOption.SampleSize)
                return ValidationResult.Success;

            double to;
            bool toOK = double.TryParse(model.PowerTo, out to);

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
                double from;
                bool fromOK = Double.TryParse(model.PowerFrom, out from);

                if (!fromOK || from > to)
                {
                    return new ValidationResult("Custom To value must be greater than the From value");
                }

                return ValidationResult.Success;
            }
        }
    }

    public class ValidateSampleSizeFrom : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.PlottingRangeType == MeansComparisonModel.PlottingRangeTypeOption.Power)
                return ValidationResult.Success;

            double from;
            bool fromOK = double.TryParse(model.SampleSizeFrom, out from);

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
                double to;
                bool toOK = Double.TryParse(model.SampleSizeTo, out to);

                if (!toOK || from > to)
                {
                    return new ValidationResult("Sample Size To value must be greater than the From value");
                }

                return ValidationResult.Success;
            }
        }
    }


    public class ValidateSampleSizeTo : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            MeansComparisonModel model = (MeansComparisonModel)validationContext.ObjectInstance;

            if (model.PlottingRangeType == MeansComparisonModel.PlottingRangeTypeOption.Power)
                return ValidationResult.Success;

            double to;
            bool toOK = double.TryParse(model.SampleSizeTo, out to);

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
                double from;
                bool fromOK = Double.TryParse(model.SampleSizeFrom, out from);

                if (!fromOK || from > to)
                {
                    return new ValidationResult("Sample Size From value must be less than the To value");
                }

                return ValidationResult.Success;
            }
        }
    }


    //SINGLE MEASURES
    public class ValidatePrimaryFactorSetWhenCovariateSelected : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            SingleMeasuresParametricAnalysisModel model = (SingleMeasuresParametricAnalysisModel)validationContext.ObjectInstance;

            if (!String.IsNullOrEmpty(model.Covariate) && String.IsNullOrEmpty(model.PrimaryFactor))
            {
                return new ValidationResult("You have selected a covariate but no primary factor is selected.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }

    public class ValidateControlLevelSetWhenComparingToControl : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            SingleMeasuresParametricAnalysisModel model = (SingleMeasuresParametricAnalysisModel)validationContext.ObjectInstance;

            if (!String.IsNullOrEmpty(model.ComparisonsBackToControl) && String.IsNullOrEmpty(model.ControlGroup))
            {
                return new ValidationResult("You have selected to compare back to a control but no control group is selected");
            }
            else
            {
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

        public void AddVars(List<string> items)
        {
            if (items != null)
            {
                varList.AddRange(items);
            }
        }

        public bool DoCheck(object varToCheck)
        {
            if (varToCheck == null)
            {
                return false;
            }
            else if (varToCheck is String)
            {
                string stringVarToCheck = (String)varToCheck;
                return !varList.Contains(varToCheck);
            }
            else if (varToCheck is List<string>)
            {
                List<string> varsToCheck = (List<string>)varToCheck;

                return !varsToCheck.Intersect(varList).Any();
            }
            else throw new ArgumentException();
        }
    }
}