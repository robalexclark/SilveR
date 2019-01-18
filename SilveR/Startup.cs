using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SilveR.Models;
using SilveR.Services;
using System.Collections.Generic;
using System.Linq;

namespace SilveR
{
    public class Startup
    {
        public static string ContentRootPath { get; private set; }

        public Startup(IHostingEnvironment env)
        {
#if DEBUG
            env.EnvironmentName = "Development";
#else
            env.EnvironmentName = "Production";
#endif

            ContentRootPath = env.ContentRootPath;

            // Set up configuration sources.
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            configuration = builder.Build();
        }

        private readonly IConfigurationRoot configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<SilveRContext>(options => options.UseSqlite("Data Source=SilveR.db"));
            services.AddScoped<ISilveRRepository, SilveRRepository>();

            //R processing services comprising of R processor and queue services
            services.AddSingleton<IRProcessorService, RProcessorService>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            ProvisionDatabase(app);

            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }


        private void ProvisionDatabase(IApplicationBuilder app)
        {
            //string analysisViewName = viewModel.AnalysisName.Replace(" ", String.Empty).Replace("-", String.Empty);

            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                SilveRContext context = serviceScope.ServiceProvider.GetRequiredService<SilveRContext>();

                context.Database.Migrate();

                IEnumerable<Script> existingScripts = context.Scripts.ToList();

                if (!existingScripts.Any(x => x.ScriptFileName == "SummaryStatistics"))
                {
                    Script summaryStatistics = new Script() { ScriptDisplayName = "Summary Statistics", ScriptFileName = "SummaryStatistics", RequiresDataset = true };
                    context.Scripts.Add(summaryStatistics);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "SingleMeasuresParametricAnalysis"))
                {
                    Script singleMeasureGLM = new Script() { ScriptDisplayName = "Single Measures Parametric Analysis", ScriptFileName = "SingleMeasuresParametricAnalysis", RequiresDataset = true };
                    context.Scripts.Add(singleMeasureGLM);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "RepeatedMeasuresParametricAnalysis"))
                {
                    Script repeatedMeasures = new Script() { ScriptDisplayName = "Repeated Measures Parametric Analysis", ScriptFileName = "RepeatedMeasuresParametricAnalysis", RequiresDataset = true };
                    context.Scripts.Add(repeatedMeasures);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "PValueAdjustment"))
                {
                    Script pValueAdjustment = new Script() { ScriptDisplayName = "P-value Adjustment", ScriptFileName = "PValueAdjustment", RequiresDataset = false };
                    context.Scripts.Add(pValueAdjustment);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "PairedTTestAnalysis"))
                {
                    Script pairedTTest = new Script() { ScriptDisplayName = "Paired t-test Analysis", ScriptFileName = "PairedTTestAnalysis", RequiresDataset = true };
                    context.Scripts.Add(pairedTTest);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "UnpairedTTestAnalysis"))
                {
                    Script unpairedTTest = new Script() { ScriptDisplayName = "Unpaired t-test Analysis", ScriptFileName = "UnpairedTTestAnalysis", RequiresDataset = true };
                    context.Scripts.Add(unpairedTTest);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "OneSampleTTestAnalysis"))
                {
                    Script oneSampleTTest = new Script() { ScriptDisplayName = "One-Sample t-test Analysis", ScriptFileName = "OneSampleTTestAnalysis", RequiresDataset = true };
                    context.Scripts.Add(oneSampleTTest);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "CorrelationAnalysis"))
                {
                    Script correlation = new Script() { ScriptDisplayName = "Correlation Analysis", ScriptFileName = "CorrelationAnalysis", RequiresDataset = true };
                    context.Scripts.Add(correlation);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "LinearRegressionAnalysis"))
                {
                    Script linearRegression = new Script() { ScriptDisplayName = "Linear Regression Analysis", ScriptFileName = "LinearRegressionAnalysis", RequiresDataset = true };
                    context.Scripts.Add(linearRegression);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "DoseResponseAndNonLinearRegressionAnalysis"))
                {
                    Script doseResponse = new Script() { ScriptDisplayName = "Dose-Response and Non-Linear Regression Analysis", ScriptFileName = "DoseResponseAndNonLinearRegressionAnalysis", RequiresDataset = true };
                    context.Scripts.Add(doseResponse);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "NonParametricAnalysis"))
                {
                    Script nonParametrics = new Script() { ScriptDisplayName = "Non-parametric Analysis", ScriptFileName = "NonParametricAnalysis", RequiresDataset = true };
                    context.Scripts.Add(nonParametrics);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "ChiSquaredAndFishersExactTest"))
                {
                    Script chiSquared = new Script() { ScriptDisplayName = "Chi-Squared and Fisher's Exact Test", ScriptFileName = "ChiSquaredAndFishersExactTest", RequiresDataset = true };
                    context.Scripts.Add(chiSquared);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "SurvivalAnalysis"))
                {
                    Script survivalAnalysis = new Script() { ScriptDisplayName = "Survival Analysis", ScriptFileName = "SurvivalAnalysis", RequiresDataset = true };
                    context.Scripts.Add(survivalAnalysis);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "GraphicalAnalysis"))
                {
                    Script graphicalAnalysis = new Script() { ScriptDisplayName = "Graphical Analysis", ScriptFileName = "GraphicalAnalysis", RequiresDataset = true };
                    context.Scripts.Add(graphicalAnalysis);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "MeansComparisonDatasetBasedInputs"))
                {
                    Script meansComparison = new Script() { ScriptDisplayName = "'Comparison of Means' Power Analysis (Dataset Based Inputs)", ScriptFileName = "MeansComparisonDatasetBasedInputs", RequiresDataset = true };
                    context.Scripts.Add(meansComparison);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "MeansComparisonUserBasedInputs"))
                {
                    Script meansComparison = new Script() { ScriptDisplayName = "'Comparison of Means' Power Analysis (User Based Inputs)", ScriptFileName = "MeansComparisonUserBasedInputs", RequiresDataset = false };
                    context.Scripts.Add(meansComparison);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "OneWayANOVADatasetBasedInputs"))
                {
                    Script meansComparison = new Script() { ScriptDisplayName = "'One-way ANOVA' Power Analysis (Dataset Based Inputs)", ScriptFileName = "OneWayANOVADatasetBasedInputs", RequiresDataset = true };
                    context.Scripts.Add(meansComparison);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "OneWayANOVAUserBasedInputs"))
                {
                    Script meansComparison = new Script() { ScriptDisplayName = "'One-way ANOVA' Power Analysis (User Based Inputs)", ScriptFileName = "OneWayANOVAUserBasedInputs", RequiresDataset = false };
                    context.Scripts.Add(meansComparison);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "MultivariateAnalysis"))
                {
                    Script multivariate = new Script() { ScriptDisplayName = "Multivariate Analysis", ScriptFileName = "MultivariateAnalysis", RequiresDataset = true };
                    context.Scripts.Add(multivariate);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "NestedDesignAnalysis"))
                {
                    Script nestedDesign = new Script() { ScriptDisplayName = "Nested Design Analysis", ScriptFileName = "NestedDesignAnalysis", RequiresDataset = true };
                    context.Scripts.Add(nestedDesign);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "IncompleteFactorialParametricAnalysis"))
                {
                    Script incompleteFactorialAnalysis = new Script() { ScriptDisplayName = "Incomplete Factorial Parametric Analysis", ScriptFileName = "IncompleteFactorialParametricAnalysis", RequiresDataset = true };
                    context.Scripts.Add(incompleteFactorialAnalysis);
                }

                context.SaveChanges();
            }
        }
    }

    public class AppSettings
    {
        public string CustomRScriptLocation { get; set; }
    }
}