using SilveR.Models;
using System;

namespace SilveR.StatsModels
{
    public static class AnalysisFactory
    {
        public static AnalysisModelBase CreateAnalysisModel(Analysis analysis)
        {
            return CreateAnalysisModel(analysis.Script.ScriptFileName, analysis.Dataset);
        }

        public static AnalysisModelBase CreateAnalysisModel(string scriptName, Dataset dataset)
        {
            AnalysisModelBase model;
            switch (scriptName)
            {
                case "SummaryStatistics":
                    model = new SummaryStatisticsModel(dataset);
                    break;
                case "SingleMeasuresParametricAnalysis":
                    model = new SingleMeasuresParametricAnalysisModel(dataset);
                    break;
                case "RepeatedMeasuresParametricAnalysis":
                    model = new RepeatedMeasuresParametricAnalysisModel(dataset);
                    break;
                case "PValueAdjustment":
                    model = new PValueAdjustmentModel();
                    break;
                case "PairedTTestAnalysis":
                    model = new PairedTTestAnalysisModel(dataset);
                    break;
                case "UnpairedTTestAnalysis":
                    model = new UnpairedTTestAnalysisModel(dataset);
                    break;
                case "OneSampleTTestAnalysis":
                    model = new OneSampleTTestAnalysisModel(dataset);
                    break;
                case "CorrelationAnalysis":
                    model = new CorrelationAnalysisModel(dataset);
                    break;
                case "LinearRegressionAnalysis":
                    model = new LinearRegressionAnalysisModel(dataset);
                    break;
                case "DoseResponseAndNonLinearRegressionAnalysis":
                    model = new DoseResponseAndNonLinearRegesssionAnalysisModel(dataset);
                    break;
                case "NonParametricAnalysis":
                    model = new NonParametricAnalysisModel(dataset);
                    break;
                case "ChiSquaredAndFishersExactTest":
                    model = new ChiSquaredAndFishersExactTestModel(dataset);
                    break;
                case "SurvivalAnalysis":
                    model = new SurvivalAnalysisModel(dataset);
                    break;
                case "GraphicalAnalysis":
                    model = new GraphicalAnalysisModel(dataset);
                    break;
                case "ComparisonOfMeansPowerAnalysisUserBasedInputs":
                    model = new ComparisonOfMeansPowerAnalysisUserBasedInputsModel();
                    break;
                case "ComparisonOfMeansPowerAnalysisDatasetBasedInputs":
                    model = new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel(dataset);
                    break;
                case "OneWayANOVAPowerAnalysisUserBasedInputs":
                    model = new OneWayANOVAPowerAnalysisUserBasedInputsModel();
                    break;
                case "OneWayANOVAPowerAnalysisDatasetBasedInputs":
                    model = new OneWayANOVAPowerAnalysisDatasetBasedInputsModel(dataset);
                    break;
                //case "MultivariateAnalysis":
                //    model = new MultivariateAnalysisModel(dataset);
                //    break;
                case "NestedDesignAnalysis":
                    model = new NestedDesignAnalysisModel(dataset);
                    break;
                case "IncompleteFactorialParametricAnalysis":
                    model = new IncompleteFactorialParametricAnalysisModel(dataset);
                    break;
                default:
                    throw new ArgumentException("Analysis type not found!");
            }

            return model;
        }
    }
}