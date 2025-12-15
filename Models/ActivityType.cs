using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ActivityType
    {
        public int ActivityTypeId { get; set; }

        [Required]
        public string ActivityTypeName { get; set; } = string.Empty;


        [Required]
        public string ActivityDescription { get; set; } = string.Empty;

        public virtual ICollection <Activity>? Activity { get; set; }
        public virtual ICollection<BadgeRequirement>? BadgeRequirements { get; set; }
    }
}
