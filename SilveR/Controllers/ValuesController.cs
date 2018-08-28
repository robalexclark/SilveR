using Microsoft.AspNetCore.Mvc;
using SilveR.Models;
using SilveRModel.Helpers;
using SilveRModel.Models;
using SilveRModel.StatsModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SilveR.Controllers
{
    public class ValuesController : Controller
    {
        private readonly SilveRRepository repository;

        public ValuesController(SilveRRepository repository)
        {
            this.repository = repository;
        }

        [HttpGet]
        public async Task<JsonResult> GetLevels(string treatment, int datasetID, bool includeNull)
        {
            if (treatment == null)
                return Json(new List<string>());

            Dataset dataset = await repository.GetDatasetByID(datasetID);

            DataTable datatable = dataset.DatasetToDataTable();
            if (datatable.GetVariableNames().Contains(treatment))
            {
                List<string> levelsList = datatable.GetLevels(treatment);

                if (includeNull)
                {
                    levelsList.Insert(0, String.Empty); //add empty item
                }

                return Json(levelsList);
            }
            else
            {
                return Json(new List<string>());
            }
        }

        [HttpGet]
        public JsonResult GetSelectedTreatments(List<string> selectedTreatments)
        {
            if (selectedTreatments != null && selectedTreatments.Any())
            {
                selectedTreatments.Insert(0, String.Empty);
                return Json(selectedTreatments);
            }
            else
            {
                return Json(selectedTreatments);
            }
        }

        [HttpGet]
        public JsonResult GetInteractions(List<string> selectedTreatments)
        {
            if (selectedTreatments != null && selectedTreatments.Any())
            {
                List<string> interactions = SingleMeasuresParametricAnalysisModel.DetermineInteractions(selectedTreatments);
                return Json(interactions);
            }
            else
            {
                return Json(new List<string>());
            }
        }

        [HttpGet]
        public JsonResult GetSelectedEffectsList(List<string> selectedTreatments)
        {
            if (selectedTreatments != null && selectedTreatments.Any())
            {
                List<string> selectedEffectsList = SingleMeasuresParametricAnalysisModel.DetermineSelectedEffectsList(selectedTreatments);
                return Json(selectedEffectsList);
            }
            else
            {
                return Json(new List<string>());
            }
        }
    }
}