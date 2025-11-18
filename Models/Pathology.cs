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
        public required PathologySeverityLevel Severity { get; set; } = PathologySeverityLevel.NotDesignated;

        public enum PathologySeverityLevel
        {
            [Display(Name = "Não designada")]
            NotDesignated,

            [Display(Name = "Ligeira")]
            Mild,

            [Display(Name = "Moderada")]
            Moderate,

            [Display(Name = "Grave")]
            Severe
        }

    }
}