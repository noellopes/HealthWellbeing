using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class SeguroSaude
    {
        [Key]
        public int SeguroSaudeId { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; } = string.Empty;
    }
}
