using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }

        [Required(ErrorMessage = "Activity Name is required.")]
        public string ActivityName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Activity Description is required.")]
        public string ActivityDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Reward points are required.")]
        public int ActivityReward { get; set; }

        [Display(Name = "Activity Type")]
        public int ActivityTypeId { get; set; }

        [ForeignKey("ActivityTypeId")]
        public virtual ActivityType? ActivityType { get; set; }

        public virtual ICollection<EventActivity>? EventActivities { get; set; }
        public virtual ICollection<CustomerActivity>? CustomerActivities { get; set; }

        public void obterRecompensas()
        {

        }
    }
}