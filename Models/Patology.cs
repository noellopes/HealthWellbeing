using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Patology
    {
        [Required(ErrorMessage = "ID is Required")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is Required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Severity is Required")]
        public string Severity { get; set; }

    }
}
