using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {
    public class ActivityType {
        public int ActivityTypeId { get; set; }

        [Required(ErrorMessage = "Type Name is required.")]
        [Display(Name = "Type Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(200, ErrorMessage = "Description cannot exceed 200 characters.")]
        public string Description { get; set; } = string.Empty;

        public virtual ICollection<Activity> Activities { get; set; } = new List<Activity>();
    }
}