using SilveRModel.StatsModel;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Data;

namespace SilveRModel.Validators
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
            if (!CheckColumnNames(allVars)) return ValidationInfo;

            if (!CheckIsNumeric(upttVariables.Response))
            {
                ValidationInfo.AddErrorMessage("The response selected contain non-numeric data that cannot be processed. Please check the raw data and make sure the data was entered correctly.");
                return ValidationInfo;
            }

            if (!CheckTreatmentsHaveLevels(upttVariables.Treatment)) return ValidationInfo;

            if (!CheckResponsesPerLevel(upttVariables.Treatment, upttVariables.Response)) return ValidationInfo;

            //check response and treatments contain values
            if (!CheckResponseAndTreatmentsNotBlank(upttVariables.Response, upttVariables.Treatment, "treatment factor")) return ValidationInfo;

            //if get here then no errors so return true
            return ValidationInfo;
        }       
    }
}