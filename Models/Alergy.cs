using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Alergy
    {
        [Key]
        public int AlergyId { get; set; }

        [Required(ErrorMessage = "Allergy name is required.")]
        [StringLength(80, ErrorMessage = "Allergy name cannot exceed 80 characters.")]
        public string AlergyName { get; set; } = string.Empty;

        public ICollection<ClientAlergy>? ClientAlergies { get; set; }
    }
}
