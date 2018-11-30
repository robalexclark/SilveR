using SilveR.Models;
using System;
using System.Collections.Generic;

namespace SilveR.ViewModels
{
    public class AnalysisDataSelectorViewModel
    {
        public string AnalysisName{ get; set; }

        public string AnalysisDisplayName { get; set; }

        public Nullable<int> SelectedDatasetID { get; set; }

        public IEnumerable<Script> Scripts { get; set; }

        public IEnumerable<DatasetViewModel> Datasets { get; set; }
    }
}