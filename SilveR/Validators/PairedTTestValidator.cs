using SilveRModel.StatsModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace SilveRModel.Validators
{
    public class PairedTTestValidator : ValidatorBase
    {
        private readonly PairedTTestModel pttVariables;

        public PairedTTestValidator(PairedTTestModel ptt)
            : base(ptt.DataTable)
        {
            pttVariables = ptt;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            if (pttVariables.OtherDesignFactors != null)
                allVars.AddRange(pttVariables.OtherDesignFactors);

            allVars.Add(pttVariables.Treatment);
            allVars.Add(pttVariables.Subject);
            allVars.Add(pttVariables.Response);
            allVars.Add(pttVariables.Covariate);
            if (!CheckColumnNames(allVars)) return ValidationInfo;

            if (!CheckTreatmentsHaveLevels(pttVariables.Treatment, true)) return ValidationInfo;
            if (!CheckTreatmentsHaveLevels(pttVariables.Subject, true)) return ValidationInfo;

            //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
            if (!CheckResponsesPerLevel(pttVariables.Treatment, pttVariables.Response)) return ValidationInfo;
            if (pttVariables.OtherDesignFactors != null && !CheckResponsesPerLevel(pttVariables.OtherDesignFactors, pttVariables.Response, "other treatment")) return ValidationInfo;
            if (!CheckResponsesPerLevel(pttVariables.Subject, pttVariables.Response, "subject ")) return ValidationInfo;

            //First create a list of categorical variables selected (i.e. as treatments and other factors)
            List<string> categorical = new List<string>();
            categorical.Add(pttVariables.Treatment);
            if (pttVariables.OtherDesignFactors != null)
                categorical.AddRange(pttVariables.OtherDesignFactors);
            categorical.Add(pttVariables.Subject);

            //do data checks on the treatments/other factors and response
            if (!FactorAndResponseCovariateChecks(categorical, pttVariables.Response)) return ValidationInfo;

            //do data checks on the treatments/other factors and covariate (if selected)
            if (!String.IsNullOrEmpty(pttVariables.Covariate))
            {
                if (!FactorAndResponseCovariateChecks(categorical, pttVariables.Covariate)) return ValidationInfo;
            }

            //Check that each subject is only present in one blocking factor
            if (pttVariables.OtherDesignFactors != null && !CheckSubjectIsPresentInOnlyOneFactor(pttVariables.OtherDesignFactors, true))
                return ValidationInfo;
            //check each subject has one response at each time point
            if (!CheckSubjectOnlyHasOneResponseForEachTreatment())
                return ValidationInfo;

            //check that the replication of treatment factors in each timepoint is greater than 1
            if (!CheckReplicationOfTreatmentFactors())
                return ValidationInfo;

            //if get here then no errors so return true
            return ValidationInfo;
        }

        private bool CheckReplicationOfTreatmentFactors()
        {
            List<string> treatAndOtherFactors = new List<string>();
            treatAndOtherFactors.Add(pttVariables.Treatment);

            if (pttVariables.OtherDesignFactors != null)
                treatAndOtherFactors.AddRange(pttVariables.OtherDesignFactors);

            List<string> timePoints = GetLevels(pttVariables.Treatment);

            foreach (string factor in treatAndOtherFactors)
            {
                List<string> factorLevels = GetLevels(factor);

                foreach (string point in timePoints)
                {
                    foreach (string factorLevel in factorLevels)
                    {
                        //do a linq query on the data, getting the distinct levels in the data
                        IEnumerable<string> query = from row in DataTable.Select()
                                                    where row[pttVariables.Treatment].ToString() == point
                                                    && row[factor].ToString() == factorLevel
                                                    && !String.IsNullOrEmpty(row[factor].ToString())
                                                    && !String.IsNullOrEmpty(row[pttVariables.Response].ToString())
                                                    select row[pttVariables.Response].ToString();

                        if (query.Count() == 1)
                        {
                            ValidationInfo.AddErrorMessage("There is no replication in one or more of the levels of one or more of the factors. Please review your factor selection.");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        private bool CheckSubjectOnlyHasOneResponseForEachTreatment()
        {
            //check each subject only has one response for each treatment

            List<string> subjectsList = GetLevels(pttVariables.Subject);
            List<string> treatLevels = GetLevels(pttVariables.Treatment);

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
                        ValidationInfo.AddErrorMessage("At least one of the subjects has more than one observation recorded on one of the treatments. Please make sure the data was entered correctly as each subject can only be measued once at each level of the treatment factor.");
                        return false;
                    }
                }
            }

            //if get this far then return true
            return true;
        }

        private bool CheckSubjectIsPresentInOnlyOneFactor(List<string> factors, bool isBlockingFactor)
        {
            List<string> subjectsList = GetLevels(pttVariables.Subject);
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
                            ValidationInfo.AddErrorMessage("According to the dataset at least one subject is associated with more than one level of the treatment factor(s) or treatment factor interactions. Please review this, in the repeated measures module each subject should be associated with only one level of each treatment factor.");
                        }
                        return false;
                    }
                }
            }

            //if get this far then return true
            return true;
        }

        private bool FactorAndResponseCovariateChecks(List<string> categorical, string continuous)
        {
            foreach (string catFactor in categorical) //go through each categorical factor and do the check on each
            {
                string factorType;
                if (pttVariables.Treatment.Contains(catFactor))
                {
                    factorType = "treatment";
                }
                else if (pttVariables.Subject == catFactor)
                {
                    factorType = "subject factor";
                }
                else
                {
                    factorType = "other factor";
                }

                string responseType;
                if (pttVariables.Response.Contains(continuous))
                {
                    responseType = "response";
                }
                else
                {
                    responseType = "covariate";
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
                    double parsedValue;
                    bool parsedOK = Double.TryParse(continuousRow[i], out parsedValue);
                    if (!String.IsNullOrEmpty(continuousRow[i]) && !parsedOK)
                    {
                        ValidationInfo.AddErrorMessage("Error: The " + responseType + " selected contain non-numerical data which cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                        return false;
                    }

                    //Check that there are no responses where the treatments are blank
                    if (String.IsNullOrEmpty(categoricalRow[i]) && !String.IsNullOrEmpty(continuousRow[i]))
                    {
                        ValidationInfo.AddErrorMessage("Error: The " + factorType + " (" + catFactor + ") selected contains missing data where there are observations present in the " + responseType + " variable. Please check the raw data and make sure the data was entered correctly.");
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
                foreach (DataRow row in DataTable.Rows)
                {
                    CheckTransformations(row, pttVariables.ResponseTransformation, pttVariables.Response, "response");

                    CheckTransformations(row, pttVariables.CovariateTransformation, pttVariables.Covariate, "covariate");
                }
            }

            //if get this far then no errors so return true...
            return true;
        }
    }
}