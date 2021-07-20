﻿using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace SilveR.Validators
{
    public class UnpairedTTestAnalysisValidator : ValidatorBase
    {
        private readonly UnpairedTTestAnalysisModel upttVariables;

        public UnpairedTTestAnalysisValidator(UnpairedTTestAnalysisModel uptt)
            : base(uptt.DataTable)
        {
            upttVariables = uptt;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.Add(upttVariables.Treatment);
            allVars.Add(upttVariables.Response);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            if (!CheckFactorsHaveLevels(upttVariables.Treatment))
                return ValidationInfo;

            if (!CheckResponsesPerLevel(upttVariables.Treatment, upttVariables.Response, ReflectionExtensions.GetPropertyDisplayName<UnpairedTTestAnalysisModel>(i => i.Treatment)))
                return ValidationInfo;

            //check response and treatments contain values
            if (!CheckFactorAndResponseNotBlank(upttVariables.Treatment, upttVariables.Response, ReflectionExtensions.GetPropertyDisplayName<UnpairedTTestAnalysisModel>(i => i.Treatment)))
                return ValidationInfo;

            //do data checks on the treatments and response
            if (!FactorAndResponseChecks(upttVariables.Treatment, upttVariables.Response))
                return ValidationInfo;

            //check transformations
            CheckTransformations(upttVariables.ResponseTransformation, upttVariables.Response);

            //if get here then no errors so return true
            return ValidationInfo;
        }

        private bool FactorAndResponseChecks(string treatmentVar, string responseVar)
        {
            int treatmentLevelCount = CountDistinctLevels(treatmentVar);

            //if only 1 level then error
            if (treatmentLevelCount == 1)
            {
                ValidationInfo.AddErrorMessage("The treatment (" + treatmentVar + ") has only one level, please select another factor.");
                return false;
            }
            else if (treatmentLevelCount > 2) // if more than 2 levels then do anova instead...
            {
                ValidationInfo.AddErrorMessage("The treatment (" + treatmentVar + ") has more than two levels, please analyse using Single Measure Parametric Analysis module.");
                return false;
            }

            //Now that the whole column checks have been done, ensure that the treatment and response for each row is ok
            List<string> treatRow = new List<string>();
            List<string> responseRow = new List<string>();

            foreach (DataRow row in DataTable.Rows) //assemble a list of the treatment and responses into each list
            {
                treatRow.Add(row[treatmentVar].ToString());
                responseRow.Add(row[responseVar].ToString());
            }

            for (int i = 0; i < DataTable.Rows.Count; i++) //use for loop cos its easier to compare the indexes of the treatment and response rows
            {
                //Check that the "response" does not contains non-numeric data
                bool parsedOK = Double.TryParse(responseRow[i], out _);
                if (!String.IsNullOrEmpty(responseRow[i]) && !parsedOK)
                {
                    ValidationInfo.AddErrorMessage("The Response (" + responseVar + ") contains non-numerical data which cannot be processed. Please check the input data and make sure the data was entered correctly.");
                    return false;
                }

                //Check that there are no responses where the treatments are blank
                if (String.IsNullOrEmpty(treatRow[i]) && !String.IsNullOrEmpty(responseRow[i]))
                {
                    ValidationInfo.AddErrorMessage("The treatment (" + treatmentVar + ") contains missing data where there are observations present in the Response. Please check the input data and make sure the data was entered correctly.");
                    return false;
                }
            }

            //if got here then all checks ok, return true
            return true;
        }
    }
}