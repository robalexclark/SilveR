using Microsoft.AspNetCore.Mvc;
using SilveR.Helpers;
using SilveR.Models;
using SilveR.Services;
using SilveR.StatsModels;
using SilveR.Validators;
using SilveR.ViewModels;
using System;
using System.Collections.Generic;
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

                if (script.RequiresDataset == false)
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
        public async Task<IActionResult> PValueAdjustment(PValueAdjustmentModel model, bool ignoreWarnings)
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
        public async Task<IActionResult> DoseResponseAndNonLinearRegressionAnalysis(DoseResponseAndNonLinearRegesssionAnalysisModel model, bool ignoreWarnings)
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

        //[HttpPost]
        //public async Task<IActionResult> MultivariateAnalysis(MultivariateModel model, bool ignoreWarnings)
        //{
        //    return await RunAnalysis(model, ignoreWarnings);
        //}

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
            else
                throw new ArgumentException("SubmitButton not valid!");
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
        public async Task<JsonResult> Destroy(Analysis analysis)
        {
            await repository.DeleteAnalysis(analysis);

            return Json(true);
        }
    }
}