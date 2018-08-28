using SilveRModel.Models;
using System;

namespace SilveRModel.StatsModel
{
    public class AnalysisFactory
    {
        public static IAnalysisModel CreateAnalysisModel(Analysis analysis)
        {
            return CreateAnalysisModel(analysis.Script.ScriptDisplayName, analysis.Dataset);
        }

        public static IAnalysisModel CreateAnalysisModel(string scriptName, Dataset dataset)
        {
            IAnalysisModel model;
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
                case "Two-Sample t-test Analysis":
                    model = new TwoSampleTTestAnalysisModel(dataset);
                    break;
                //case "Correlation Analysis":
                //    model = new CorrelationAnalysisModel(dataset);
                //    break;
                //case "Linear Regression Analysis":
                //    model = new LinearRegressionAnalysisModel(dataset);
                //    break;
                //case "Dose Response Analysis":
                //    model = new DoseResponseAnalysisModel(dataset);
                //    break;
                case "Non-Parametric Analysis":
                    model = new NonParametricAnalysisModel(dataset);
                    break;
                //case "Chi-Squared Analysis":
                //    model = new ChiSquaredAnalysisModel(dataset);
                //    break;
                //case "Survival Analysis":
                //    model = new SurvivalAnalysisModel(dataset);
                //    break;
                case "Graphical Analysis":
                    model = new GraphicalAnalysisModel(dataset);
                    break;
                case "Means Comparison":
                    model = new MeansComparisonModel(dataset);
                    break;
                //case "Multivariate Analysis":
                //    model = new MultivariateAnalysisModel(dataset);
                //    break;
                //case "Nested Design Analysis":
                //    model = new NestedDesignAnalysis(dataset);
                //    break;
                //case "Incomplete Factorial Parametric Analysis":
                //    model = new IncompleteFactorialParametricAnalysisModel(dataset);
                //    break;
                default:
                    throw new Exception("Analysis type not found!");
            }

            return model;
        }
    }
}