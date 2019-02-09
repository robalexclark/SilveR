using System.ComponentModel.DataAnnotations;

namespace SilveR.Models
{
    public partial class Argument
    {
        [Key]
        public int ArgumentID { get; set; }

        public int AnalysisID { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(100)]
        public string Value { get; set; }

        public Analysis Analysis { get; set; }
    }
}