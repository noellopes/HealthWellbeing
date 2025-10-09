using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Pathology
    {
        public int PathologyId { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "The name must have at least 3 chars and no more than 100 chars")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Severity is Required")]
        public string Severity { get; set; }

    }
}
