using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SilveR.Validators
{
    public class MultivariateAnalysisValidator : ValidatorBase
    {
        private readonly MultivariateAnalysisModel maVariables;

        public MultivariateAnalysisValidator(MultivariateAnalysisModel ma)
            : base(ma.DataTable)
        {
            maVariables = ma;
        }

        public override ValidationInfo Validate()
        {
            //go through all the column names, if any are not numeric then stop the analysis
            List<string> allVars = new List<string>();
            allVars.AddRange(maVariables.Responses);

            if (maVariables.CategoricalPredictor != null)
                allVars.Add(maVariables.CategoricalPredictor);

            if (maVariables.ContinuousPredictors != null)
                allVars.AddRange(maVariables.ContinuousPredictors);

            allVars.Add(maVariables.CaseID);

            if (!CheckColumnNames(allVars))
                return ValidationInfo;

            if (maVariables.CategoricalPredictor != null)
            {
                if (!CheckFactorsHaveLevels(maVariables.CategoricalPredictor, true))
                    return ValidationInfo;

                if (CountDistinctLevels(maVariables.CategoricalPredictor) < 2)
                {
                    ValidationInfo.AddErrorMessage("Error: At least one of your categorical predictors only has one level. Please remove it from the analysis.");
                    return ValidationInfo;
                }
            }


            List<int> responseCounts = new List<int>();
            //Go through each response
            foreach (string response in maVariables.Responses)
            {
                if (!CheckIsNumeric(response))
                {
                    ValidationInfo.AddErrorMessage("Error: The response variable (" + response + ") selected contains non-numerical data. Please amend the dataset prior to running the analysis.");
                    return ValidationInfo;
                }

                //check no of responses are the same (actual check is later)
                int responseCount = 0;
                foreach (DataRow row in DataTable.Rows)
                {
                    if (!String.IsNullOrEmpty(row[response].ToString()))
                    {
                        responseCount = responseCount + 1;
                    }
                }
                responseCounts.Add(responseCount);

                CheckTransformations(DataTable, maVariables.ResponseTransformation, response);

                //Do checks to ensure that treatments contain a response etc and the responses contain a treatment etc...
                if (maVariables.CategoricalPredictor != null && maVariables.AnalysisType == MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis)
                {
                    //check response and cat factors contain values
                    if (!CheckFactorAndResponseNotBlank(maVariables.CategoricalPredictor, response, ReflectionExtensions.GetPropertyDisplayName<MultivariateAnalysisModel>(i => i.CategoricalPredictor)))
                        return ValidationInfo;

                    if (!CheckResponsesPerLevel(maVariables.CategoricalPredictor, response, ReflectionExtensions.GetPropertyDisplayName<MultivariateAnalysisModel>(i => i.CategoricalPredictor)))
                        return ValidationInfo;
                }

                //Go through each continuous predictor
                if (maVariables.ContinuousPredictors != null && maVariables.AnalysisType == MultivariateAnalysisModel.AnalysisOption.PartialLeastSquares)
                {
                    foreach (string continuousPredictor in maVariables.ContinuousPredictors)
                    {
                        if (!CheckIsNumeric(continuousPredictor))
                        {
                            ValidationInfo.AddErrorMessage("Error: The continuous predictor variable (" + continuousPredictor + ") selected contains non-numerical data. Please amend the dataset prior to running the analysis.");
                            return ValidationInfo;
                        }
                        else if (!CheckFactorAndResponseNotBlank(continuousPredictor, response, ReflectionExtensions.GetPropertyDisplayName<MultivariateAnalysisModel>(i => i.ContinuousPredictors)))
                            return ValidationInfo;
                    }
                }

                if (maVariables.CaseID != null)
                {
                    //check response and cat factors contain values
                    if (!CheckFactorAndResponseNotBlank(maVariables.CaseID, response, ReflectionExtensions.GetPropertyDisplayName<MultivariateAnalysisModel>(i => i.CaseID)))
                        return ValidationInfo;
                }
            }

            if (!responseCounts.All(x => x == responseCounts.First()))
            {
                ValidationInfo.AddErrorMessage("Error: Not all the responses contain the same number of values. Please amend the dataset prior to running the analysis.");
                return ValidationInfo;
            }

            if (maVariables.AnalysisType == MultivariateAnalysisModel.AnalysisOption.PrincipalComponentsAnalysis)
            {
                if (maVariables.CategoricalPredictor != null)
                {
                    ValidationInfo.AddWarningMessage("Warning: When performing a PCA analysis the categorical predictor you have selected will not be used. If you do need to use them in the analysis, then another analysis option may be more appropriate.");
                }
                else if (maVariables.ContinuousPredictors != null)
                {
                    ValidationInfo.AddWarningMessage("Warning: When performing a PCA analysis the continuous predictors you have selected will not be used. If you do need to use them in the analysis, then another analysis option may be more appropriate.");
                }
            }
            else if (maVariables.AnalysisType == MultivariateAnalysisModel.AnalysisOption.ClusterAnalysis)
            {
                if (maVariables.CategoricalPredictor != null)
                {
                    ValidationInfo.AddWarningMessage("Warning: When performing a Cluster analysis the categorical predictor you have selected will not be used. If you do need to use them in the analysis, then another analysis option may be more appropriate.");
                }
                if (maVariables.ContinuousPredictors != null)
                {
                    ValidationInfo.AddWarningMessage("Warning: When performing a Cluster analysis the continuous predictors you have selected will not be used. If you do need to use them in the analysis, then another analysis option may be more appropriate.");
                }
            }
            else if (maVariables.AnalysisType == MultivariateAnalysisModel.AnalysisOption.LinearDiscriminantAnalysis)
            {
                if (maVariables.CategoricalPredictor == null)
                {
                    ValidationInfo.AddErrorMessage("Warning: When performing a LDA analysis a categorical predictor is required.");
                }
                if (maVariables.ContinuousPredictors != null)
                {
                    ValidationInfo.AddWarningMessage("Warning: When performing a LDA analysis the continuous predictors you have selected will not be used. If you do need to use them in the analysis, then another analysis option may be more appropriate.");
                }
            }
            //else if (maVariables.AnalysisType == MultivariateAnalysisModel.AnalysisOption.PartialLeastSquares)
            //{
            //    if (maVariables.CategoricalPredictor != null)
            //    {
            //        ValidationInfo.AddWarningMessage("Warning: When performing a PLS analysis the categorical predictor you have selected will not be used. If you do need to use them in the analysis, then another analysis option may be more appropriate.");
            //    }
            //    if (maVariables.ContinuousPredictors == null)
            //    {
            //        ValidationInfo.AddErrorMessage("Warning: When performing a LDA analysis a continuous predictor(s) is required.");
            //    }
            //}

            //display the warning messages (if any) and return the result
            return ValidationInfo;
        }
    }
}