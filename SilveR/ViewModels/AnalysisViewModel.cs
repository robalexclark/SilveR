using SilveR.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace SilveR.ViewModels
{
    public class AnalysisViewModel
    {
        public AnalysisViewModel()
        {
        }

        public AnalysisViewModel(Analysis analysis)
        {
            AnalysisID = analysis.AnalysisID;
            AnalysisGuid = analysis.AnalysisGuid;
            DatasetID = analysis.DatasetID;
            DatasetName = analysis.DatasetName;
            ScriptID = analysis.ScriptID;
            ScriptDisplayName = analysis.Script.ScriptDisplayName;
            Tag = analysis.Tag;
            RProcessOutput = analysis.RProcessOutput;
            HtmlOutput = analysis.HtmlOutput;
            DateAnalysed = analysis.DateAnalysed;
        }

        public int AnalysisID { get; set; }

        [Required]
        [StringLength(128)]
        public string AnalysisGuid { get; set; }

        public int? DatasetID { get; set; }

        [Required]
        [StringLength(50)]
        public string DatasetName { get; set; }

        public int ScriptID { get; set; }

        public string ScriptDisplayName { get; set; }

        [StringLength(200)]
        public string Tag { get; set; }

        public string RProcessOutput { get; set; }

        public string HtmlOutput { get; set; }

        public DateTime DateAnalysed { get; set; }
    }
}