using SilveRModel.Helpers;
using System;
using System.Collections.Generic;
using System.Data;

namespace SilveRModel.Validators
{
    public abstract class ValidatorBase
    {
        private readonly DataTable dataTable;
        protected DataTable DataTable
        {
            get { return dataTable; }
        }

        private ValidationInfo validationInfo = new ValidationInfo();
        public ValidationInfo ValidationInfo
        {
            get { return validationInfo; }
        }

        protected ValidatorBase(DataTable dataTable)
        {
            if (dataTable != null)
            {
                this.dataTable = dataTable;
            }
        }

        public abstract ValidationInfo Validate();


        //***
        //the following functions are used by derived classes to check data etc.
        //***

        protected List<string> GetLevels(string column)
        {
            return dataTable.GetLevels(column);
        }

        protected int CountDistinctLevels(string column)
        {
            List<string> levels = GetLevels(column);
            return levels.Count;
        }

        protected int NoOfResponses(string column)
        {
            return GetLevels(column).Count;
        }

        //protected Dictionary<string, int> ResponsesPerLevel(string treatCol, string responseCol)
        //{
        //    //determine the number of responses per level and return as dictionary
        //    Dictionary<string, int> responseCounts = new Dictionary<string, int>();

        //    if (String.IsNullOrEmpty(treatCol) || dataTable == null) return responseCounts; //i.e. empty list

        //    List<string> levels = GetLevels(treatCol);

        //    foreach (string level in levels)
        //    {
        //        int responseCount = 0;

        //        foreach (DataRow row in dataTable.Rows)
        //        {
        //            if (!String.IsNullOrEmpty(row[treatCol].ToString()) && !String.IsNullOrEmpty(row[responseCol].ToString()) && row[treatCol].ToString() == level)
        //            {
        //                responseCount++;
        //            }
        //        }

        //        responseCounts.Add(level, responseCount);
        //    }

        //    return responseCounts;
        //}

        protected Dictionary<string, int> ResponsesPerLevel(string treatCol, string responseCol)
        {
            //determine the number of responses per level and return as dictionary
            Dictionary<string, int> responseCounts = new Dictionary<string, int>();

            List<string> levels = GetLevels(treatCol);

            if (String.IsNullOrEmpty(treatCol) || dataTable == null) return responseCounts; //i.e. empty list

            foreach (string level in levels)
            {
                // IEnumerable<string> query = (from row in dataTable.AsEnumerable()
                //                            where row.Field<string>(treatCol) == level & !String.IsNullOrEmpty(Convert.ToString( row.Field<string>(responseCol)))
                //                          select row.Field<string>(treatCol));

                //var query = dataTable.AsEnumerable().Where(dt => dt.Field<string>(treatCol) == level && !String.IsNullOrEmpty(dt.Field<string>(responseCol)));

                //responseCounts.Add(level, query.Count());

                //REALLY NOT SURE WHY THE ABOVE SUDDENLY STARTED FAILING
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
                    double number;
                    bool isNumeric = Double.TryParse(row[column].ToString(), out number);

                    if (!isNumeric && !String.IsNullOrEmpty(row[column].ToString())) return false;
                }
            }
            return true;
        }

        protected bool CheckColumnNames(List<string> vars)
        {
            foreach (string s in vars)
            {
                if (String.IsNullOrEmpty(s)) continue; //if the string is null then ignore it and carry on with next one

                double vvoid;
                if (double.TryParse(s[0].ToString(), out vvoid))
                {
                    validationInfo.AddErrorMessage("One or more of your selected columns has a numeric name or starts with a number. The analysis will not proceed.");
                    return false; //error so return
                }
            }

            //if get here then no error so return true
            return true;
        }

        protected bool CheckTreatmentsHaveLevels(List<String> treatments)
        {
            return CheckTreatmentsHaveLevels(treatments, false);
        }

        protected bool CheckTreatmentsHaveLevels(List<String> treatments, bool multipleFactors)
        {
            //Check to see if the number of treatment levels selected is at least 2
            foreach (string treatment in treatments)
            {
                if (!CheckTreatmentsHaveLevels(treatment, multipleFactors)) return false;
            }

            return true;
        }

        protected bool CheckTreatmentsHaveLevels(string treatment)
        {
            return CheckTreatmentsHaveLevels(treatment, false);
        }

        protected bool CheckTreatmentsHaveLevels(string treatment, bool multipleFactors)
        {
            string message = null;

            //Check to see if the number of treatment levels selected is at least 2
            if (CountDistinctLevels(treatment) == 0)
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
            else if (CountDistinctLevels(treatment) == 1)
            {
                if (multipleFactors)
                {
                    message = "One or more of the factors has only one level present in the dataset. Please select another factor.";
                }
                else
                {
                    message = "The treatment factor has only one level present in the dataset. Please select another factor.";
                }

                validationInfo.AddErrorMessage(message);
                return false;
            }

            return true;
        }


        protected bool CheckResponsesPerLevel(string treatment, string response)
        {
            List<string> treatments = new List<string>();
            treatments.Add(treatment);

            return CheckResponsesPerLevel(treatments, response);
        }

        protected bool CheckResponsesPerLevel(string treatment, string response, string text)
        {
            List<string> treatments = new List<string>();
            treatments.Add(treatment);

            return CheckResponsesPerLevel(treatments, response, text);
        }

        protected bool CheckResponsesPerLevel(List<string> treatments, string response)
        {
            return CheckResponsesPerLevel(treatments, response, "treatment");
        }

        protected bool CheckResponsesPerLevel(List<string> treatments, string response, string text)
        {
            //Check that the number of responses for each level is at least 2 for treatments
            foreach (string treatment in treatments)
            {
                Dictionary<string, int> levelResponses = ResponsesPerLevel(treatment, response);
                foreach (KeyValuePair<string, int> level in levelResponses)
                {
                    if (level.Value < 2)
                    {
                        validationInfo.AddErrorMessage("There is no replication in one or more of the levels of the " + text + " factor. Please select another factor.");
                        return false;
                    }
                }
            }

            return true;
        }

        protected bool CheckResponseAndTreatmentsNotBlank(string response, string treatment, string text)
        {
            if (String.IsNullOrEmpty(response) || String.IsNullOrEmpty(treatment)) return true;

            foreach (DataRow row in DataTable.Rows)
            {
                //Check that there are treatment levels for where there are response data
                if (!String.IsNullOrEmpty(row[response].ToString()) && String.IsNullOrEmpty(row[treatment].ToString()))
                {
                    validationInfo.AddErrorMessage("The " + text + " selected contains missing data where there are observations present in the response variable. Please check the raw data and make sure the data was entered correctly.");
                    return false;
                }

                //check that the response contains data for each treatment (not fatal)
                if (String.IsNullOrEmpty(row[response].ToString()) && !String.IsNullOrEmpty(row[treatment].ToString()))
                {
                    string message = "The response selected (" + response + ") contains missing data.";

                    validationInfo.AddWarningMessage(message);
                }
            }

            return true;
        }

        protected void CheckTransformations(DataRow row, string transformation, string column, string text)
        {
            double respValue;
            if (!String.IsNullOrEmpty(column))
            {
                bool respParsedOK = Double.TryParse(row[column].ToString(), out respValue);

                string tempMessage = null;

                if (respParsedOK && transformation != "None")
                {
                    if (transformation.StartsWith("Log") && respValue <= 0) //must not be <= 0
                    {
                        tempMessage = "You have " + transformation +
                               " transformed the " + column + " variable. Unfortunately some of the " + column + " values are either zero or negative or both. These values have been ignored in the analysis as it is not possible to transform them.";
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

                    if (tempMessage != null && text == "covariate")
                    {
                        tempMessage = tempMessage + Environment.NewLine + "Any response where the covariate has been removed will also be excluded from the analysis.";
                    }

                    validationInfo.AddWarningMessage(tempMessage);
                }
            }
        }
    }
}