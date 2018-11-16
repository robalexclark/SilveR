using Microsoft.AspNetCore.Mvc;
using SilveR.Models;
using SilveR.Services;
using SilveR.StatsModels;
using SilveR.ViewModels;
using SilveR.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SilveR.Controllers
{
    public class AnalysesController : Controller
    {
        private readonly SilveRRepository repository;
        private readonly IBackgroundTaskQueue backgroundQueue;
        private readonly IRProcessorService rProcessorService;

        public AnalysesController(SilveRRepository repository, IBackgroundTaskQueue backgroundQueue, IRProcessorService rProcessorService)
        {
            this.repository = repository;
            this.backgroundQueue = backgroundQueue;
            this.rProcessorService = rProcessorService;
        }

        [HttpGet]
        public async Task<ActionResult> Index()
        {
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
            IList<Analysis> analyses = await repository.GetAnalyses();
            IEnumerable<AnalysisViewModel> analysesViewModel = analyses.Select(x => new AnalysisViewModel(x));

            return Json(analysesViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> AnalysisDataSelector(string analysisType)
        {
            IList<DatasetViewModel> theDatasets = await repository.GetDatasetViewModels();
            ViewBag.HasDatasets = theDatasets.Any();

            AnalysisDataSelectorViewModel viewModel = new AnalysisDataSelectorViewModel();
            if (theDatasets.Any())
            {
                //get the scripts
                ViewBag.ScriptList = await repository.GetScriptNames();

                //add the last uploaded dataset in first
                List<DatasetViewModel> datasetInfoList = new List<DatasetViewModel>();
                DatasetViewModel lastUploadedDataset = theDatasets.OrderByDescending(x => x.DatasetID).First();
                datasetInfoList.Add(lastUploadedDataset);

                //then add all the other datasets in alphabetical order
                foreach (DatasetViewModel dvm in theDatasets.Where(x => x.DatasetID != lastUploadedDataset.DatasetID).OrderBy(x => x.DatasetName).ThenBy(x => x.VersionNo))
                {
                    datasetInfoList.Add(dvm);
                }

                ViewBag.Datasets = datasetInfoList.AsEnumerable<DatasetViewModel>();


                viewModel.AnalysisType = analysisType;
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
            Dataset dataset = await repository.GetDatasetByID(viewModel.DatasetID);

            AnalysisModelBase analysisModel = AnalysisFactory.CreateAnalysisModel(viewModel.AnalysisType, dataset);

            //the method name is the same as the display name after spaces and hyphens removed
            string analysisViewName = viewModel.AnalysisType.Replace(" ", String.Empty).Replace("-", String.Empty);
            return View(analysisViewName, analysisModel);
        }

        [HttpGet]
        public ActionResult SummaryStatistics() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult SingleMeasuresParametricAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult RepeatedMeasuresParametricAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult PValueAdjustment() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult PairedTTestAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult UnpairedTTestAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult TwoSampleTTestAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult CorrelationAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult LinearRegressionAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult DoseResponseAndNonLinearRegressionAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult NonParametricAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult ChiSquaredAndFishersExactTest() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult SurvivalAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult GraphicalAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult MeansComparison() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult MultivariateAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult NestedDesignAnalysis() { return RedirectToAction("Index"); }
        [HttpGet]
        public ActionResult IncompleteFactorialParametricAnalysis() { return RedirectToAction("Index"); }


        [HttpPost]
        public async Task<IActionResult> SummaryStatistics(SummaryStatisticsModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> SingleMeasuresParametricAnalysis(SingleMeasuresParametricAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> RepeatedMeasuresParametricAnalysis(RepeatedMeasuresParametricAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> PValueAdjustment(PValueAdjustmentModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> PairedTTestAnalysis(PairedTTestAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> UnpairedTTestAnalysis(UnpairedTTestAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> TwoSampleTTestAnalysis(TwoSampleTTestAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> CorrelationAnalysis(CorrelationAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> LinearRegressionAnalysis(LinearRegressionAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> DoseResponseAndNonLinearRegressionAnalysis(DoseResponseAndNonLinearRegesssionAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> NonParametricAnalysis(NonParametricAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> ChiSquaredAndFishersExactTest(ChiSquaredAndFishersExactTestModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> SurvivalAnalysis(SurvivalAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> GraphicalAnalysis(GraphicalAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> MeansComparison(MeansComparisonModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        //[HttpPost]
        //public async Task<IActionResult> MultivariateAnalysis(MultivariateModel model, string submitButton)
        //{
        //    return await RunAnalysis(model, submitButton);
        //}

        [HttpPost]
        public async Task<IActionResult> NestedDesignAnalysis(NestedDesignAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        [HttpPost]
        public async Task<IActionResult> IncompleteFactorialParametricAnalysis(IncompleteFactorialParametricAnalysisModel model, string submitButton)
        {
            return await RunAnalysis(model, submitButton);
        }

        private async Task<IActionResult> RunAnalysis(AnalysisModelBase model, string submitButton)
        {
            Dataset dataset = await repository.GetDatasetByID(model.DatasetID);
            model.ReInitialize(dataset);

            if (ModelState.IsValid)
            {
                ValidationInfo validationInfo = model.Validate();
                ViewBag.HasError = !validationInfo.ValidatedOK;

                if (!validationInfo.ValidatedOK)
                {
                    foreach (string error in validationInfo.ErrorMessages)
                    {
                        ModelState.AddModelError(String.Empty, error);
                    }

                    return View(model);
                }
                else if (submitButton != "SubmitAnyway" && validationInfo.WarningMessages.Any())
                {
                    ViewBag.WarningMessages = validationInfo.WarningMessages;
                    ViewBag.HasWarnings = true;

                    return View(model);
                }
                else //run analysis...
                {
                    string analysisGuid = Guid.NewGuid().ToString();

                    //save settings to database
                    Analysis newAnalysis = new Analysis();
                    newAnalysis.AnalysisGuid = analysisGuid;

                    if (dataset != null)
                    {
                        newAnalysis.DatasetID = dataset.DatasetID;
                        newAnalysis.DatasetName = dataset.DatasetName;
                    }

                    newAnalysis.Script = await repository.GetScriptByName(model.ScriptFileName);
                    newAnalysis.DateAnalysed = DateTime.Now;

                    //Arguments
                    foreach (Argument argument in model.GetArguments())
                    {
                        newAnalysis.Arguments.Add(argument);
                    }

                    await repository.SaveAnalysis(newAnalysis);

                    backgroundQueue.QueueBackgroundWorkItem(async cancellationToken =>
                    {
                        try
                        {
                            await rProcessorService.Execute(analysisGuid);
                        }
                        catch (Exception ex)
                        {
                            string s = ex.Message;
                        }
                    });

                    //eventually work out how to return message that analysis complete, but for now...
                    return RedirectToAction("Processing", new { analysisGuid = analysisGuid });
                }
            }
            else
            {
                ViewBag.HasError = true;
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> HasFinished(string analysisGuid)
        {
            bool hasFinished = await repository.HasAnalysisCompleted(analysisGuid);
            return Json(hasFinished);
        }

        [HttpGet]
        public async Task<IActionResult> ReAnalyse(string analysisGuid)
        {
            Analysis analysis = await repository.GetAnalysisComplete(analysisGuid);

            if (analysis.DatasetID == null && analysis.Script.ScriptDisplayName != "P-value Adjustment") //then dataset has been deleted
            {
                TempData["ErrorMessage"] = "It is not possible to re-analyse this dataset as it has been deleted";
                return RedirectToAction("Index");
            }
            else
            {
                AnalysisModelBase model = AnalysisFactory.CreateAnalysisModel(analysis.Script.ScriptDisplayName, analysis.Dataset);

                model.LoadArguments(analysis.Arguments);

                string analysisLink = analysis.Script.ScriptDisplayName.Replace(" ", String.Empty).Replace("-", String.Empty);

                return View(analysisLink, model);
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

        [HttpPost]
        public ActionResult ViewResults(string analysisGuid, string submitButton)
        {
            if (submitButton == "ReAnalyse")
            {
                return RedirectToAction("ReAnalyse", new { analysisGuid = analysisGuid });
            }
            else if (submitButton == "ViewLog")
            {
                return RedirectToAction("ViewLog", new { analysisGuid = analysisGuid });
            }
            else throw new Exception();
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

            return View();
        }

        [HttpDelete]
        public async Task<IActionResult> Destroy(Analysis analysis)
        {
            await repository.DeleteAnalysis(analysis);

            return RedirectToAction("Index");
        }
    }
}