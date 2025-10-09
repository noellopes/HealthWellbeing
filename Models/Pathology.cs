using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Pathology
    {
        [Required(ErrorMessage = "ID is Required")]
        public int PathologyId { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Severity is Required")]
        public string Severity { get; set; }

    }
}
