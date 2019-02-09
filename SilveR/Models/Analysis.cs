using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SilveR.Models
{
    public partial class Analysis
    {
        private Analysis()
        {
        }

        public Analysis(Dataset dataset)
        {
            if (dataset != null)
            {
                this.Dataset = dataset;
                this.DatasetID = dataset.DatasetID;
                this.DatasetName = dataset.DatasetName;
            }

            AnalysisGuid = Guid.NewGuid().ToString();

            Arguments = new HashSet<Argument>();
        }

        [Key]
        public int AnalysisID { get; set; }

        [Required]
        [StringLength(128)]
        public string AnalysisGuid { get; private set; }

        public Nullable<int> DatasetID { get; set; }

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