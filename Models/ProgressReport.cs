using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class ProgressReport
    {
        [Key]
        public int ReportId { get; set; }

        [Required]
        public int GoalId { get; set; }

        [Required]
        public DateTime ReportDate { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [Range(0, 100)]
        public int ProgressPercentage { get; set; }

        // Navigation properties
        [ForeignKey("GoalId")]
        public virtual Goal? Goal { get; set; }
    }
}