using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;

namespace SilveR.Validators
{
    public class GraphicalAnalysisValidator : ValidatorBase
    {
        private readonly GraphicalAnalysisModel gaVariables;

        public GraphicalAnalysisValidator(GraphicalAnalysisModel ga)
            : base(ga.DataTable)
        {
            gaVariables = ga;
        }

        public override ValidationInfo Validate()
        {
            //first just check to ensure that the user has actually selected something to output!
            if (!gaVariables.SEMPlotSelected && !gaVariables.BoxplotSelected && !gaVariables.CaseProfilesPlotSelected && !gaVariables.HistogramSelected && !gaVariables.ScatterplotSelected)
            {
                ValidationInfo.AddErrorMessage("You have not selected anything to output!");
                return ValidationInfo;
            }

            //go through all the column names, if any are numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.Add(gaVariables.XAxis);
            allVars.Add(gaVariables.Response);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            //if the x-axis is selected then check if it is numeric
            bool xIsNumeric = false;
            if (!String.IsNullOrEmpty(gaVariables.XAxis))
            {
                xIsNumeric = CheckIsNumeric(gaVariables.XAxis);
                //if the x-axis has been transformed then check that all is numeric
                if (!xIsNumeric && gaVariables.XAxisTransformation != "None")
                {
                    ValidationInfo.AddWarningMessage("You have " + gaVariables.XAxisTransformation + " transformed the x-axis variable. Unfortunately the x-axis variable is non-numerical and hence cannot be transformed. The transformation has been ignored.");
                }
            }

            //if the response is selected then check if it is numeric
            bool yIsNumeric = CheckIsNumeric(gaVariables.Response);

            //check that both axes are not numeric (or y is not numeric if histogram selected)
            if (!yIsNumeric && String.IsNullOrEmpty(gaVariables.XAxis) && gaVariables.HistogramSelected)
            {
                ValidationInfo.AddErrorMessage("As the Response is non-numeric, no histogram or other output has been produced.");
                return ValidationInfo;
            }
            else if (!xIsNumeric && !yIsNumeric)
            {
                ValidationInfo.AddErrorMessage("Both the x-axis and the response variables are not numeric.");
                return ValidationInfo;
            }

            if (!yIsNumeric && gaVariables.ResponseTransformation != "None")
            {
                ValidationInfo.AddWarningMessage("You have " + gaVariables.ResponseTransformation + " transformed the response variable. Unfortunately the response variable is non-numerical and hence cannot be transformed. The transformation has been ignored.");
            }

            if (!yIsNumeric && !String.IsNullOrEmpty(gaVariables.Response) && (gaVariables.HistogramSelected || gaVariables.CaseProfilesPlotSelected || gaVariables.BoxplotSelected || gaVariables.SEMPlotSelected))
                ValidationInfo.AddWarningMessage("Only the scatterplot option accepts non-numeric response variables. As the response is non-numeric, no plot other than a scatterplot can be produced.");

            //check that the cat factors are numeric
            if (!String.IsNullOrEmpty(gaVariables.FirstCatFactor) && CheckIsNumeric(gaVariables.FirstCatFactor))
                ValidationInfo.AddWarningMessage("The 1st categorisation factor selected is numerical. Each numerical value present will consitute a category");

            if (!String.IsNullOrEmpty(gaVariables.SecondCatFactor) && CheckIsNumeric(gaVariables.SecondCatFactor))
                ValidationInfo.AddWarningMessage("The 2nd categorisation factor selected is numerical. Each numerical value present will consitute a category");

            //check that the cat factors have more than 1 level
            if (!String.IsNullOrEmpty(gaVariables.FirstCatFactor) && CountDistinctLevels(gaVariables.FirstCatFactor) == 1)
            {
                ValidationInfo.AddErrorMessage("The 1st categorisation factor selected has only one distinct level. Please review your selection.");
                return ValidationInfo;
            }

            if (!String.IsNullOrEmpty(gaVariables.SecondCatFactor) && CountDistinctLevels(gaVariables.SecondCatFactor) == 1)
            {
                ValidationInfo.AddErrorMessage("The 2nd categorisation factor selected has only one distinct level. Please review your selection.");
                return ValidationInfo;
            }

            //check if linear fit selected that both axes are numeric
            if ((!xIsNumeric || !yIsNumeric) && gaVariables.LinearFitSelected)
                ValidationInfo.AddWarningMessage("As one of the axes is non-numeric, the best fit line is not included on the scatterplot.");

            //check if SEM or boxplot selected that x is numeric
            if (xIsNumeric && (gaVariables.SEMPlotSelected || gaVariables.BoxplotSelected))
                ValidationInfo.AddWarningMessage("The selected x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.");

            //check if the histogram is selected and the x-axis is blank
            if (gaVariables.HistogramSelected && !String.IsNullOrEmpty(gaVariables.XAxis))
                ValidationInfo.AddWarningMessage("The x-axis variable is ignored in the Histogram plot option. If you wish to include a categorisation factor in the plot, then select the categorisation factors.");

            //go through each row in the datatable and do further checks...
            CheckTransformations(DataTable, gaVariables.XAxisTransformation, gaVariables.XAxis);

            //if the response has been transformed check if the value is <= 0
            CheckTransformations(DataTable, gaVariables.ResponseTransformation, gaVariables.Response);

            foreach (DataRow row in DataTable.Rows)
            {
                //check that the x-axis has values for each response value
                if (!String.IsNullOrEmpty(gaVariables.XAxis))
                {
                    if (!String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && String.IsNullOrEmpty(row[gaVariables.XAxis].ToString()))
                    {
                        ValidationInfo.AddErrorMessage("The x-axis variable selected (" + gaVariables.XAxis + ") contains missing values whereas the response variable (" + gaVariables.Response + ") contains data. To generate the Histogram (which does not require an x-axis variable) deselect the x-axis variable prior to analysis.");
                        return ValidationInfo;
                    }

                    if (String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && !String.IsNullOrEmpty(row[gaVariables.XAxis].ToString()))
                    {
                        ValidationInfo.AddWarningMessage("The response selected (" + gaVariables.Response + ") contains missing values whereas the x-axis variable (" + gaVariables.XAxis + ") contains data. The corresponding x-axis variable values have been excluded from the analysis.");
                    }
                }

                //check that the 1st cat factor has values for each response value
                if (!String.IsNullOrEmpty(gaVariables.FirstCatFactor))
                {
                    if (!String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && String.IsNullOrEmpty(row[gaVariables.FirstCatFactor].ToString()))
                    {
                        ValidationInfo.AddErrorMessage("The 1st categorisation factor selected (" + gaVariables.FirstCatFactor + ") contains missing values whereas the response variable (" + gaVariables.Response + ") contains data.");
                        return ValidationInfo;
                    }

                    if (String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && !String.IsNullOrEmpty(row[gaVariables.FirstCatFactor].ToString()))
                    {
                        ValidationInfo.AddWarningMessage("The response selected (" + gaVariables.Response + ") contains missing values whereas the 1st categorisation factor (" + gaVariables.FirstCatFactor + ") contains data. The corresponding 1st categorisation factor values have been excluded from the analysis.");
                    }
                }

                //check that the 2nd cat factor has values for each response value
                if (!String.IsNullOrEmpty(gaVariables.SecondCatFactor))
                {
                    if (!String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && String.IsNullOrEmpty(row[gaVariables.SecondCatFactor].ToString()))
                    {
                        ValidationInfo.AddErrorMessage("The 2nd categorisation factor selected (" + gaVariables.SecondCatFactor + ") contains missing values whereas the response variable (" + gaVariables.Response + ") contains data.");
                        return ValidationInfo;
                    }

                    if (String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && !String.IsNullOrEmpty(row[gaVariables.SecondCatFactor].ToString()))
                    {
                        ValidationInfo.AddWarningMessage("The response selected (" + gaVariables.Response + ") contains missing values whereas the 2nd categorisation factor (" + gaVariables.SecondCatFactor + ") contains data. The corresponding 2nd categorisation factor values have been excluded from the analysis.");
                    }
                }


                //check that the 1st cat factor has values for each x-axis value
                if (!String.IsNullOrEmpty(gaVariables.XAxis) && !String.IsNullOrEmpty(gaVariables.FirstCatFactor))
                {
                    if (!String.IsNullOrEmpty(row[gaVariables.XAxis].ToString()) && String.IsNullOrEmpty(row[gaVariables.FirstCatFactor].ToString()))
                    {
                        ValidationInfo.AddErrorMessage("The 1st categorisation factor selected (" + gaVariables.FirstCatFactor + ") contains missing values whereas the x-axis variable (" + gaVariables.XAxis + ") contains data.");
                        return ValidationInfo;
                    }
                }

                //check that the 2nd cat factor has values for each x-axis value
                if (!String.IsNullOrEmpty(gaVariables.XAxis) && !String.IsNullOrEmpty(gaVariables.SecondCatFactor))
                {
                    if (!String.IsNullOrEmpty(row[gaVariables.XAxis].ToString()) && String.IsNullOrEmpty(row[gaVariables.SecondCatFactor].ToString()))
                    {
                        ValidationInfo.AddWarningMessage("The 2nd categorisation factor selected (" + gaVariables.SecondCatFactor + ") contains missing values whereas the x-axis variable (" + gaVariables.XAxis + ") contains data. The corresponding x-axis variable values have been excluded from the analysis.");
                    }
                }
            }

            //if get here then no errors so return true
            return ValidationInfo;
        }
    }
}