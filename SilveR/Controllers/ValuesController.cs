using Microsoft.AspNetCore.Mvc;
using SilveR.Helpers;
using SilveR.Models;
using SilveR.StatsModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace SilveR.Controllers
{
    public class ValuesController : Controller
    {
        private readonly ISilveRRepository repository;

        public ValuesController(ISilveRRepository repository)
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
                List<string> levelsList = datatable.GetLevels(treatment).ToList();

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
        public JsonResult GetSMPAInteractions(List<string> selectedTreatments)
        {
            if (selectedTreatments != null)
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
        public JsonResult GetSMPASelectedEffectsList(List<string> selectedTreatments)
        {
            if (selectedTreatments != null)
            {
                List<string> selectedEffectsList = SingleMeasuresParametricAnalysisModel.DetermineSelectedEffectsList(selectedTreatments);
                return Json(selectedEffectsList);
            }
            else
            {
                return Json(new List<string>());
            }
        }


        [HttpGet]
        public JsonResult GetRMPAInteractions(List<string> selectedTreatments)
        {
            if (selectedTreatments != null)
            {
                List<string> interactions = RepeatedMeasuresParametricAnalysisModel.DetermineInteractions(selectedTreatments);
                return Json(interactions);
            }
            else
            {
                return Json(new List<string>());
            }
        }

        [HttpGet]
        public JsonResult GetRMPASelectedEffectsList(List<string> selectedTreatments, string repeatedFactor)
        {
            if (selectedTreatments != null  && repeatedFactor != null)
            {
                List<string> selectedEffectsList = RepeatedMeasuresParametricAnalysisModel.DetermineSelectedEffectsList(selectedTreatments, repeatedFactor);
                return Json(selectedEffectsList);
            }
            else
            {
                return Json(new List<string>());
            }
        }


        [HttpGet]
        public JsonResult GetIncompleteFactorialInteractions(List<string> selectedTreatments)
        {
            if (selectedTreatments != null)
            {
                List<string> interactions = IncompleteFactorialParametricAnalysisModel.DetermineInteractions(selectedTreatments);
                return Json(interactions);
            }
            else
            {
                return Json(new List<string>());
            }
        }

        [HttpGet]
        public JsonResult GetIncompleteFactorialSelectedEffectsList(List<string> selectedTreatments)
        {
            if (selectedTreatments != null)
            {
                List<string> selectedEffectsList = IncompleteFactorialParametricAnalysisModel.DetermineSelectedEffectsList(selectedTreatments);
                return Json(selectedEffectsList);
            }
            else
            {
                return Json(new List<string>());
            }
        }
    }
}