using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SilveR.Validators
{
    public class RepeatedMeasuresParametricAnalysisValidator : ValidatorBase
    {
        private readonly RepeatedMeasuresParametricAnalysisModel rmVariables;

        public RepeatedMeasuresParametricAnalysisValidator(RepeatedMeasuresParametricAnalysisModel rm)
            : base(rm.DataTable)
        {
            rmVariables = rm;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.AddVariables(rmVariables.Treatments);
            allVars.AddVariables(rmVariables.OtherDesignFactors);
            allVars.AddVariables(rmVariables.RepeatedFactor);
            allVars.AddVariables(rmVariables.Subject);
            allVars.AddVariables(rmVariables.Response);
            allVars.AddVariables(rmVariables.Covariates);

            if (!CheckColumnNames(allVars)) return ValidationInfo;

            if (!CheckFactorsHaveLevels(rmVariables.Treatments, true))
                return ValidationInfo;
            if (!CheckFactorsHaveLevels(rmVariables.RepeatedFactor, true))
                return ValidationInfo;
            if (!CheckFactorsHaveLevels(rmVariables.Subject, true))
                return ValidationInfo;

            //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
            if (!CheckResponsesPerLevel(rmVariables.Treatments, rmVariables.Response, ReflectionExtensions.GetPropertyDisplayName<RepeatedMeasuresParametricAnalysisModel>(i => i.Treatments)))
                return ValidationInfo;
            if (!CheckResponsesPerLevel(rmVariables.OtherDesignFactors, rmVariables.Response, ReflectionExtensions.GetPropertyDisplayName<RepeatedMeasuresParametricAnalysisModel>(i => i.OtherDesignFactors)))
                return ValidationInfo;
            if (!CheckResponsesPerLevel(rmVariables.RepeatedFactor, rmVariables.Response, ReflectionExtensions.GetPropertyDisplayName<RepeatedMeasuresParametricAnalysisModel>(i => i.RepeatedFactor)))
                return ValidationInfo;
            if (!CheckResponsesPerLevel(rmVariables.Subject, rmVariables.Response, ReflectionExtensions.GetPropertyDisplayName<RepeatedMeasuresParametricAnalysisModel>(i => i.Subject)))
                return ValidationInfo;

            //First create a list of categorical variables selected (i.e. as treatments and other factors)
            List<string> categorical = new List<string>();
            categorical.AddVariables(rmVariables.Treatments);
            categorical.AddVariables(rmVariables.OtherDesignFactors);
            categorical.Add(rmVariables.RepeatedFactor);
            categorical.Add(rmVariables.Subject);

            //do data checks on the treatments/other factors and response
            if (!FactorAndResponseCovariateChecks(categorical, rmVariables.Response))
                return ValidationInfo;

            //do data checks on the treatments/other factors and covariate (if selected)
            if (rmVariables.Covariates != null)
            {
                foreach (string covariate in rmVariables.Covariates)
                {
                    if (!FactorAndResponseCovariateChecks(categorical, covariate))
                        return ValidationInfo;
                }
            }

            //Check that each subject is only present in one treatment factor
            if (!CheckSubjectIsPresentInOnlyOneFactor(rmVariables.Treatments, false))
                return ValidationInfo;

            //Check that each subject is only present in one blocking factor
            if (rmVariables.OtherDesignFactors != null && !CheckSubjectIsPresentInOnlyOneFactor(rmVariables.OtherDesignFactors, true))
                return ValidationInfo;

            //check that the replication of treatment factors in each timepoint is greater than 1
            if (!CheckReplicationOfTreatmentFactors())
                return ValidationInfo;

            //Here we are checking that all treatment combinations are present at each time point in the design
            if (!CheckTreatmentCombinations())
                return ValidationInfo;

            //check that the effect selected is the highest order interaction possible from selected factors, else output warning
            CheckEffectSelectedIsHighestOrderInteraction();

            //if get here then no errors so return true
            return ValidationInfo;
        }


        private bool CheckReplicationOfTreatmentFactors()
        {
            List<string> treatAndOtherFactors = new List<string>();
            treatAndOtherFactors.AddVariables(rmVariables.Treatments);
            treatAndOtherFactors.AddVariables(rmVariables.OtherDesignFactors);

            IEnumerable<string> timePoints = GetLevels(rmVariables.RepeatedFactor);

            foreach (string factor in treatAndOtherFactors)
            {
                IEnumerable<string> factorLevels = GetLevels(factor);

                foreach (string point in timePoints)
                {
                    foreach (string factorLevel in factorLevels)
                    {
                        int count = 0;
                        foreach (DataRow row in DataTable.Rows)
                        {
                            if (row[rmVariables.RepeatedFactor].ToString() == point && row[factor].ToString() == factorLevel && !String.IsNullOrEmpty(row[factor].ToString()) && !String.IsNullOrEmpty(row[rmVariables.Response].ToString()))
                            {
                                count++;
                            }
                        }

                        if (count == 1)
                        {
                            ValidationInfo.AddErrorMessage("There is no replication in one or more of the levels of one or more of the factors. Please review your factor selection.");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CheckSubjectIsPresentInOnlyOneFactor(IEnumerable<string> factors, bool isBlockingFactor)
        {
            IEnumerable<string> subjectList = GetLevels(rmVariables.Subject);
            foreach (string factor in factors)
            {
                foreach (string subject in subjectList)
                {
                    HashSet<string> distinctLevels = new HashSet<string>();
                    foreach (DataRow row in DataTable.Rows)
                    {
                        if (row[rmVariables.Subject].ToString() == subject && !String.IsNullOrEmpty(row[factor].ToString()) && !String.IsNullOrEmpty(row[rmVariables.Response].ToString()))
                        {
                            distinctLevels.Add(row[factor].ToString());
                        }
                    }

                    if (distinctLevels.Count > 1)
                    {
                        if (isBlockingFactor)
                        {
                            ValidationInfo.AddErrorMessage("Error: According to the dataset at least one subject is associated with more than one level of one of the blocking factors. Please review this, as each subject must be associated with only one level of each blocking factor.");
                        }
                        else
                        {
                            ValidationInfo.AddErrorMessage("Error: According to the dataset at least one subject is associated with more than one level of the treatment factor(s) or treatment factor interactions. Please review this, in the repeated measures module each subject should be associated with only one level of each treatment factor.");
                        }
                        return false;
                    }
                }
            }

            //if get this far then return true
            return true;
        }

        private bool CheckTreatmentCombinations()
        {
            //get a list of all treatment factors plus time
            List<string> treatmentsAndTime = new List<string>(rmVariables.Treatments);
            if (!String.IsNullOrEmpty(rmVariables.RepeatedFactor)) treatmentsAndTime.Add(rmVariables.RepeatedFactor);

            //Use hashsets to store the unique values
            HashSet<string> treatmentsAndTimeInteractions = new HashSet<string>();
            HashSet<string> timeLevels = new HashSet<string>();
            foreach (DataRow row in DataTable.Rows) //go through each row...
            {
                string treatmentWithTime = null;

                string respVal = row[rmVariables.Response].ToString().Trim();

                foreach (string factor in treatmentsAndTime)
                {
                    string treatValue = row[factor].ToString().Trim();

                    if (!String.IsNullOrEmpty(treatValue) && !String.IsNullOrEmpty(respVal))
                    {
                        //combine the values from each column into one string (only if both are present)
                        treatmentWithTime = treatmentWithTime + treatValue;
                    }
                }

                //if the treatments by day value exists add it to the hashset
                if (!String.IsNullOrEmpty(treatmentWithTime))
                {
                    treatmentsAndTimeInteractions.Add(treatmentWithTime);
                }

                //if the time exists, add it to the hashset
                if (!String.IsNullOrEmpty(row[rmVariables.RepeatedFactor].ToString().Trim()))
                {
                    timeLevels.Add(row[rmVariables.RepeatedFactor].ToString().Trim());
                }
            }

            //now work out the product of the treatment factor level counts
            int productOfFactorLevelCount = 1;
            foreach (string treat in rmVariables.Treatments)
            {
                HashSet<string> treatmentFactors = new HashSet<string>();
                foreach (DataRow row in DataTable.Rows) //go through each row...
                {
                    string treatVal = row[treat].ToString().Trim();
                    string respVal = row[rmVariables.Response].ToString().Trim();
                    if (!String.IsNullOrEmpty(treatVal) && !String.IsNullOrEmpty(respVal))
                    {
                        treatmentFactors.Add(treatVal);
                    }
                }

                productOfFactorLevelCount = productOfFactorLevelCount * treatmentFactors.Count;
            }

            //if the following calc is false then there is a problem...
            if (treatmentsAndTimeInteractions.Count != (productOfFactorLevelCount * timeLevels.Count))
            {
                ValidationInfo.AddErrorMessage("Error: One of the levels of the treatment factor(s), or a combination of the levels of the treatment factors, is not present at at least one of the timepoints. Please review this selection as all treatment factors (and combinations thereof) must be present at each timepoint.");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void CheckEffectSelectedIsHighestOrderInteraction()
        {
            if (!String.IsNullOrEmpty(rmVariables.SelectedEffect))
            {
                string[] splittedEffect = rmVariables.SelectedEffect.Split('*');
                if (splittedEffect.GetLength(0) < rmVariables.Treatments.Count() + 1) //add on one to account for subject factor
                {
                    string mainEffectOrInteraction;

                    if (splittedEffect.GetLength(0) == 1) //then only one factor
                    {
                        mainEffectOrInteraction = "a main effect";
                    }
                    else // then an interaction
                    {
                        mainEffectOrInteraction = "an interaction";
                    }

                    ValidationInfo.AddWarningMessage("You have selected to plot/compare levels of " + mainEffectOrInteraction +
                        " in the presence of a higher order interaction(s). This should only be carried out if the higher order interaction(s) are not statistically significant. In the following we have removed these interaction(s) from the model prior to making the comparisons. The actual model fitted is " +

                        rmVariables.GetEffectModel().Replace("mainEffect", " selected effect"));
                }
            }
        }

        private bool FactorAndResponseCovariateChecks(List<string> categorical, string continuous)
        {
            foreach (string catFactor in categorical) //go through each categorical factor and do the check on each
            {
                string factorType;
                if (rmVariables.Treatments.Contains(catFactor))
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<RepeatedMeasuresParametricAnalysisModel>(i => i.Treatments);
                }
                else
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<RepeatedMeasuresParametricAnalysisModel>(i => i.OtherDesignFactors);
                }

                string responseType;
                if (rmVariables.Response.Contains(continuous))
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<RepeatedMeasuresParametricAnalysisModel>(i => i.Response);
                }
                else
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<RepeatedMeasuresParametricAnalysisModel>(i => i.Covariates);
                }

                //Now that the whole column checks have been done, ensure that the treatment and response for each row is ok
                List<string> categoricalRow = new List<string>();
                List<string> continuousRow = new List<string>();

                foreach (DataRow row in DataTable.Rows) //assemble a list of the categorical data and the continuous data...
                {
                    categoricalRow.Add(row[catFactor].ToString());
                    continuousRow.Add(row[continuous].ToString());
                }

                for (int i = 0; i < DataTable.Rows.Count; i++) //use for loop cos its easier to compare the indexes of the cat and cont rows
                {
                    //Check that the "response" does not contain non-numeric data
                    bool parsedOK = Double.TryParse(continuousRow[i], out var parsedValue);
                    if (!String.IsNullOrEmpty(continuousRow[i]) && !parsedOK)
                    {
                        ValidationInfo.AddErrorMessage("The " + responseType + " selected (" + continuous + ") contain non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                        return false;
                    }

                    //Check that there are no responses where the treatments are blank
                    if (String.IsNullOrEmpty(categoricalRow[i]) && !String.IsNullOrEmpty(continuousRow[i]))
                    {
                        ValidationInfo.AddErrorMessage("The " + factorType + " selected (" + catFactor + ") contains missing data where there are observations present in the " + responseType + " variable. Please check the raw data and make sure the data was entered correctly.");
                        return false;
                    }

                    //check that the "response" contains data for each "treatment" (not fatal)
                    if (!String.IsNullOrEmpty(categoricalRow[i]) && String.IsNullOrEmpty(continuousRow[i]))
                    {
                        string mess = "The " + responseType + " selected contains missing data.";
                        if (responseType == "covariate")
                        {
                            mess = mess + Environment.NewLine + " Any response that does not have a corresponding covariate will be excluded from the analysis.";
                        }

                        ValidationInfo.AddWarningMessage(mess);
                    }
                }

                //check transformations
                CheckTransformations(DataTable, rmVariables.ResponseTransformation, rmVariables.Response);

                if (rmVariables.Covariates != null)
                {
                    foreach (string covariate in rmVariables.Covariates)
                    {
                        CheckTransformations(DataTable, rmVariables.CovariateTransformation, covariate, true);
                    }
                }
            }

            //if get this far then no errors so return true...
            return true;
        }
    }
}