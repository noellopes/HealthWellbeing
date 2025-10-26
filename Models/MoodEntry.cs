using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class MoodEntry
    {
        [Key]
        public int MoodEntryId { get; set; }

        [Required]
        public int ClientId { get; set; }

        [Required]
        public DateTime EntryDateTime { get; set; }

        [Range(1, 10)]
        public int MoodScore { get; set; }

        [MaxLength(100)]
        public string? Emotion { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(500)]
        public string? Triggers { get; set; }

        // Navigation properties
        [ForeignKey("ClientId")]
        public virtual Client? Client { get; set; }
    }
}