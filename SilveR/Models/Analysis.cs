using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SilveR.Models
{
    public partial class Analysis
    {
        public Analysis()
        {
            Arguments = new HashSet<Argument>();
        }

        [Key]
        public int AnalysisID { get; set; }

        [Required]
        [StringLength(128)]
        public string AnalysisGuid { get; set; }

        public int? DatasetID { get; set; }

        [StringLength(50)]
        public string DatasetName { get; set; }

        public int ScriptID { get; set; }

        [StringLength(200)]
        public string Tag { get; set; }

        public string RProcessOutput { get; set; }

        public string HtmlOutput { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DateAnalysed { get; set; }

        public Dataset Dataset { get; set; }

        public Script Script { get; set; }

        public ICollection<Argument> Arguments { get; set; }
    }
}