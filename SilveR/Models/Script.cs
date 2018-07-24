using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SilveRModel.Models
{
    public partial class Script
    {
        public Script()
        {
            Analysis = new HashSet<Analysis>();
        }

        public int ScriptID { get; set; }

        [Required]
        [StringLength(50)]
        public string ScriptDisplayName { get; set; }

        [Required]
        [StringLength(50)]
        public string ScriptFileName { get; set; }

        public ICollection<Analysis> Analysis { get; set; }
    }
}