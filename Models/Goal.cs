using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Goal
    {
        [Key]
        public int GoalId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? TargetDate { get; set; }

        public GoalStatus Status { get; set; }

        [Range(0, 100)]
        public int ProgressPercentage { get; set; }

        // Navigation properties
        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }

        public virtual ICollection<ProgressReport>? ProgressReports { get; set; }
    }

    public enum GoalStatus
    {
        Active,
        Completed,
        Paused,
        Abandoned
    }
}