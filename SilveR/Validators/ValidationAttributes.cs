﻿using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace SilveR.Validators
{
    //SHARED
    public class CheckUsedOnceOnlyAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            IEnumerable<PropertyInfo> properties = validationContext.ObjectInstance.GetType().GetProperties().Where(prop => prop.IsDefined(typeof(CheckUsedOnceOnlyAttribute), false));

            List<string> varList = new List<string>();
            foreach (PropertyInfo p in properties.Where(x => x.Name != validationContext.MemberName))
            {
                object propertyValue = p.GetValue(validationContext.ObjectInstance);

                switch (propertyValue)
                {
                    case string strPropertyValue:
                        varList.Add(strPropertyValue);

                        break;
                    case IEnumerable<string> strIEnumerable:
                        varList.AddRange(strIEnumerable);

                        break;
                }
            }

            bool result = true;
            string duplicateVarName = null;
            switch (value)
            {
                case string stringVarToCheck:
                    if (varList.Contains(stringVarToCheck))
                    {
                        duplicateVarName = stringVarToCheck;
                        result = false;
                    }
                    break;
                case IEnumerable<string> varsToCheck:
                    if (varsToCheck.Intersect(varList).Any())
                    {
                        duplicateVarName = varsToCheck.Intersect(varList).First();
                        result = false;
                    }
                    break;

                default:
                    throw new InvalidOperationException("Attempting to check unknown type!");
            }

            if (result)
            {
                return ValidationResult.Success;
            }
            else
            {
                string displayName = validationContext.DisplayName.TrimEnd('s');
                return new ValidationResult(displayName + " (" + duplicateVarName + ") has been selected in more than one input category, please change your input options.");
            }
        }
    }


    //GRAPHICAL ANALYSIS
    public class ValidateXAxisAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GraphicalAnalysisModel ga = (GraphicalAnalysisModel)validationContext.ObjectInstance;

            if (String.IsNullOrEmpty(ga.XAxis) && (ga.ScatterplotSelected || ga.BoxplotSelected || ga.ErrorBarPlotSelected || ga.CaseProfilesPlotSelected))
            {
                return new ValidationResult("X-axis variable required for all plots except histogram.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }

    public class ValidateCaseIDFactorAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GraphicalAnalysisModel ga = (GraphicalAnalysisModel)validationContext.ObjectInstance;

            if (String.IsNullOrEmpty(ga.CaseIDFactor) && ga.CaseProfilesPlotSelected)
            {
                return new ValidationResult("Case ID variable required to produce Case Profiles Plot.");
            }
            else
            {
                return ValidationResult.Success;
            }
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
                return new ValidationResult("Control level is a required variable when comparing to control.");
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
            PValueAdjustmentUserBasedInputsModel model = (PValueAdjustmentUserBasedInputsModel)validationContext.ObjectInstance;

            if (String.IsNullOrEmpty(model.PValues))
            {
                return new ValidationResult("P values have not been set");
            }
            else //check to ensure that values in list are all numbers and are all comma separated and >0
            {
                string[] pValues = model.PValues.Replace(" ", "").Split(','); //split list by comma

                foreach (string p in pValues)//go through list and check that is a number and is greater than 0
                {
                    if (!Double.TryParse(p, out double number) && p != "<0.001" && p != "<0.0001") //n.b. remove < sign when checking
                    {
                        return new ValidationResult("P values contains non-numeric values detected or values are not comma separated.");
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

            if (model.Significance < 1 || model.Significance > 100)
            {
                return new ValidationResult("You have selected a confidence limit that is less than 1. Note that this value should be entered as a percentage and not a fraction.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }


    public class HasAtLeastTwoEntriesAttribute : ValidationAttribute
    {
        private readonly string additionalErrorMessage;
        public HasAtLeastTwoEntriesAttribute(string additionalErrorMessage = null)
        {
            this.additionalErrorMessage = additionalErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            IEnumerable<object> iEnumerable = (IEnumerable<object>)value;

            if (iEnumerable == null || iEnumerable.Count() < 2)
            {
                return new ValidationResult((validationContext.DisplayName + " requires at least two entries. " + additionalErrorMessage).Trim());
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }


    //DOSE RESPONSE
    public class ValidateResponseOrDoseAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            DoseResponseAndNonLinearRegressionAnalysisModel model = (DoseResponseAndNonLinearRegressionAnalysisModel)validationContext.ObjectInstance;

            if (value == null && model.AnalysisType == DoseResponseAndNonLinearRegressionAnalysisModel.AnalysisOption.FourParameter)
            {
                return new ValidationResult(validationContext.DisplayName + " is a required variable.");
            }
            else
            {
                return ValidationResult.Success;
            }
        }
    }


    //EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel
    public class CheckTrueDifferenceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel model = (EquivalenceOfMeansPowerAnalysisDatasetBasedInputsModel)validationContext.ObjectInstance;

            if (String.IsNullOrEmpty(model.TrueDifference))
            {
                return new ValidationResult("True difference has not been set");
            }
            else //check to ensure that values in list are all numbers and are all comma separated and >0
            {
                string[] pValues = model.TrueDifference.Replace(" ", "").Split(','); //split list by comma

                foreach (string p in pValues)//go through list and check that is a number and is greater than 0
                {
                    if (!Double.TryParse(p, out double _))
                    {
                        return new ValidationResult("True difference contains non-numeric values detected or values are not comma separated.");
                    }
                }

                return ValidationResult.Success;
            }
        }
    }
}