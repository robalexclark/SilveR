using Microsoft.Extensions.DependencyInjection;
using SilveR.Models;
using SilveRModel.Helpers;
using SilveRModel.Models;
using SilveRModel.StatsModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SilveR.Services
{
    public interface IRProcessorService
    {
        Task Execute(string analysisGuid);
    }

    public class RProcessorService : IRProcessorService
    {
        private readonly IServiceProvider services;

        public RProcessorService(IServiceProvider services)
        {
            this.services = services;
        }

        public async Task Execute(string analysisGuid)
        {
            using (IServiceScope scope = services.CreateScope())
            {
                SilveRRepository repository = scope.ServiceProvider.GetRequiredService<SilveRRepository>();

#if !DEBUG
                try
                {
#endif
                Stopwatch sw = Stopwatch.StartNew();

                string workingDir = Path.GetTempPath();

                //get analysis
                Analysis analysis = await repository.GetAnalysisComplete(analysisGuid);

                //combine script files into analysisGuid.R
                string scriptFileName = Path.Combine(workingDir, analysisGuid + ".R");
                string[] commonFunctionsContents = File.ReadAllLines(Path.Combine(Startup.ContentRootPath, "Scripts", "Common_Functions.R"));
                string[] mainScriptContents = File.ReadAllLines(Path.Combine(Startup.ContentRootPath, "Scripts", analysis.Script.ScriptFileName + ".R"));

                File.WriteAllLines(scriptFileName, commonFunctionsContents);
                File.AppendAllLines(scriptFileName, mainScriptContents);

                //load the analysis entity into the model so that arguments can be extracted
                IAnalysisModel analysisModel = AnalysisFactory.CreateAnalysisModel(analysis);
                analysisModel.LoadArguments(analysis.Arguments);

                string csvFileName = null;
                string[] csvData = analysisModel.ExportData();
                if (csvData != null)
                {
                    csvFileName = Path.Combine(workingDir, analysisGuid + ".csv");
                    Dictionary<string, string> charConverterLines = ArgumentConverters.GetCharConversionList();

                    //go through and replace the first line...
                    foreach (KeyValuePair<string, string> kp in charConverterLines)
                    {
                        csvData[0] = csvData[0].Replace(kp.Key, kp.Value);
                    }

                    File.WriteAllLines(csvFileName, csvData);
                }

                //setup the r process (way of calling rscript.exe is slightly different for each OS)
                string rscriptPath = null;
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    rscriptPath = Path.Combine(Startup.ContentRootPath, "R-3.5.1", "bin", "Rscript.exe");
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    rscriptPath = "Rscript";
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    rscriptPath = "Rscript";
                }

                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = rscriptPath;
                psi.WorkingDirectory = workingDir;

                string theArguments = analysisModel.GetCommandLineArguments();
                psi.Arguments = scriptFileName.WrapInDoubleQuotes() + " --vanilla --args " + csvFileName.WrapInDoubleQuotes() + theArguments;

                //Configure some options for the R process
                psi.UseShellExecute = false;
                psi.CreateNoWindow = true;
                psi.RedirectStandardOutput = true;
                psi.RedirectStandardError = true;

                //start rscript.exe
                Process R = Process.Start(psi);

                //create a stringbuilder to hold the output, and get the output line by line until R process finishes
                StringBuilder output = new StringBuilder();

                bool completedOK = R.WaitForExit(60 * 1000);

                //if R didn't complete in time then add message and kill it
                if (!completedOK)
                {
                    output.AppendLine("WARNING! The R process timed out before the script could complete");
                    output.AppendLine();

                    R.Kill();
                }

                //need to make sure that we have got all the output so do a readtoend here
                output.AppendLine(R.StandardOutput.ReadToEnd());
                output.AppendLine();

                //output the errors from R
                string errorsFromR = R.StandardError.ReadToEnd().Trim();

                R.Close();
                R.Dispose();

                if (!String.IsNullOrEmpty(errorsFromR))
                {
                    output.AppendLine();
                    output.Append(errorsFromR);
                    output.AppendLine();
                }

                TimeSpan timeElapsed = sw.Elapsed;
                output.AppendLine();
                output.AppendLine("Analysis by the R Processor took " + Math.Round(timeElapsed.TotalSeconds, 2) + " seconds.");

                analysis.RProcessOutput = output.ToString().Trim();

                //assemble the entire path and file to the html output
                string htmlFile = Path.Combine(workingDir, analysisGuid + ".html");

                if (File.Exists(htmlFile)) //wont exist if there is an error!
                {
                    //get the output files
                    List<string> resultsFiles = new List<string>();
                    string[] outputFiles = Directory.GetFiles(workingDir, analysis.AnalysisGuid + "*");
                    foreach (string file in outputFiles)
                    {
                        if (file.EndsWith(".R") || file.EndsWith(".csv")) continue;

                        resultsFiles.Add(file);
                    }

                    string inlineHtml = InlineHtmlCreator.CreateInlineHtml(resultsFiles);

                    analysis.HtmlOutput = inlineHtml;
                }

                await repository.SaveChangesAsync();

#if !DEBUG
                    string[] filesToClean = Directory.GetFiles(workingDir, analysis.AnalysisGuid + "*");
                    foreach (string file in filesToClean)
                        File.Delete(file);
                }
                catch (Exception ex)
                {
                    try
                    {
                        Analysis analysis = await repository.GetAnalysis(analysisGuid);

                        string message = "ContentRoot=" + Startup.ContentRootPath + Environment.NewLine + ex.Message;
                        if (ex.InnerException != null)
                        {
                            message = message + Environment.NewLine + "Inner Exception: " + ex.InnerException.Message;
                        }

                        analysis.RProcessOutput = message;
                        await repository.SaveChangesAsync();
                    }
                    catch { }

                    throw ex;
                }
#endif
            }
        }
    }

    public static class StringExtensions
    {
        public static string WrapInDoubleQuotes(this string str)
        {
            return "\"" + str + "\"";
        }
    }
}