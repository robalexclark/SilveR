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
            AnalysisModelBase model = scriptName switch
            {
                "SummaryStatistics" => new SummaryStatisticsModel(dataset),
                "SingleMeasuresParametricAnalysis" => new SingleMeasuresParametricAnalysisModel(dataset),
                "RepeatedMeasuresParametricAnalysis" => new RepeatedMeasuresParametricAnalysisModel(dataset),
                "PValueAdjustmentUserBasedInputs" => new PValueAdjustmentUserBasedInputsModel(),
                "PValueAdjustmentDatasetBasedInputs" => new PValueAdjustmentDatasetBasedInputsModel(dataset),
                "PairedTTestAnalysis" => new PairedTTestAnalysisModel(dataset),
                "UnpairedTTestAnalysis" => new UnpairedTTestAnalysisModel(dataset),
                "OneSampleTTestAnalysis" => new OneSampleTTestAnalysisModel(dataset),
                "CorrelationAnalysis" => new CorrelationAnalysisModel(dataset),
                "LinearRegressionAnalysis" => new LinearRegressionAnalysisModel(dataset),
                "LogisticRegressionAnalysis" => new LogisticRegressionAnalysisModel(dataset),
                "DoseResponseAndNonLinearRegressionAnalysis" => new DoseResponseAndNonLinearRegressionAnalysisModel(dataset),
                "NonParametricAnalysis" => new NonParametricAnalysisModel(dataset),
                "ChiSquaredAndFishersExactTest" => new ChiSquaredAndFishersExactTestModel(dataset),
                "SurvivalAnalysis" => new SurvivalAnalysisModel(dataset),
                "GraphicalAnalysis" => new GraphicalAnalysisModel(dataset),
                "ComparisonOfMeansPowerAnalysisUserBasedInputs" => new ComparisonOfMeansPowerAnalysisUserBasedInputsModel(),
                "ComparisonOfMeansPowerAnalysisDatasetBasedInputs" => new ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel(dataset),
                "OneWayANOVAPowerAnalysisUserBasedInputs" => new OneWayANOVAPowerAnalysisUserBasedInputsModel(),
                "OneWayANOVAPowerAnalysisDatasetBasedInputs" => new OneWayANOVAPowerAnalysisDatasetBasedInputsModel(dataset),
                "MultivariateAnalysis" => new MultivariateAnalysisModel(dataset),
                "NestedDesignAnalysis" => new NestedDesignAnalysisModel(dataset),
                "IncompleteFactorialParametricAnalysis" => new IncompleteFactorialParametricAnalysisModel(dataset),
                "SingleMeasuresToRepeatedMeasuresDataTransformation" => new SingleMeasuresToRepeatedMeasuresDataTransformationModel(dataset),
                "AreaUnderCurveDataTransformation" => new AreaUnderCurveDataTransformationModel(dataset),
                "EquivalenceTOSTTest" => new EquivalenceTOSTTestModel(dataset),
                "RRunner" => new RRunnerModel(dataset),
                _ => throw new ArgumentException("Analysis type not found!"),
            };
            return model;
        }
    }
}