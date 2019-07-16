using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SilveR.Validators
{
    public class PairedTTestAnalysisValidator : ValidatorBase
    {
        private readonly PairedTTestAnalysisModel pttVariables;

        public PairedTTestAnalysisValidator(PairedTTestAnalysisModel ptt)
            : base(ptt.DataTable)
        {
            pttVariables = ptt;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.AddVariables(pttVariables.OtherDesignFactors);
            allVars.AddVariables(pttVariables.Treatment);
            allVars.AddVariables(pttVariables.Subject);
            allVars.AddVariables(pttVariables.Response);
            allVars.AddVariables(pttVariables.Covariates);

            if (!CheckColumnNames(allVars)) return ValidationInfo;

            //First create a list of categorical variables selected (i.e. as treatments and other factors)
            List<string> categoricalVariables = new List<string>();
            categoricalVariables.AddVariables(pttVariables.Treatment);
            categoricalVariables.AddVariables(pttVariables.OtherDesignFactors);
            categoricalVariables.AddVariables(pttVariables.Subject);

            if (!CheckFactorsHaveLevels(categoricalVariables, true))
                return ValidationInfo;

            //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
            if (!CheckResponsesPerLevel(pttVariables.Treatment, pttVariables.Response, ReflectionExtensions.GetPropertyDisplayName<PairedTTestAnalysisModel>(i => i.Treatment)))
                return ValidationInfo;
            if (!CheckResponsesPerLevel(pttVariables.OtherDesignFactors, pttVariables.Response, ReflectionExtensions.GetPropertyDisplayName<PairedTTestAnalysisModel>(i => i.OtherDesignFactors)))
                return ValidationInfo;
            if (!CheckResponsesPerLevel(pttVariables.Subject, pttVariables.Response, ReflectionExtensions.GetPropertyDisplayName<PairedTTestAnalysisModel>(i => i.Subject)))
                return ValidationInfo;

            //do data checks on the treatments/other factors and response
            if (!CategoricalAgainstContinuousVariableChecks(categoricalVariables, pttVariables.Response))
                return ValidationInfo;

            //do data checks on the treatments/other factors and covariate (if selected)
            if (pttVariables.Covariates != null)
            {
                foreach (string covariate in pttVariables.Covariates)
                {
                    if (!CategoricalAgainstContinuousVariableChecks(categoricalVariables, covariate))
                        return ValidationInfo;
                }
            }

            //Check that each subject is only present in one blocking factor
            if (!CheckSubjectIsPresentInOnlyOneFactor(pttVariables.OtherDesignFactors, true))
                return ValidationInfo;

            //check each subject has one response at each time point
            if (!CheckSubjectOnlyHasOneResponseForEachTreatment())
                return ValidationInfo;

            //if get here then no errors so return true
            return ValidationInfo;
        }     

        private bool CheckSubjectOnlyHasOneResponseForEachTreatment()
        {
            //check each subject only has one response for each treatment
            IEnumerable<string> subjectsList = GetLevels(pttVariables.Subject);
            IEnumerable<string> treatLevels = GetLevels(pttVariables.Treatment);

            foreach (string treat in treatLevels)
            {
                foreach (string subject in subjectsList)
                {
                    //do a linq query on the data, getting the distinct levels in the data
                    IEnumerable<string> query = (from row in DataTable.Select()
                                                 where row[pttVariables.Subject].ToString() == subject
                                                 && row[pttVariables.Treatment].ToString() == treat
                                                 && !String.IsNullOrEmpty(row[pttVariables.Response].ToString())
                                                 select row[pttVariables.Response].ToString()).Distinct();

                    if (query.Count() > 1)
                    {
                        ValidationInfo.AddErrorMessage("At least one of the subjects has more than one observation recorded on one of the treatments. Please make sure the data was entered correctly as each subject can only be measued once at each level of the Treatment.");
                        return false;
                    }
                }
            }

            //if get this far then return true
            return true;
        }

        private bool CheckSubjectIsPresentInOnlyOneFactor(IEnumerable<string> factors, bool isBlockingFactor)
        {
            if (factors == null) return true;

            IEnumerable<string> subjectsList = GetLevels(pttVariables.Subject);
            foreach (string factor in factors)
            {
                foreach (string subject in subjectsList)
                {
                    //do a linq query on the data, getting the distinct levels in the data
                    IEnumerable<string> query = (from row in DataTable.Select()
                                                 where row[pttVariables.Subject].ToString() == subject
                                                 && !String.IsNullOrEmpty(row[factor].ToString())
                                                 && !String.IsNullOrEmpty(row[pttVariables.Response].ToString())
                                                 select row[factor].ToString()).Distinct();

                    if (query.Count() > 1)
                    {
                        if (isBlockingFactor)
                        {
                            ValidationInfo.AddErrorMessage("According to the dataset at least one subject is associated with more than one level of one of the blocking factors. Please review this, as each subject must be associated with only one level of each blocking factor.");
                        }
                        else
                        {
                            ValidationInfo.AddErrorMessage("According to the dataset at least one subject is associated with more than one level of the Treatment(s) or Treatment interactions. Please review this, in the repeated measures module each subject should be associated with only one level of each Treatment.");
                        }
                        return false;
                    }
                }
            }

            //if get this far then return true
            return true;
        }

        private bool CategoricalAgainstContinuousVariableChecks(List<string> categorical, string continuous)
        {
            foreach (string catFactor in categorical) //go through each categorical factor and do the check on each
            {
                string factorType;
                if (pttVariables.Treatment == catFactor)
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<PairedTTestAnalysisModel>(i => i.Treatment);
                }
                else if (pttVariables.Subject == catFactor)
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<PairedTTestAnalysisModel>(i => i.Subject);
                }
                else
                {
                    factorType = ReflectionExtensions.GetPropertyDisplayName<PairedTTestAnalysisModel>(i => i.OtherDesignFactors);
                }

                string responseType;
                if (pttVariables.Response.Contains(continuous))
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<PairedTTestAnalysisModel>(i => i.Response);
                }
                else
                {
                    responseType = ReflectionExtensions.GetPropertyDisplayName<PairedTTestAnalysisModel>(i => i.Covariates);
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
                    //Check that the "response" does not contains non-numeric data
                    bool parsedOK = Double.TryParse(continuousRow[i], out double parsedValue);
                    if (!String.IsNullOrEmpty(continuousRow[i]) && !parsedOK)
                    {
                        ValidationInfo.AddErrorMessage("The " + responseType + " (" + continuous + ") contains non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                        return false;
                    }

                    //Check that there are no responses where the treatments are blank
                    if (String.IsNullOrEmpty(categoricalRow[i]) && !String.IsNullOrEmpty(continuousRow[i]))
                    {
                        ValidationInfo.AddErrorMessage("The " + factorType + " (" + catFactor + ") contains missing data where there are observations present in the " + responseType + ". Please check the raw data and make sure the data was entered correctly.");
                        return false;
                    }

                    //check that the "response" contains data for each "treatment" (not fatal)
                    if (!String.IsNullOrEmpty(categoricalRow[i]) && String.IsNullOrEmpty(continuousRow[i]))
                    {
                        string mess = "The " + responseType + " (" + continuous + ") contains missing data.";
                        if (responseType == "Covariate")
                        {
                            mess = mess + " Any response that does not have a corresponding covariate will be excluded from the analysis.";
                        }

                        ValidationInfo.AddWarningMessage(mess);
                    }
                }

                //check transformations
                CheckTransformations(DataTable, pttVariables.ResponseTransformation, pttVariables.Response);

                if (pttVariables.Covariates != null)
                {
                    foreach (string covariate in pttVariables.Covariates)
                    {
                        CheckTransformations(DataTable, pttVariables.CovariateTransformation, covariate, true);
                    }
                }
            }

            //if got here then all checks ok, return true
            return true;
        }
    }
}