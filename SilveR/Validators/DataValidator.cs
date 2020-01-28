using SilveR.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace SilveR.Validators
{
    public abstract class ValidatorBase
    {
        private readonly DataTable dataTable;
        protected DataTable DataTable
        {
            get { return dataTable; }
        }

        private readonly ValidationInfo validationInfo = new ValidationInfo();
        public ValidationInfo ValidationInfo
        {
            get { return validationInfo; }
        }

        protected ValidatorBase(DataTable dataTable = null) //naughty
        {
            if (dataTable != null)
            {
                this.dataTable = dataTable;
            }
        }

        public abstract ValidationInfo Validate();


        protected IEnumerable<string> GetLevels(string column)
        {
            return dataTable.GetLevels(column);
        }

        protected int CountDistinctLevels(string column)
        {
            return dataTable.GetLevels(column).Count();
        }

        protected int CountResponses(string column)
        {
            return dataTable.GetValues(column).Count();
        }


        protected Dictionary<string, int> ResponsesPerLevel(string treatCol, string responseCol)
        {
            //determine the number of responses per level and return as dictionary
            Dictionary<string, int> responseCounts = new Dictionary<string, int>();

            IEnumerable<string> levels = GetLevels(treatCol);

            if (String.IsNullOrEmpty(treatCol) || dataTable == null)
                return responseCounts; //i.e. empty list

            foreach (string level in levels)
            {
                int count = 0;
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row[treatCol].ToString() == level && !String.IsNullOrEmpty(row[responseCol].ToString()))
                    {
                        count = count + 1;
                    }
                }
                responseCounts.Add(level, count);
            }

            return responseCounts;
        }



        protected bool CheckIsNumeric(string column)
        {
            if (!String.IsNullOrEmpty(column))
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    bool isNumeric = Double.TryParse(row[column].ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var vvoid);

                    if (!isNumeric && !String.IsNullOrEmpty(row[column].ToString()))
                        return false;
                }
            }

            //if get here then no error so return true
            return true;
        }

        protected bool CheckColumnNames(IEnumerable<string> vars)
        {
            foreach (string s in vars)
            {
                if (String.IsNullOrEmpty(s)) //if the string is null then ignore it and carry on with next one
                    continue; 

                if (Double.TryParse(s[0].ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out var vvoid))
                {
                    validationInfo.AddErrorMessage("One or more of your selected columns has a numeric name or starts with a number. The analysis will not proceed.");
                    return false; //error so return
                }
            }

            //if get here then no error so return true
            return true;
        }


        protected bool CheckFactorsHaveLevels(IEnumerable<String> factors, bool multipleFactors)
        {
            if (factors == null)
                return true;

            //Check to see if the number of treatment levels selected is at least 2
            foreach (string factor in factors)
            {
                if (!CheckFactorsHaveLevels(factor, multipleFactors)) return false;
            }

            return true;
        }

        protected bool CheckFactorsHaveLevels(string factors)
        {
            return CheckFactorsHaveLevels(factors, false);
        }

        protected bool CheckFactorsHaveLevels(string factors, bool multipleFactors)
        {
            if (factors == null)
                return true;

            string message;

            //Check to see if the number of treatment levels selected is at least 2
            if (CountDistinctLevels(factors) == 0)
            {
                if (multipleFactors)
                {
                    message = "There are no data in one or more of the factors selected. Please select another factor.";
                }
                else
                {
                    message = "There are no data in the factor selected. Please select another factor.";
                }

                validationInfo.AddErrorMessage(message);
                return false;
            }
            else if (CountDistinctLevels(factors) == 1)
            {
                if (multipleFactors)
                {
                    message = "One or more of the factors (" + factors + ") has only one level present in the dataset. Please select another factor.";
                }
                else
                {
                    message = "The Treatment factor (" + factors + ") has only one level present in the dataset. Please select another factor.";
                }

                validationInfo.AddErrorMessage(message);
                return false;
            }

            return true;
        }

        protected bool CheckResponsesPerLevel(IEnumerable<string> factors, IEnumerable<string> responses, string displayName)
        {
            foreach (string response in responses)
            {
                if (!CheckResponsesPerLevel(factors, response, displayName)) //then error
                    return false;
            }

            return true;
        }

        protected bool CheckResponsesPerLevel(string factor, string response, string displayName)
        {
            List<string> factors = new List<string>();
            factors.Add(factor);

            return CheckResponsesPerLevel(factors, response, displayName);
        }


        protected virtual bool CheckResponsesPerLevel(IEnumerable<string> factors, string response, string displayName)
        {
            if (factors == null)
                return true;

            //Check that the number of responses for each level is at least 2 for factors
            foreach (string factor in factors)
            {
                Dictionary<string, int> levelResponses = ResponsesPerLevel(factor, response);
                foreach (KeyValuePair<string, int> level in levelResponses)
                {
                    if (level.Value == 0)
                    {
                        validationInfo.AddErrorMessage("There are no observations recorded on the levels of the " + displayName + " (" + factor + "). Please amend the dataset prior to running the analysis.");
                        return false;
                    }
                    else if (level.Value < 2)
                    {
                        validationInfo.AddErrorMessage("There is no replication in one or more of the levels of the " + displayName + " (" + factor + "). Please select another factor.");
                        return false;
                    }
                }
            }

            return true;
        }

        protected bool CheckFactorAndResponseNotBlank(string factor, string response, string displayName)
        {
            if (String.IsNullOrEmpty(response) || String.IsNullOrEmpty(factor))
                return true;

            bool hasWarning = false;
            bool hasError = false;

            foreach (DataRow row in DataTable.Rows)
            {
                //Check that there are treatment levels for where there are response data
                if (!String.IsNullOrEmpty(row[response].ToString()) && String.IsNullOrEmpty(row[factor].ToString()))
                {
                    hasError = true;
                    break;
                }

                //check that the response contains data for each treatment (not fatal)
                if (String.IsNullOrEmpty(row[response].ToString()) && !String.IsNullOrEmpty(row[factor].ToString()))
                {
                    hasWarning = true;
                }
            }

            if (hasError)
            {
                validationInfo.AddErrorMessage("The " + displayName + " (" + factor + ") contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.");
                return false;
            }
            else if (hasWarning)
            {
                string message = "The Response (" + response + ") contains missing data. Any rows of the dataset that contain missing responses will be excluded prior to the analysis.";
                validationInfo.AddWarningMessage(message);
            }

            return true;
        }

        protected bool CheckFactorsAndResponsesNotBlank(IEnumerable<string> factors, IEnumerable<string> responses, string displayName)
        {
            foreach (string response in responses)
            {
                if (!CheckFactorsAndResponseNotBlank(factors, response, displayName))
                    return false;
            }

            return true;
        }

        protected bool CheckFactorsAndResponseNotBlank(IEnumerable<string> factors, string response, string displayName)
        {
            foreach (string factor in factors)
            {
                if (!CheckFactorAndResponseNotBlank(factor, response, displayName))
                    return false;
            }

            return true;
        }


        protected void CheckTransformations(string transformation, IEnumerable<string> columns, bool isCovariate = false)
        {
            foreach (string column in columns)
            {
                CheckTransformations(transformation, column, isCovariate);
            }
        }

        protected void CheckTransformations(string transformation, string column, bool isCovariate = false)
        {
            if (String.IsNullOrEmpty(column))
                return;

            foreach (DataRow row in dataTable.Rows)
            {
                if (transformation != "None")
                {
                    bool respParsedOK = Double.TryParse(row[column].ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double respValue);

                    if (respParsedOK)
                    {
                        string tempMessage = null;

                        if (transformation.StartsWith("Log") && respValue <= 0) //must not be <= 0
                        {
                            tempMessage = "You have " + transformation +
                                   " transformed the " + column + " variable. Unfortunately some of the " + column + " values are zero and/or negative. These values have been ignored in the analysis as it is not possible to transform them.";
                        }
                        else if (transformation.StartsWith("Square") && respValue < 0) //must not be < 0
                        {
                            tempMessage = "You have " + transformation +
                                   " transformed the " + column + " variable. Unfortunately some of the " + column + " values are negative. These values have been ignored in the analysis as it is not possible to transform them.";
                        }
                        else if (transformation == "ArcSine" && (respValue < 0 || respValue > 1)) //must be between 0 and 1
                        {
                            tempMessage = "You have " + transformation + " transformed the " + column + " variable. Unfortunately some of the " + column + " values are <0 or >1. These values have been ignored in the analysis as it is not possible to transform them.";
                        }

                        if (tempMessage != null && isCovariate)
                        {
                            tempMessage = tempMessage + " Any response where the covariate has been removed will also be excluded from the analysis.";
                        }

                        validationInfo.AddWarningMessage(tempMessage);
                    }
                }
            }
        }
    }


    public static class ListExtension
    {
        public static void AddVariables(this List<string> list, string variable)
        {
            if (variable != null)
            {
                list.Add(variable);
            }
        }
        public static void AddVariables(this List<string> list, IEnumerable<string> variable)
        {
            if (variable != null)
            {
                list.AddRange(variable);
            }
        }
    }
}