using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class CrisisAlert
    {
        [Key]
        public int AlertId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime AlertDateTime { get; set; }

        public CrisisLevel Level { get; set; }

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; }

        public bool IsResolved { get; set; }

        public DateTime? ResolvedDateTime { get; set; }

        [MaxLength(2000)]
        public string? Resolution { get; set; }

        // Navigation properties
        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }
    }

    public enum CrisisLevel
    {
        Low,
        Medium,
        High,
        Critical
    }
}