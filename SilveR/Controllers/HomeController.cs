using ElectronNET.API;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SilveR.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SilveR.Controllers
{
    public class HomeController : Controller
    {
        private const string R_VERSION = "3.6.3";

        private readonly AppSettings appSettings;

        public HomeController(IOptions<AppSettings> appSettings)
        {
            this.appSettings = appSettings.Value;
        }

        public IActionResult Index()
        {
            //check R installed
            bool rInstalled = CheckRInstalled();

            if (!rInstalled) //then major error as R is not found
            {
                StringBuilder stringBuilder = new StringBuilder("R " + R_VERSION + " is not installed or is misconfigured." + Environment.NewLine);

                if (!String.IsNullOrEmpty(appSettings.CustomRScriptLocation) && !System.IO.File.Exists(appSettings.CustomRScriptLocation))
                {
                    stringBuilder.AppendLine("R cannot be found at the custom R location (" + appSettings.CustomRScriptLocation + ").");
                }
                else
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    {
                        stringBuilder.AppendLine("R " + R_VERSION + " should have been installed but is missing from the expected location. Please reinstall the application or check your custom location.");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                    {
                        stringBuilder.AppendLine("To install " + R_VERSION + " on linux see the setup instructions at <a href='https://github.com/robalexclark/SilveR#linux'>https://github.com/robalexclark/SilveR#linux</a>");
                    }
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                    {
                        stringBuilder.AppendLine("To install " + R_VERSION + " on mac see the setup instructions at <a href='https://github.com/robalexclark/SilveR#macos>https://github.com/robalexclark/SilveR#macos</a>");
                    }
                }

                ViewBag.ErrorMessage = stringBuilder.ToString();
            }

            return View();
        }

        private bool CheckRInstalled()
        {
            string rscriptPath = null;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!String.IsNullOrEmpty(appSettings.CustomRScriptLocation))
                    rscriptPath = appSettings.CustomRScriptLocation;
                else
                    rscriptPath = Path.Combine(Startup.ContentRootPath, "R", "bin", "Rscript.exe");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                if (!String.IsNullOrEmpty(appSettings.CustomRScriptLocation))
                    rscriptPath = appSettings.CustomRScriptLocation;
                else
                    rscriptPath = "Rscript";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                if (!String.IsNullOrEmpty(appSettings.CustomRScriptLocation))
                    rscriptPath = appSettings.CustomRScriptLocation;
                else
                    rscriptPath = "/usr/local/bin/Rscript";
            }

            ProcessStartInfo processStartInfo = new ProcessStartInfo(rscriptPath);
            processStartInfo.RedirectStandardError = true;
            processStartInfo.Arguments = "--version";
            processStartInfo.UseShellExecute = false;

            try
            {
                Process process = Process.Start(processStartInfo);
                string processOutput = process.StandardError.ReadToEnd();

                return processOutput.Contains(R_VERSION);
            }
            catch
            {
                return false;
            }
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}