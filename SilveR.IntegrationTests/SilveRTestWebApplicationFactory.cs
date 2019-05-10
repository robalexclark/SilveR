using ExcelDataReader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SilveR;
using SilveR.Helpers;
using SilveR.Models;
using SilveR.Services;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace ControlledForms.IntegrationTests
{
    public class SilveRTestWebApplicationFactory<TStartup> : WebApplicationFactory<Startup>
    {
        public Dictionary<int, string> SheetNames { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                services.AddDbContext<SilveRContext>(options => options.UseSqlite("Data Source=SilveR.db"));
                services.AddScoped<ISilveRRepository, SilveRRepository>();

                //R processing services comprising of R processor and queue services
                services.AddSingleton<IRProcessorService, RProcessorService>();
                services.AddHostedService<QueuedHostedService>();
                services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

                services.AddMvc();
            });
        }

        public SilveRTestWebApplicationFactory()
        {
            DbContextOptionsBuilder<SilveRContext> optionsBuilder = new DbContextOptionsBuilder<SilveRContext>();
            optionsBuilder.UseSqlite("Data Source=SilveR.db");
            SilveRContext silverContext = new SilveRContext(optionsBuilder.Options);

            SheetNames = silverContext.Datasets.Select(x => new KeyValuePair<int, string>(x.DatasetID, x.DatasetName)).ToDictionary(x => x.Key, x => x.Value);

            //if (File.Exists("SilveR.db"))
            //    File.Delete("SilveR.db");

            //LoadDatasets();
        }

        private void LoadDatasets()
        {
            //SheetNames = new Dictionary<int, string>();

            DbContextOptionsBuilder<SilveRContext> optionsBuilder = new DbContextOptionsBuilder<SilveRContext>();
            optionsBuilder.UseSqlite("Data Source=SilveR.db");
            SilveRContext silverContext = new SilveRContext(optionsBuilder.Options);
            silverContext.Database.Migrate();

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = File.Open("_test dataset.xlsx", FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelReaderFactory.CreateReader(stream))
                {
                    var result = reader.AsDataSet(new ExcelDataSetConfiguration()
                    {
                        ConfigureDataTable = (_) => new ExcelDataTableConfiguration()
                        {
                            UseHeaderRow = true
                        }
                    });

                    int counter = 1;
                    foreach (DataTable dataTable in result.Tables)
                    {
                        this.SheetNames.Add(counter, dataTable.TableName);

                        var dataset = dataTable.GetDataset(dataTable.TableName, 0);
                        silverContext.Datasets.Add(dataset);
                        silverContext.SaveChanges();

                        counter++;
                    }
                }
            }

            IEnumerable<Script> existingScripts = silverContext.Scripts.ToList();

            if (!existingScripts.Any(x => x.ScriptFileName == "SummaryStatistics"))
            {
                Script summaryStatistics = new Script() { ScriptDisplayName = "Summary Statistics", ScriptFileName = "SummaryStatistics", RequiresDataset = true };
                silverContext.Scripts.Add(summaryStatistics);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "SingleMeasuresParametricAnalysis"))
            {
                Script singleMeasureGLM = new Script() { ScriptDisplayName = "Single Measures Parametric Analysis", ScriptFileName = "SingleMeasuresParametricAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(singleMeasureGLM);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "RepeatedMeasuresParametricAnalysis"))
            {
                Script repeatedMeasures = new Script() { ScriptDisplayName = "Repeated Measures Parametric Analysis", ScriptFileName = "RepeatedMeasuresParametricAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(repeatedMeasures);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "PValueAdjustmentUserBasedInputs"))
            {
                Script pValueAdjustmentUserBasedInputs = new Script() { ScriptDisplayName = "P-value Adjustment (User Based Inputs)", ScriptFileName = "PValueAdjustmentUserBasedInputs", RequiresDataset = false };
                silverContext.Scripts.Add(pValueAdjustmentUserBasedInputs);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "PValueAdjustmentDatasetBasedInputs"))
            {
                Script pValueAdjustmentUserBasedInputs = new Script() { ScriptDisplayName = "P-value Adjustment (Dataset Based Inputs)", ScriptFileName = "PValueAdjustmentDatasetBasedInputs", RequiresDataset = true };
                silverContext.Scripts.Add(pValueAdjustmentUserBasedInputs);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "PairedTTestAnalysis"))
            {
                Script pairedTTest = new Script() { ScriptDisplayName = "Paired t-test Analysis", ScriptFileName = "PairedTTestAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(pairedTTest);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "UnpairedTTestAnalysis"))
            {
                Script unpairedTTest = new Script() { ScriptDisplayName = "Unpaired t-test Analysis", ScriptFileName = "UnpairedTTestAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(unpairedTTest);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "OneSampleTTestAnalysis"))
            {
                Script oneSampleTTest = new Script() { ScriptDisplayName = "One-Sample t-test Analysis", ScriptFileName = "OneSampleTTestAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(oneSampleTTest);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "CorrelationAnalysis"))
            {
                Script correlation = new Script() { ScriptDisplayName = "Correlation Analysis", ScriptFileName = "CorrelationAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(correlation);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "LinearRegressionAnalysis"))
            {
                Script linearRegression = new Script() { ScriptDisplayName = "Linear Regression Analysis", ScriptFileName = "LinearRegressionAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(linearRegression);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "DoseResponseAndNonLinearRegressionAnalysis"))
            {
                Script doseResponse = new Script() { ScriptDisplayName = "Dose-Response and Non-Linear Regression Analysis", ScriptFileName = "DoseResponseAndNonLinearRegressionAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(doseResponse);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "NonParametricAnalysis"))
            {
                Script nonParametrics = new Script() { ScriptDisplayName = "Non-parametric Analysis", ScriptFileName = "NonParametricAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(nonParametrics);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "ChiSquaredAndFishersExactTest"))
            {
                Script chiSquared = new Script() { ScriptDisplayName = "Chi-Squared and Fisher's Exact Test", ScriptFileName = "ChiSquaredAndFishersExactTest", RequiresDataset = true };
                silverContext.Scripts.Add(chiSquared);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "SurvivalAnalysis"))
            {
                Script survivalAnalysis = new Script() { ScriptDisplayName = "Survival Analysis", ScriptFileName = "SurvivalAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(survivalAnalysis);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "GraphicalAnalysis"))
            {
                Script graphicalAnalysis = new Script() { ScriptDisplayName = "Graphical Analysis", ScriptFileName = "GraphicalAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(graphicalAnalysis);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "ComparisonOfMeansPowerAnalysisDatasetBasedInputs"))
            {
                Script meansComparison = new Script() { ScriptDisplayName = "'Comparison of Means' Power Analysis (Dataset Based Inputs)", ScriptFileName = "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", RequiresDataset = true };
                silverContext.Scripts.Add(meansComparison);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "ComparisonOfMeansPowerAnalysisUserBasedInputs"))
            {
                Script meansComparison = new Script() { ScriptDisplayName = "'Comparison of Means' Power Analysis (User Based Inputs)", ScriptFileName = "ComparisonOfMeansPowerAnalysisUserBasedInputs", RequiresDataset = false };
                silverContext.Scripts.Add(meansComparison);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "OneWayANOVAPowerAnalysisDatasetBasedInputs"))
            {
                Script meansComparison = new Script() { ScriptDisplayName = "'One-way ANOVA' Power Analysis (Dataset Based Inputs)", ScriptFileName = "OneWayANOVAPowerAnalysisDatasetBasedInputs", RequiresDataset = true };
                silverContext.Scripts.Add(meansComparison);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "OneWayANOVAPowerAnalysisUserBasedInputs"))
            {
                Script meansComparison = new Script() { ScriptDisplayName = "'One-way ANOVA' Power Analysis (User Based Inputs)", ScriptFileName = "OneWayANOVAPowerAnalysisUserBasedInputs", RequiresDataset = false };
                silverContext.Scripts.Add(meansComparison);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "MultivariateAnalysis"))
            {
                Script multivariate = new Script() { ScriptDisplayName = "Multivariate Analysis", ScriptFileName = "MultivariateAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(multivariate);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "NestedDesignAnalysis"))
            {
                Script nestedDesign = new Script() { ScriptDisplayName = "Nested Design Analysis", ScriptFileName = "NestedDesignAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(nestedDesign);
            }

            if (!existingScripts.Any(x => x.ScriptFileName == "IncompleteFactorialParametricAnalysis"))
            {
                Script incompleteFactorialAnalysis = new Script() { ScriptDisplayName = "Incomplete Factorial Parametric Analysis", ScriptFileName = "IncompleteFactorialParametricAnalysis", RequiresDataset = true };
                silverContext.Scripts.Add(incompleteFactorialAnalysis);
            }

            silverContext.SaveChanges();
        }
    }
}