using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using SilveR.Helpers;
using SilveR.Models;
using SilveR.Services;
using SilveR.StatsModels;
using SilveR.Validators;
using SilveR.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace SilveR.Controllers
{
    public class AnalysesController : Controller
    {
        private readonly ISilveRRepository repository;
        private readonly IBackgroundTaskQueue backgroundQueue;
        private readonly IRProcessorService rProcessorService;

        public AnalysesController(ISilveRRepository repository, IBackgroundTaskQueue backgroundQueue, IRProcessorService rProcessorService)
        {
            this.repository = repository;
            this.backgroundQueue = backgroundQueue;
            this.rProcessorService = rProcessorService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
            //check if any analyses already in db
            ViewBag.HasAnalyses = await repository.HasAnalyses();

            if (TempData.ContainsKey("ErrorMessage"))
            {
                ViewBag.ErrorMessage = TempData["ErrorMessage"];
            }

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetAnalyses()
        {
            IEnumerable<Analysis> analyses = await repository.GetAnalyses();
            IEnumerable<AnalysisViewModel> analysesViewModel = analyses.Select(x => new AnalysisViewModel(x));

            return Json(analysesViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AnalysisDataSelector(string analysisName)
        {
            IEnumerable<Script> scripts = await repository.GetScripts();

            AnalysisDataSelectorViewModel viewModel = new AnalysisDataSelectorViewModel();

            if (analysisName != null)
            {
                viewModel.AnalysisName = analysisName;

                Script script = scripts.Single(x => x.ScriptFileName == analysisName);

                if (!script.RequiresDataset)
                {
                    return RedirectToAction("Analysis", viewModel);
                }

                viewModel.AnalysisDisplayName = script.ScriptDisplayName;
            }

            IEnumerable<DatasetViewModel> theDatasets = await repository.GetDatasetViewModels();
            ViewBag.HasDatasets = theDatasets.Any();

            if (theDatasets.Any())
            {
                //get the scripts
                viewModel.Scripts = scripts;

                //add the last uploaded dataset in first
                List<DatasetViewModel> datasetInfoList = new List<DatasetViewModel>();
                DatasetViewModel lastUploadedDataset = theDatasets.OrderByDescending(x => x.DatasetID).First();
                datasetInfoList.Add(lastUploadedDataset);

                //then add all the other datasets in alphabetical order
                foreach (DatasetViewModel dvm in theDatasets.Where(x => x.DatasetID != lastUploadedDataset.DatasetID).OrderBy(x => x.DatasetName).ThenBy(x => x.VersionNo))
                {
                    datasetInfoList.Add(dvm);
                }

                viewModel.Datasets = datasetInfoList.AsEnumerable<DatasetViewModel>();
            }

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AnalysisDataSelector(AnalysisDataSelectorViewModel viewModel)
        {
            return RedirectToAction("Analysis", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Analysis(AnalysisDataSelectorViewModel viewModel)
        {
            Dataset dataset = null;
            if (viewModel.SelectedDatasetID.HasValue)
            {
                dataset = await repository.GetDatasetByID(viewModel.SelectedDatasetID.Value);
            }

            AnalysisModelBase analysisModel = AnalysisFactory.CreateAnalysisModel(viewModel.AnalysisName, dataset);

            //the view name is the same as the display name after spaces and hyphens removed (should be same as scriptfilename)
            return View(viewModel.AnalysisName, analysisModel);
        }


        [HttpPost]
        public async Task<IActionResult> SummaryStatistics(SummaryStatisticsModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> SingleMeasuresParametricAnalysis(SingleMeasuresParametricAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> RepeatedMeasuresParametricAnalysis(RepeatedMeasuresParametricAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> PValueAdjustmentUserBasedInputs(PValueAdjustmentUserBasedInputsModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> PValueAdjustmentDatasetBasedInputs(PValueAdjustmentDatasetBasedInputsModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> PairedTTestAnalysis(PairedTTestAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> UnpairedTTestAnalysis(UnpairedTTestAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> OneSampleTTestAnalysis(OneSampleTTestAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> CorrelationAnalysis(CorrelationAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> LinearRegressionAnalysis(LinearRegressionAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> DoseResponseAndNonLinearRegressionAnalysis(DoseResponseAndNonLinearRegressionAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> NonParametricAnalysis(NonParametricAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> ChiSquaredAndFishersExactTest(ChiSquaredAndFishersExactTestModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> SurvivalAnalysis(SurvivalAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> GraphicalAnalysis(GraphicalAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> ComparisonOfMeansPowerAnalysisDatasetBasedInputs(ComparisonOfMeansPowerAnalysisDatasetBasedInputsModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> ComparisonOfMeansPowerAnalysisUserBasedInputs(ComparisonOfMeansPowerAnalysisUserBasedInputsModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> OneWayANOVAPowerAnalysisDatasetBasedInputs(OneWayANOVAPowerAnalysisDatasetBasedInputsModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> OneWayANOVAPowerAnalysisUserBasedInputs(OneWayANOVAPowerAnalysisUserBasedInputsModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> MultivariateAnalysis(MultivariateAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> NestedDesignAnalysis(NestedDesignAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        [HttpPost]
        public async Task<IActionResult> IncompleteFactorialParametricAnalysis(IncompleteFactorialParametricAnalysisModel model, bool ignoreWarnings)
        {
            return await RunAnalysis(model, ignoreWarnings);
        }

        private async Task<IActionResult> RunAnalysis(AnalysisModelBase model, bool ignoreWarnings)
        {
            Dataset dataset = null;
            AnalysisDataModelBase analysisDataModelBase = model as AnalysisDataModelBase;
            if (analysisDataModelBase != null)
            {
                dataset = await repository.GetDatasetByID(analysisDataModelBase.DatasetID);
                analysisDataModelBase.ReInitialize(dataset);
            }

            if (ModelState.IsValid)
            {
                ValidationInfo validationInfo = model.Validate();

                if (!validationInfo.ValidatedOK)
                {
                    foreach (string error in validationInfo.ErrorMessages)
                    {
                        ModelState.AddModelError(String.Empty, error);
                    }

                    return View(model);
                }
                else if (validationInfo.WarningMessages.Any() && !ignoreWarnings) //display warnings
                {
                    ViewBag.WarningMessages = validationInfo.WarningMessages;

                    return View(model);
                }
                else //run analysis...
                {
                    //save settings to database
                    Analysis newAnalysis = new Analysis(dataset);
                    newAnalysis.Script = await repository.GetScriptByName(model.ScriptFileName);
                    newAnalysis.DateAnalysed = DateTime.Now;
                    newAnalysis.Arguments.AddRange(model.GetArguments());

                    await repository.AddAnalysis(newAnalysis);

                    backgroundQueue.QueueBackgroundWorkItem(async cancellationToken =>
                    {
                        await rProcessorService.Execute(newAnalysis.AnalysisGuid);
                    });

                    //eventually work out how to return message that analysis complete, but for now...
                    return RedirectToAction("Processing", new { analysisGuid = newAnalysis.AnalysisGuid });
                }
            }
            else
            {
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ReAnalyse(string analysisGuid)
        {
            Analysis analysis = await repository.GetAnalysisComplete(analysisGuid);

            if (analysis.Dataset != null || !analysis.Script.RequiresDataset)
            {
                AnalysisModelBase model = AnalysisFactory.CreateAnalysisModel(analysis.Script.ScriptFileName, analysis.Dataset);
                model.LoadArguments(analysis.Arguments);

                return View(analysis.Script.ScriptFileName, model);
            }
            else //then dataset has been deleted?
            {
                TempData["ErrorMessage"] = "It is not possible to re-analyse this dataset as it has been deleted";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ViewResults(string analysisGuid)
        {
            Analysis analysis = await repository.GetAnalysis(analysisGuid);

            if (analysis.HtmlOutput != null)
            {
                return View(analysis);
            }
            else
            {
                TempData["ErrorMessage"] = "No results output was found for that analysis. Check the Analysis Log (below) for more details.";
                return RedirectToAction("ViewLog", new { analysisGuid = analysis.AnalysisGuid });
            }
        }

        [HttpGet]
        public FileContentResult ExportToPdf(string analysisGuid)
        {
            //generate pdf using static method (but should really be a service?)
            byte[] bytes = PdfGenerator.GeneratePdf(new Uri($"{Request.Scheme}://{Request.Host.Value}/Analyses/ResultsForExport?analysisGuid=" + analysisGuid));

            Response.Headers.Add("Content-Disposition", "inline; filename=" + analysisGuid + ".pdf");
            return File(bytes, "application/pdf");
        }

        [HttpGet]
        public async Task<IActionResult> ResultsForExport(string analysisGuid)
        {
            Analysis analysis = await repository.GetAnalysis(analysisGuid);
            return View(analysis);
        }

        [HttpGet]
        public async Task<ActionResult> ExportImages(string analysisGuid)
        {
            Analysis analysis = await repository.GetAnalysis(analysisGuid);
            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(analysis.HtmlOutput);

            HtmlNodeCollection nodes = document.DocumentNode.SelectNodes("//img");

            byte[] zippedBytes = new byte[] { 0x20, 0x20, 0x20, 0x20, 0x20, 0x20, 0x20 }; //
            if (nodes != null)
            {
                Dictionary<string, byte[]> files = new Dictionary<string, byte[]>();

                foreach (var node in nodes)
                {
                    string base64StringEncodedImage = node.Attributes["src"].Value.Remove(0, "data:image/png;base64,".Length);
                    files.Add(Guid.NewGuid().ToString() + ".png", Convert.FromBase64String(base64StringEncodedImage));
                }

                zippedBytes = GetZipArchive(files);
            }

            Response.Headers.Add("Content-Disposition", "inline; filename=" + analysisGuid + ".zip");
            return File(zippedBytes, "application/zip");
        }

        private byte[] GetZipArchive(Dictionary<string, byte[]> files)
        {
            byte[] archiveFile;
            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    foreach (var file in files)
                    {
                        var zipArchiveEntry = archive.CreateEntry(file.Key, CompressionLevel.Fastest);
                        using (var zipStream = zipArchiveEntry.Open())
                            zipStream.Write(file.Value, 0, file.Value.Length);
                    }
                }

                archiveFile = archiveStream.ToArray();
            }

            return archiveFile;
        }

        [HttpGet]
        public ActionResult Processing(string analysisGuid)
        {
            ViewBag.AnalysisGuid = analysisGuid;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AnalysisCompleted(string analysisGuid)
        {
            bool finished = await repository.HasAnalysisCompleted(analysisGuid);
            return Json(finished);
        }

        [HttpGet]
        public async Task<IActionResult> ViewLog(string analysisGuid)
        {
            Analysis analysis = await repository.GetAnalysis(analysisGuid);

            ViewBag.ErrorMessage = TempData["ErrorMessage"];
            ViewBag.AnalysisLog = analysis.RProcessOutput;
            ViewBag.AnalysisGuid = analysisGuid;

            return View();
        }

        [HttpDelete]
        public async Task<JsonResult> Destroy(int analysisID)
        {
            await repository.DeleteAnalysis(analysisID);
            return Json(true);
        }

        [HttpPost]
        public async Task<JsonResult> DeleteSelected(IEnumerable<int> analysisIds)
        {
            await repository.DeleteAnalyses(analysisIds);
            return Json(true);
        }
    }
}