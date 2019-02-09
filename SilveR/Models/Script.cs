using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SilveR.Models
{
    public partial class Script
    {
        public Script()
        {
            Analysis = new HashSet<Analysis>();
        }

        [Key]
        public int ScriptID { get; set; }

        [Required]
        [StringLength(100)]
        public string ScriptDisplayName { get; set; }

        [Required]
        [StringLength(50)]
        public string ScriptFileName { get; set; }

        public ICollection<Analysis> Analysis { get; set; }

        public bool RequiresDataset { get; set; }
    }
}