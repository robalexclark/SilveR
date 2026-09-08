using ElectronNET.API;
using ElectronNET.API.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SilveR.Models;
using SilveR.Services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

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
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            AppSettings settings = configuration.GetSection(nameof(AppSettings)).Get<AppSettings>();
            string customDbLocation = settings.CustomDbLocation;

            string appDataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Program.AppName); //default db location
            if (!String.IsNullOrWhiteSpace(customDbLocation))
            {
                appDataFolder = customDbLocation;
            }

            Directory.CreateDirectory(appDataFolder); //create the app folder if it does not exist
            services.AddDbContext<SilveRContext>(options => options.UseSqlite("Data Source=" + Path.Combine(appDataFolder, Program.AppName + ".db")));
            services.AddScoped<ISilveRRepository, SilveRRepository>();

            //R processing services comprising of R processor and queue services
            services.AddSingleton<IRProcessorService, RProcessorService>();
            services.AddHostedService<QueuedHostedService>();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();

            services.Configure<AppSettings>(configuration.GetSection("AppSettings"));

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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

            //MenuItem[] contextMenu = new MenuItem[] {
            //    new MenuItem { Label = "Copy", Accelerator = "CmdOrCtrl+C", Role = MenuRole.copy }
            //};

            //Electron.Menu.SetContextMenu(browserWindow, contextMenu);

            browserWindow.OnReadyToShow += () =>
            {
                browserWindow.Show();
                //browserWindow.Maximize();
            };
        }


        private static void EnsureMigrationHistoryTable(SilveRContext context)
        {
            var connection = context.Database.GetDbConnection();
            bool openedHere = connection.State != ConnectionState.Open;

            if (openedHere)
            {
                connection.Open();
            }

            try
            {
                bool datasetsTableExists;
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='Datasets';";
                    datasetsTableExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                }

                if (!datasetsTableExists)
                {
                    return;
                }

                bool migrationsTableExists;
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='__EFMigrationsHistory';";
                    migrationsTableExists = Convert.ToInt32(command.ExecuteScalar()) > 0;
                }

                if (!migrationsTableExists)
                {
                    context.Database.ExecuteSqlRaw("CREATE TABLE IF NOT EXISTS \"__EFMigrationsHistory\" (\"MigrationId\" TEXT NOT NULL CONSTRAINT \"PK___EFMigrationsHistory\" PRIMARY KEY, \"ProductVersion\" TEXT NOT NULL);");
                    context.Database.ExecuteSqlRaw("INSERT OR IGNORE INTO \"__EFMigrationsHistory\" (\"MigrationId\", \"ProductVersion\") VALUES ('20210908204311_InitialCreate', '5.0.9');");
                }
            }
            finally
            {
                if (openedHere && connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private void ProvisionDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {
                SilveRContext context = serviceScope.ServiceProvider.GetRequiredService<SilveRContext>();

                EnsureMigrationHistoryTable(context);

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

                (string ScriptDisplayName, string ScriptFileName, bool RequiresDataset)[] scriptDefinitions =
                {
                    ("Summary Statistics", "SummaryStatistics", true),
                    ("Single Measures Parametric Analysis", "SingleMeasuresParametricAnalysis", true),
                    ("Repeated Measures Parametric Analysis", "RepeatedMeasuresParametricAnalysis", true),
                    ("P-value Adjustment (User Based Inputs)", "PValueAdjustmentUserBasedInputs", false),
                    ("P-value Adjustment (Dataset Based Inputs)", "PValueAdjustmentDatasetBasedInputs", true),
                    ("Extended Paired t-test Analysis", "PairedTTestAnalysis", true),
                    ("Unpaired t-test Analysis", "UnpairedTTestAnalysis", true),
                    ("One-sample t-test Analysis", "OneSampleTTestAnalysis", true),
                    ("Correlation Analysis", "CorrelationAnalysis", true),
                    ("Linear Regression Analysis", "LinearRegressionAnalysis", true),
                    ("Logistic Regression Analysis", "LogisticRegressionAnalysis", true),
                    ("Dose-response and Non-linear Regression Analysis", "DoseResponseAndNonLinearRegressionAnalysis", true),
                    ("Non-parametric Analysis", "NonParametricAnalysis", true),
                    ("Chi-squared and Fisher's Exact Test", "ChiSquaredAndFishersExactTest", true),
                    ("Survival Analysis", "SurvivalAnalysis", true),
                    ("Graphical Analysis", "GraphicalAnalysis", true),
                    ("'Comparison of Means' Power Analysis (Dataset Based Inputs)", "ComparisonOfMeansPowerAnalysisDatasetBasedInputs", true),
                    ("'Comparison of Means' Power Analysis (User Based Inputs)", "ComparisonOfMeansPowerAnalysisUserBasedInputs", false),
                    ("'Equivalence of Means' Power Analysis (Dataset Based Inputs)", "EquivalenceOfMeansPowerAnalysisDatasetBasedInputs", true),
                    ("'Equivalence of Means' Power Analysis (User Based Inputs)", "EquivalenceOfMeansPowerAnalysisUserBasedInputs", false),
                    ("'One-way ANOVA' Power Analysis (Dataset Based Inputs)", "OneWayANOVAPowerAnalysisDatasetBasedInputs", true),
                    ("'One-way ANOVA' Power Analysis (User Based Inputs)", "OneWayANOVAPowerAnalysisUserBasedInputs", false),
                    ("Multivariate Analysis", "MultivariateAnalysis", true),
                    ("Hasse Diagram Generator", "HasseDiagramGenerator", true),
                    ("Nested Design Analysis", "NestedDesignAnalysis", true),
                    ("Incomplete Factorial Parametric Analysis", "IncompleteFactorialParametricAnalysis", true),
                    ("Single Measures to Repeated Measures Data Transformation", "SingleMeasuresToRepeatedMeasuresDataTransformation", true),
                    ("Area Under Curve Data Transformation", "AreaUnderCurveDataTransformation", true),
                    ("Equivalence TOST Test", "EquivalenceTOSTTest", true),
                    ("R-Runner", "RRunner", true)
                };

                foreach ((string scriptDisplayName, string scriptFileName, bool requiresDataset) in scriptDefinitions)
                {
                    if (scriptFileName == "HasseDiagramGenerator")
                    {
                        Script existingHasseDiagramGenerator = existingScripts.SingleOrDefault(x => x.ScriptFileName == "HasseDiagramGenerator" || x.ScriptFileName == "HasseDiagramsGenerator");
                        if (existingHasseDiagramGenerator == null)
                        {
                            context.Scripts.Add(new Script
                            {
                                ScriptDisplayName = scriptDisplayName,
                                ScriptFileName = scriptFileName,
                                RequiresDataset = requiresDataset
                            });
                        }
                        else
                        {
                            existingHasseDiagramGenerator.ScriptDisplayName = scriptDisplayName;
                            existingHasseDiagramGenerator.ScriptFileName = scriptFileName;
                            existingHasseDiagramGenerator.RequiresDataset = requiresDataset;
                        }

                        continue;
                    }

                    if (!existingScripts.Any(x => x.ScriptFileName == scriptFileName))
                    {
                        context.Scripts.Add(new Script
                        {
                            ScriptDisplayName = scriptDisplayName,
                            ScriptFileName = scriptFileName,
                            RequiresDataset = requiresDataset
                        });
                    }
                }

                context.SaveChanges();
            }
        }
    }

    public class AppSettings
    {
        public string CustomDbLocation { get; set; }
        public string CustomRScriptLocation { get; set; }
    }
}
