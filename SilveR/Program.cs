using ElectronNET.API;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace SilveR
{
    public static class Program
    {
        public static string AppName { get; private set; } = "InVivoStat";
        public static string AppVersion { get; private set; } = "v" + Assembly.GetEntryAssembly().GetName().Version.Major + "." + Assembly.GetEntryAssembly().GetName().Version.Minor + "." + Assembly.GetEntryAssembly().GetName().Version.Build;
        public static CultureInfo UserCulture { get; private set; } = CultureInfo.CurrentCulture;

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(Path.GetTempPath(), AppName + "-" + DateTime.Now.ToString("yyyyMMddHHmmss")) + ".log")
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                CreateWebHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.Information("Stopping web host");
                Log.CloseAndFlush();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
              .UseElectron(args)
                .UseStartup<Startup>()
                    .UseSerilog();
    }
}