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

                if (!existingScripts.Any(x => x.ScriptFileName == "SummaryStats"))
                {
                    Script summaryStatistics = new Script() { ScriptDisplayName = "Summary Statistics", ScriptFileName = "SummaryStats" };
                    context.Scripts.Add(summaryStatistics);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "NonParametric"))
                {
                    Script nonParametrics = new Script() { ScriptDisplayName = "Non-Parametric Analysis", ScriptFileName = "NonParametric" };
                    context.Scripts.Add(nonParametrics);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "PValueAdjustment"))
                {
                    Script pValueAdjustment = new Script() { ScriptDisplayName = "P-value Adjustment", ScriptFileName = "PValueAdjustment" };
                    context.Scripts.Add(pValueAdjustment);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "SingleMeasureGLM"))
                {
                    Script singleMeasureGLM = new Script() { ScriptDisplayName = "Single Measures Parametric Analysis", ScriptFileName = "SingleMeasureGLM" };
                    context.Scripts.Add(singleMeasureGLM);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "UnpairedTTest"))
                {
                    Script twoSampleTTest = new Script() { ScriptDisplayName = "Two-Sample t-Test Analysis", ScriptFileName = "TwoSampleTTest" };
                    context.Scripts.Add(twoSampleTTest);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "RepeatedMeasures"))
                {
                    Script repeatedMeasures = new Script() { ScriptDisplayName = "Repeated Measures Parametric Analysis", ScriptFileName = "RepeatedMeasures" };
                    context.Scripts.Add(repeatedMeasures);
                }

                if (!existingScripts.Any(x => x.ScriptFileName == "PairedTTest"))
                {
                    Script pairedTTest = new Script() { ScriptDisplayName = "Paired t-test Analysis", ScriptFileName = "PairedTTest" };
                    context.Scripts.Add(pairedTTest);
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

                context.SaveChanges();
            }
        }
    }

    public class AppSettings
    {
        public string CustomRScriptLocation { get; set; }
    }
}