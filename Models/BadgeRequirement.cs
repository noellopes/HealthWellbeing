using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {
    public class BadgeRequirement {
        public int BadgeRequirementId { get; set; }

        [Display(Name = "Badge")]
        public int BadgeId { get; set; }
        public virtual Badge? Badge { get; set; }

        [Display(Name = "Badge Requirement")]
        [RegularExpression(@"^[a-zA-Z0-9À-ÿ\s\-_.!,?()&+:;""']+$",
            ErrorMessage = "Only letters, numbers, spaces, and common punctuation are allowed.")]
        [Required(ErrorMessage = "Please enter the badge requirement.")]
        [StringLength(100, ErrorMessage = "Badge requirement cannot exceed {1} characters.")]
        public required string BadgeRequirementName { get; set; }

        [Display(Name = "Requirement Description")]
        [StringLength(300, ErrorMessage = "The requirement description cannot exceed {1} characters.")]
        public string? RequirementDescription { get; set; }

        [Display(Name = "Target Value")]
        [Range(1, 100, ErrorMessage = "Target value must be between {1} and {2}.")]
        public int TargetValue { get; set; }

        public RequirementType RequirementType { get; set; }

        [Display(Name = "Event Type")]
        public int EventTypeId { get; set; }
        public virtual EventType? EventTypes { get; set; }

        [Display(Name = "Activity Type")]
        public int ActivityTypeId { get; set; }
        public virtual ActivityType? ActivityTypes { get; set;

        }
}
