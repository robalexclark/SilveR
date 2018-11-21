using System;
using System.Collections.Generic;

namespace SilveR.ViewModels
{
    public class AnalysisDataSelectorViewModel
    {
        public string AnalysisType { get; set; }

        public Nullable<int> SelectedDatasetID { get; set; }

        public IEnumerable<string> Scripts { get; set; }

        public IEnumerable<DatasetViewModel> Datasets { get; set; }
    }
}