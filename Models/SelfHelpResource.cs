using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class SelfHelpResource
    {
        [Key]
        public int ResourceId { get; set; }

        public ResourceType Type { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        [MaxLength(5000)]
        public string? Content { get; set; }

        [MaxLength(500)]
        [Url]
        public string? Url { get; set; }

        public int DurationMinutes { get; set; }

        [MaxLength(100)]
        public string? Category { get; set; }

        public bool IsActive { get; set; }
    }

    public enum ResourceType
    {
        Article,
        Exercise,
        Meditation,
        Video,
        Audio
    }
}