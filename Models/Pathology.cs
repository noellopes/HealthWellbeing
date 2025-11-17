using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Pathology
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is Required")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "The name must have at least 3 chars and no more than 100 chars")]
        [Display(Name = "Nome")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is Required")]
        [Display(Name = "Descrição")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Severity is Required")]
        [Display(Name = "Gravidade")]
        public required string Severity { get; set; }

    }
}