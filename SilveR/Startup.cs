using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SilveR.Models;
using SilveR.Services;
using SilveRModel.Models;
using System.Collections.Generic;
using System.Linq;

namespace SilveR
{
    public class Startup
    {
        public static string ContentRootPath { get; private set; }
        public static string AppName { get; private set; }

        public Startup(IHostingEnvironment env)
        {
#if DEBUG
            env.EnvironmentName = "Development";
#else
            env.EnvironmentName = "Production";
#endif

            ContentRootPath = env.ContentRootPath;

            string appName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            if (appName == "SilveR")
                AppName = "ƩilveR";
            else
                AppName = appName;

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
            services.AddDbContext<SilveRContext>(options => options.UseSqlite("Data Source=" + System.Reflection.Assembly.GetEntryAssembly().GetName().Name + ".db"));
            services.AddScoped<SilveRRepository>();

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
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                SilveRContext context = serviceScope.ServiceProvider.GetRequiredService<SilveRContext>();

                context.Database.Migrate();

                IEnumerable<Script> existingScripts = context.Scripts.ToList();

                if (!existingScripts.Any(x => x.ScriptFileName == "SummaryStatistics"))
                {
                    Script summaryStatistics = new Script() { ScriptDisplayName = "Summary Statistics", ScriptFileName = "SummaryStatistics" };
                    context.Scripts.Add(summaryStatistics);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "SingleMeasuresParametricAnalysis"))
                {
                    Script singleMeasureGLM = new Script() { ScriptDisplayName = "Single Measures Parametric Analysis", ScriptFileName = "SingleMeasuresParametricAnalysis" };
                    context.Scripts.Add(singleMeasureGLM);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "RepeatedMeasuresParametricAnalysis"))
                {
                    Script repeatedMeasures = new Script() { ScriptDisplayName = "Repeated Measures Parametric Analysis", ScriptFileName = "RepeatedMeasuresParametricAnalysis" };
                    context.Scripts.Add(repeatedMeasures);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "PValueAdjustment"))
                {
                    Script pValueAdjustment = new Script() { ScriptDisplayName = "P-value Adjustment", ScriptFileName = "PValueAdjustment" };
                    context.Scripts.Add(pValueAdjustment);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "PairedTTestAnalysis"))
                {
                    Script pairedTTest = new Script() { ScriptDisplayName = "Paired t-test Analysis", ScriptFileName = "PairedTTestAnalysis" };
                    context.Scripts.Add(pairedTTest);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "UnpairedTTestAnalysis"))
                {
                    Script unpairedTTest = new Script() { ScriptDisplayName = "Unpaired t-test Analysis", ScriptFileName = "UnpairedTTestAnalysis" };
                    context.Scripts.Add(unpairedTTest);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "TwoSampleTTestAnalysis"))
                {
                    Script twoSampleTTest = new Script() { ScriptDisplayName = "Two-Sample t-test Analysis", ScriptFileName = "TwoSampleTTestAnalysis" };
                    context.Scripts.Add(twoSampleTTest);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "CorrelationAnalysis"))
                {
                    Script correlation = new Script() { ScriptDisplayName = "Correlation Analysis", ScriptFileName = "CorrelationAnalysis" };
                    context.Scripts.Add(correlation);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "LinearRegressionAnalysis"))
                {
                    Script linearRegression = new Script() { ScriptDisplayName = "Linear Regression Analysis", ScriptFileName = "LinearRegressionAnalysis" };
                    context.Scripts.Add(linearRegression);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "DoseResponseAnalysis"))
                {
                    Script doseResponse = new Script() { ScriptDisplayName = "Dose-response Analysis", ScriptFileName = "DoseResponseAnalysis" };
                    context.Scripts.Add(doseResponse);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "NonParametricAnalysis"))
                {
                    Script nonParametrics = new Script() { ScriptDisplayName = "Non-Parametric Analysis", ScriptFileName = "NonParametricAnalysis" };
                    context.Scripts.Add(nonParametrics);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "ChiSquaredAndFishersExactTest"))
                {
                    Script chiSquared = new Script() { ScriptDisplayName = "Chi-Squared and Fishers Exact Test", ScriptFileName = "ChiSquaredAndFishersExactTest" };
                    context.Scripts.Add(chiSquared);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "SurvivalAnalysis"))
                {
                    Script survivalAnalysis = new Script() { ScriptDisplayName = "Survival Analysis", ScriptFileName = "SurvivalAnalysis" };
                    context.Scripts.Add(survivalAnalysis);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "GraphicalAnalysis"))
                {
                    Script graphicalAnalysis = new Script() { ScriptDisplayName = "Graphical Analysis", ScriptFileName = "GraphicalAnalysis" };
                    context.Scripts.Add(graphicalAnalysis);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "MeansComparison"))
                {
                    Script meansComparison = new Script() { ScriptDisplayName = "Means Comparison", ScriptFileName = "MeansComparison" };
                    context.Scripts.Add(meansComparison);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "MultivariateAnalysis"))
                {
                    Script multivariate = new Script() { ScriptDisplayName = "Multivariate Analysis", ScriptFileName = "MultivariateAnalysis" };
                    context.Scripts.Add(multivariate);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "NestedDesignAnalysis"))
                {
                    Script nestedDesign = new Script() { ScriptDisplayName = "Nested Design Analysis", ScriptFileName = "NestedDesignAnalysis" };
                    context.Scripts.Add(nestedDesign);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "IncompleteFactorialParametricAnalysis"))
                {
                    Script incompleteFactorialAnalysis = new Script() { ScriptDisplayName = "Incomplete Factorial Parametric Analysis", ScriptFileName = "IncompleteFactorialParametricAnalysis" };
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