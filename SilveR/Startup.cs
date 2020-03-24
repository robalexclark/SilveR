using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SilveR.Models;
using SilveR.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Globalization;

namespace SilveR
{
    public class Startup
    {
        public static string ContentRootPath { get; private set; }

        public Startup(IWebHostEnvironment env)
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
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false);

            configuration = builder.Build();

            System.Threading.Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
        }

        private readonly IConfigurationRoot configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.AppName);
            Directory.CreateDirectory(appDataFolder); //create the app folder if it does not exist

            services.AddDbContext<SilveRContext>(options => options.UseSqlite("Data Source=" + Path.Combine(appDataFolder, Program.AppName + ".db")));
            services.AddScoped<ISilveRRepository, SilveRRepository>();

            //R processing services comprising of R processor and queue services
            services.AddSingleton<IRProcessorService, RProcessorService>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddRouting();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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

            app.UseRouting();

            app.UseStaticFiles();

            app.UseRequestLocalization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute(); // Map conventional MVC controllers using the default route
            });

            // Open the Electron-Window here
            //Task.Run(async () => await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions { Title = Program.AppName, Width = 1280, Height = 1024, AutoHideMenuBar = true, WebPreferences = new WebPreferences { NodeIntegration = false } }));

            if (HybridSupport.IsElectronActive)
            {
                ElectronBootstrap();
            }
        }

        public async void ElectronBootstrap()
        {
            BrowserWindow browserWindow = await Electron.WindowManager.CreateWindowAsync(new BrowserWindowOptions
            {
                Title = Program.AppName,
                Width = 1280,
                Height = 1024,
                AutoHideMenuBar = true,
                WebPreferences = new WebPreferences { NodeIntegration = false }
            });

            await browserWindow.WebContents.Session.ClearCacheAsync();

            MenuItem[] contextMenu = new MenuItem[] {
                new MenuItem { Label = "Copy", Accelerator = "CmdOrCtrl+C", Role = MenuRole.copy }
            };

            Electron.Menu.SetContextMenu(browserWindow, contextMenu);

            browserWindow.OnReadyToShow += () =>
            {
                browserWindow.Show();
                browserWindow.Maximize();
            };
        }


        private void ProvisionDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                SilveRContext context = serviceScope.ServiceProvider.GetRequiredService<SilveRContext>();

                bool retry = false;
            retry:
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    if (!retry)
                    {
                        context.Database.EnsureDeleted();
                        retry = true;
                        goto retry;
                    }
                    else
                        throw new Exception("Database creation failed!", ex);
                }


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

                if (!existingScripts.Any(x => x.ScriptFileName == "PValueAdjustmentUserBasedInputs"))
                {
                    Script pValueAdjustmentUserBasedInputs = new Script() { ScriptDisplayName = "P-value Adjustment (User Based Inputs)", ScriptFileName = "PValueAdjustmentUserBasedInputs", RequiresDataset = false };
                    context.Scripts.Add(pValueAdjustmentUserBasedInputs);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "PValueAdjustmentDatasetBasedInputs"))
                {
                    Script pValueAdjustmentUserBasedInputs = new Script() { ScriptDisplayName = "P-value Adjustment (Dataset Based Inputs)", ScriptFileName = "PValueAdjustmentDatasetBasedInputs", RequiresDataset = true };
                    context.Scripts.Add(pValueAdjustmentUserBasedInputs);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "PairedTTestAnalysis"))
                {
                    Script pairedTTest = new Script() { ScriptDisplayName = "Extended Paired t-test Analysis", ScriptFileName = "PairedTTestAnalysis", RequiresDataset = true };
                    context.Scripts.Add(pairedTTest);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "UnpairedTTestAnalysis"))
                {
                    Script unpairedTTest = new Script() { ScriptDisplayName = "Unpaired t-test Analysis", ScriptFileName = "UnpairedTTestAnalysis", RequiresDataset = true };
                    context.Scripts.Add(unpairedTTest);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "OneSampleTTestAnalysis"))
                {
                    Script oneSampleTTest = new Script() { ScriptDisplayName = "One-sample t-test Analysis", ScriptFileName = "OneSampleTTestAnalysis", RequiresDataset = true };
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

                if (!existingScripts.Any(x => x.ScriptFileName == "LogisticRegressionAnalysis"))
                {
                    Script logisticRegression = new Script() { ScriptDisplayName = "Logistic Regression Analysis", ScriptFileName = "LogisticRegressionAnalysis", RequiresDataset = true };
                    context.Scripts.Add(logisticRegression);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "DoseResponseAndNonLinearRegressionAnalysis"))
                {
                    Script doseResponse = new Script() { ScriptDisplayName = "Dose-response and Non-linear Regression Analysis", ScriptFileName = "DoseResponseAndNonLinearRegressionAnalysis", RequiresDataset = true };
                    context.Scripts.Add(doseResponse);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "NonParametricAnalysis"))
                {
                    Script nonParametrics = new Script() { ScriptDisplayName = "Non-parametric Analysis", ScriptFileName = "NonParametricAnalysis", RequiresDataset = true };
                    context.Scripts.Add(nonParametrics);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "ChiSquaredAndFishersExactTest"))
                {
                    Script chiSquared = new Script() { ScriptDisplayName = "Chi-squared and Fisher's Exact Test", ScriptFileName = "ChiSquaredAndFishersExactTest", RequiresDataset = true };
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

                if (!existingScripts.Any(x => x.ScriptFileName == "ComparisonOfMeansPowerAnalysisDatasetBasedInputs"))
                {
                    Script meansComparison = new Script() { ScriptDisplayName = "'Comparison of Means' Power Analysis (Dataset Based Inputs)", ScriptFileName = "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", RequiresDataset = true };
                    context.Scripts.Add(meansComparison);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "ComparisonOfMeansPowerAnalysisUserBasedInputs"))
                {
                    Script meansComparison = new Script() { ScriptDisplayName = "'Comparison of Means' Power Analysis (User Based Inputs)", ScriptFileName = "ComparisonOfMeansPowerAnalysisUserBasedInputs", RequiresDataset = false };
                    context.Scripts.Add(meansComparison);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "OneWayANOVAPowerAnalysisDatasetBasedInputs"))
                {
                    Script meansComparison = new Script() { ScriptDisplayName = "'One-way ANOVA' Power Analysis (Dataset Based Inputs)", ScriptFileName = "OneWayANOVAPowerAnalysisDatasetBasedInputs", RequiresDataset = true };
                    context.Scripts.Add(meansComparison);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "OneWayANOVAPowerAnalysisUserBasedInputs"))
                {
                    Script meansComparison = new Script() { ScriptDisplayName = "'One-way ANOVA' Power Analysis (User Based Inputs)", ScriptFileName = "OneWayANOVAPowerAnalysisUserBasedInputs", RequiresDataset = false };
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
