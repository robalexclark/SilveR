using System;
using SilveRModel.Models;

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
                    model = new SummaryStatsModel(dataset);
                    break;
                case "Non-Parametric Analysis":
                    model = new NonParametricModel(dataset);
                    break;
                case "P-value Adjustment":
                    model = new PValueAdjustmentModel();
                    break;
                case "Single Measures Parametric Analysis":
                    model = new SingleMeasuresModel(dataset);
                    break;
                case "Two-Sample t-Test Analysis":
                    model = new TwoSampleTTestModel(dataset);
                    break;
                case "Repeated Measures Parametric Analysis":
                    model = new RepeatedMeasuresModel(dataset);
                    break;
                case "Paired t-test Analysis":
                    model = new PairedTTestModel(dataset);
                    break;
                case "Graphical Analysis":
                    model = new GraphicalAnalysisModel(dataset);
                    break;
                case "Means Comparison":
                    model = new MeansComparisonModel(dataset);
                    break;

                default:
                    throw new Exception("Analysis type not found!");
            }

            return model;
        }
    }
}