using SilveR.Models;
using System;

namespace SilveR.StatsModels
{
    public static class AnalysisFactory
    {
        public static AnalysisModelBase CreateAnalysisModel(Analysis analysis)
        {
            return CreateAnalysisModel(analysis.Script.ScriptDisplayName, analysis.Dataset);
        }

        public static AnalysisModelBase CreateAnalysisModel(string scriptName, Dataset dataset)
        {
            AnalysisModelBase model;
            switch (scriptName)
            {
                case "Summary Statistics":
                    model = new SummaryStatisticsModel(dataset);
                    break;
                case "Single Measures Parametric Analysis":
                    model = new SingleMeasuresParametricAnalysisModel(dataset);
                    break;
                case "Repeated Measures Parametric Analysis":
                    model = new RepeatedMeasuresParametricAnalysisModel(dataset);
                    break;
                case "P-value Adjustment":
                    model = new PValueAdjustmentModel();
                    break;
                case "Paired t-test Analysis":
                    model = new PairedTTestAnalysisModel(dataset);
                    break;
                case "Unpaired t-test Analysis":
                    model = new UnpairedTTestAnalysisModel(dataset);
                    break;
                case "One-Sample t-test Analysis":
                    model = new OneSampleTTestAnalysisModel(dataset);
                    break;
                case "Correlation Analysis":
                    model = new CorrelationAnalysisModel(dataset);
                    break;
                case "Linear Regression Analysis":
                    model = new LinearRegressionAnalysisModel(dataset);
                    break;
                case "Dose-Response and Non-Linear Regression Analysis":
                    model = new DoseResponseAndNonLinearRegesssionAnalysisModel(dataset);
                    break;
                case "Non-Parametric Analysis":
                    model = new NonParametricAnalysisModel(dataset);
                    break;
                case "Chi-Squared and Fishers Exact Test":
                    model = new ChiSquaredAndFishersExactTestModel(dataset);
                    break;
                case "Survival Analysis":
                    model = new SurvivalAnalysisModel(dataset);
                    break;
                case "Graphical Analysis":
                    model = new GraphicalAnalysisModel(dataset);
                    break;
                case "Means Comparison":
                    model = new MeansComparisonModel(dataset);
                    break;
                //case "Multivariate Analysis":
                //    model = new MultivariateAnalysisModel(dataset);
                //    break;
                case "Nested Design Analysis":
                    model = new NestedDesignAnalysisModel(dataset);
                    break;
                case "Incomplete Factorial Parametric Analysis":
                    model = new IncompleteFactorialParametricAnalysisModel(dataset);
                    break;
                default:
                    throw new ArgumentException("Analysis type not found!");
            }

            return model;
        }
    }
}