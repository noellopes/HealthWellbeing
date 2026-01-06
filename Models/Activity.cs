using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models {
    public class Activity {
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "Activity Name is required.")]
        [Display(Name = "Activity Name")]
        public string ActivityName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string ActivityDescription { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000, ErrorMessage = "Reward must be between 1 and 1000 points.")]
        [Display(Name = "Reward Points")]
        public int ActivityReward { get; set; }

        [Display(Name = "Activity Type")]
        public int ActivityTypeId { get; set; }

        [ForeignKey("ActivityTypeId")]
        public virtual ActivityType? ActivityType { get; set; }

        public virtual ICollection<EventActivity> EventActivities { get; set; } = new List<EventActivity>();
        public virtual ICollection<CustomerActivity> CustomerActivities { get; set; } = new List<CustomerActivity>();
    }
}