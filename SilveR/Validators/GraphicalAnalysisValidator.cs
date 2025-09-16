using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
            if (!gaVariables.ErrorBarPlotSelected && !gaVariables.BoxplotSelected && !gaVariables.CaseProfilesPlotSelected && !gaVariables.HistogramSelected && !gaVariables.ScatterplotSelected)
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
                    ValidationInfo.AddWarningMessage("You have " + gaVariables.XAxisTransformation + " transformed the x-axis variable ("+ gaVariables.XAxis + "). Unfortunately the x-axis variable is non-numerical and hence cannot be transformed. The transformation has been ignored.");
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
                ValidationInfo.AddErrorMessage("The response variable is not numeric.");
                return ValidationInfo;
            }

            if (!yIsNumeric && gaVariables.ResponseTransformation != "None")
            {
                ValidationInfo.AddWarningMessage("You have " + gaVariables.ResponseTransformation + " transformed the Response (" + gaVariables.Response + "). Unfortunately the Response is non-numerical and hence cannot be transformed. The transformation has been ignored.");
            }

            if (!yIsNumeric && !String.IsNullOrEmpty(gaVariables.Response) && (gaVariables.HistogramSelected || gaVariables.CaseProfilesPlotSelected || gaVariables.BoxplotSelected || gaVariables.ErrorBarPlotSelected))
                ValidationInfo.AddWarningMessage("Only the scatterplot option accepts non-numeric response variables. As the response is non-numeric, no plot other than a scatterplot can be produced.");

            //check that the cat factors are numeric
            if (!String.IsNullOrEmpty(gaVariables.FirstCatFactor) && CheckIsNumeric(gaVariables.FirstCatFactor))
                ValidationInfo.AddWarningMessage("The 1st categorisation factor is numerical. Each numerical value present will constitute a category.");

            if (!String.IsNullOrEmpty(gaVariables.SecondCatFactor) && CheckIsNumeric(gaVariables.SecondCatFactor))
                ValidationInfo.AddWarningMessage("The 2nd categorisation factor is numerical. Each numerical value present will constitute a category.");

            //check that the cat factors have more than 1 level
            if (!String.IsNullOrEmpty(gaVariables.FirstCatFactor) && CountDistinctLevels(gaVariables.FirstCatFactor) == 1)
            {
                ValidationInfo.AddErrorMessage("The 1st categorisation factor has only one distinct level. Please review your selection.");
                return ValidationInfo;
            }

            if (!String.IsNullOrEmpty(gaVariables.SecondCatFactor) && CountDistinctLevels(gaVariables.SecondCatFactor) == 1)
            {
                ValidationInfo.AddErrorMessage("The 2nd categorisation factor has only one distinct level. Please review your selection.");
                return ValidationInfo;
            }

            //check if linear fit selected that both axes are numeric
            if ((!xIsNumeric || !yIsNumeric) && gaVariables.LinearFitSelected)
                ValidationInfo.AddWarningMessage("As one of the axes is non-numeric, the best fit linear line is not included on the scatterplot.");

            //check if SEM or boxplot selected that x is numeric
            if (xIsNumeric && (gaVariables.ErrorBarPlotSelected || gaVariables.BoxplotSelected))
                ValidationInfo.AddWarningMessage("The x-axis variable is numeric, for all plots other than scatterplot this will be treated as categorical.");

            //check if the histogram is selected and the x-axis is blank
            if (gaVariables.HistogramSelected && !String.IsNullOrEmpty(gaVariables.XAxis))
                ValidationInfo.AddWarningMessage("The x-axis variable is ignored in the Histogram plot option. If you wish to include a categorisation factor in the plot, then select the categorisation factors.");

            //go through each row in the datatable and do further checks...
            CheckTransformations(gaVariables.XAxisTransformation, gaVariables.XAxis);

            //if the response has been transformed check if the value is <= 0
            CheckTransformations(gaVariables.ResponseTransformation, gaVariables.Response);

            foreach (DataRow row in DataTable.Rows)
            {
                //check that the x-axis has values for each response value
                if (!String.IsNullOrEmpty(gaVariables.XAxis))
                {
                    if (!String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && String.IsNullOrEmpty(row[gaVariables.XAxis].ToString()))
                    {
                        ValidationInfo.AddWarningMessage("The x-axis variable (" + gaVariables.XAxis + ") contains missing values whereas the Response (" + gaVariables.Response + ") contains data, these responses will therefore be excluded from all analyses. To generate the Histogram containing these excluded responses (which does not require an x-axis variable) deselect the x-axis variable prior to analysis.");
                    }
                    else if (String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && !String.IsNullOrEmpty(row[gaVariables.XAxis].ToString()))
                    {
                        ValidationInfo.AddWarningMessage("The Response (" + gaVariables.Response + ") contains missing values whereas the x-axis variable (" + gaVariables.XAxis + ") contains data. The corresponding x-axis variable values have been excluded from the analysis.");
                    }
                }

                //check that the 1st cat factor has values for each response value
                if (!String.IsNullOrEmpty(gaVariables.FirstCatFactor))
                {
                    if (!String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && String.IsNullOrEmpty(row[gaVariables.FirstCatFactor].ToString()))
                    {
                        ValidationInfo.AddErrorMessage("The 1st categorisation factor (" + gaVariables.FirstCatFactor + ") contains missing values whereas the Response (" + gaVariables.Response + ") contains data.");
                        return ValidationInfo;
                    }
                    else if (String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && !String.IsNullOrEmpty(row[gaVariables.FirstCatFactor].ToString()))
                    {
                        ValidationInfo.AddWarningMessage("The Response (" + gaVariables.Response + ") contains missing values whereas the 1st categorisation factor (" + gaVariables.FirstCatFactor + ") contains data. The corresponding 1st categorisation factor values have been excluded from the analysis.");
                    }
                }

                //check that the 2nd cat factor has values for each response value
                if (!String.IsNullOrEmpty(gaVariables.SecondCatFactor))
                {
                    if (!String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && String.IsNullOrEmpty(row[gaVariables.SecondCatFactor].ToString()))
                    {
                        ValidationInfo.AddErrorMessage("The 2nd categorisation factor (" + gaVariables.SecondCatFactor + ") contains missing values whereas the Response (" + gaVariables.Response + ") contains data.");
                        return ValidationInfo;
                    }
                    else if (String.IsNullOrEmpty(row[gaVariables.Response].ToString()) && !String.IsNullOrEmpty(row[gaVariables.SecondCatFactor].ToString()))
                    {
                        ValidationInfo.AddWarningMessage("The Response (" + gaVariables.Response + ") contains missing values whereas the 2nd categorisation factor (" + gaVariables.SecondCatFactor + ") contains data. The corresponding 2nd categorisation factor values have been excluded from the analysis.");
                    }
                }

                //check that the 1st cat factor has values for each x-axis value
                if (!String.IsNullOrEmpty(gaVariables.XAxis) && !String.IsNullOrEmpty(gaVariables.FirstCatFactor))
                {
                    if (!String.IsNullOrEmpty(row[gaVariables.XAxis].ToString()) && String.IsNullOrEmpty(row[gaVariables.FirstCatFactor].ToString()))
                    {
                        ValidationInfo.AddErrorMessage("The 1st categorisation factor (" + gaVariables.FirstCatFactor + ") contains missing values whereas the x-axis variable (" + gaVariables.XAxis + ") contains data.");
                        return ValidationInfo;
                    }
                }

                //check that the 2nd cat factor has values for each x-axis value
                if (!String.IsNullOrEmpty(gaVariables.XAxis) && !String.IsNullOrEmpty(gaVariables.SecondCatFactor))
                {
                    if (!String.IsNullOrEmpty(row[gaVariables.XAxis].ToString()) && String.IsNullOrEmpty(row[gaVariables.SecondCatFactor].ToString()))
                    {
                        ValidationInfo.AddWarningMessage("The 2nd categorisation factor (" + gaVariables.SecondCatFactor + ") contains missing values whereas the x-axis variable (" + gaVariables.XAxis + ") contains data. The corresponding x-axis variable values have been excluded from the analysis.");
                    }
                }
            }


            //check x axis levels ordering
            if (!CheckLevelsOrdering(gaVariables.XAxisLevelsOrder, gaVariables.XAxis, ReflectionExtensions.GetPropertyDisplayName<GraphicalAnalysisModel>(i => i.XAxisLevelsOrder), ReflectionExtensions.GetPropertyDisplayName<GraphicalAnalysisModel>(i => i.XAxis)))
                return ValidationInfo;

            if (!CheckLevelsOrdering(gaVariables.FirstCatFactorLevelsOrder, gaVariables.FirstCatFactor, ReflectionExtensions.GetPropertyDisplayName<GraphicalAnalysisModel>(i => i.FirstCatFactorLevelsOrder), ReflectionExtensions.GetPropertyDisplayName<GraphicalAnalysisModel>(i => i.FirstCatFactor)))
                return ValidationInfo;

            if (!CheckLevelsOrdering(gaVariables.SecondCatFactorLevelsOrder, gaVariables.SecondCatFactor, ReflectionExtensions.GetPropertyDisplayName<GraphicalAnalysisModel>(i => i.SecondCatFactorLevelsOrder), ReflectionExtensions.GetPropertyDisplayName<GraphicalAnalysisModel>(i => i.SecondCatFactor)))
                return ValidationInfo;

            if(gaVariables.HistogramSelected && (gaVariables.XAxisMin.HasValue || gaVariables.XAxisMax.HasValue))
            {
                ValidationInfo.AddWarningMessage("For the histogram plot the axis ranges cannot be changed.");
            }

            if (gaVariables.BoxplotSelected && (gaVariables.XAxisMin.HasValue || gaVariables.XAxisMax.HasValue))
            {
                ValidationInfo.AddWarningMessage("For the boxplot the X-axis range cannot be changed.");
            }

            if (gaVariables.CaseProfilesPlotSelected && !xIsNumeric && (gaVariables.XAxisMin.HasValue || gaVariables.XAxisMax.HasValue))
            {
                ValidationInfo.AddWarningMessage("On the case profiles plot, as the X-axis variable is categorical the X-axis range cannot be changed.");
            }

            if (gaVariables.ErrorBarPlotSelected && (gaVariables.XAxisMin.HasValue || gaVariables.XAxisMax.HasValue))
            {
                ValidationInfo.AddWarningMessage("For the Means with error bars plot the X-axis range cannot be changed.");
            }

            if (gaVariables.ScatterplotSelected && !yIsNumeric && (gaVariables.YAxisMin.HasValue || gaVariables.YAxisMax.HasValue))
            {
                ValidationInfo.AddWarningMessage("On the scatterplot, as the Y-axis variable is categorical the Y-axis range cannot be changed.");
            }

            if (gaVariables.ScatterplotSelected && !xIsNumeric && (gaVariables.XAxisMin.HasValue || gaVariables.XAxisMax.HasValue))
            {
                ValidationInfo.AddWarningMessage("On the scatterplot, as the X-axis variable is categorical the X-axis range cannot be changed.");
            }


            if (gaVariables.XAxisMin.HasValue != gaVariables.XAxisMax.HasValue)
            {
                ValidationInfo.AddErrorMessage("You have only defined one of the values for the X-axis range - both are required to generate the plot.");
                return ValidationInfo;
            }

            if (gaVariables.YAxisMin.HasValue != gaVariables.YAxisMax.HasValue)
            {
                ValidationInfo.AddErrorMessage("You have only defined one of the values for the Y-axis range - both are required to generate the plot.");
                return ValidationInfo;
            }

            if (gaVariables.XAxisMin>= gaVariables.XAxisMax)
            {
                ValidationInfo.AddErrorMessage("For the X-axis range, the minimum value needs to be less than the maximum.");
                return ValidationInfo;
            }

            if (gaVariables.YAxisMin >= gaVariables.YAxisMax)
            {
                ValidationInfo.AddErrorMessage("For the Y-axis range, the minimum value needs to be less than the maximum.");
                return ValidationInfo;
            }


            //if get here then no errors so return true
            return ValidationInfo;
        }

        private bool CheckLevelsOrdering(string levelsVariable, string actualVariable, string levelsVariableDisplayName, string actualVariableDisplayName)
        {
            if (!String.IsNullOrWhiteSpace(levelsVariable))
            {
                //check that an x axis has actually been selected
                if (actualVariable == null)
                {
                    ValidationInfo.AddErrorMessage("The " + levelsVariableDisplayName + " order has been entered, but no " + actualVariableDisplayName + " has been selected.");
                    return false;
                }
                else
                {
                    //get levels
                    IEnumerable<string> levels = GetLevels(actualVariable);

                    string[] levelsOrder = levelsVariable.Split(','); //split list by comma

                    foreach (string level in levelsOrder)
                    {
                        if (!levels.Contains(level.Trim()))
                        {
                            ValidationInfo.AddErrorMessage("The " + levelsVariableDisplayName + " order contains levels that do not exist in the dataset, either the levels have not been entered correctly or the levels are not separated by commas. The list needs to be comma separated.");
                            return false;
                        }
                    }

                    //do this last as above step verifies that the levels entered are correct
                    if (ContainsDuplicates(levelsOrder))
                    {
                        ValidationInfo.AddErrorMessage("The " + levelsVariableDisplayName + " order contains duplicate levels.");
                        return false;
                    }
                }
            }

            return true;
        }


        private bool ContainsDuplicates(IEnumerable<string> enumerable)
        {
            HashSet<string> knownKeys = new HashSet<string>();
            return enumerable.Any(item => !knownKeys.Add(item));
        }
    }
}