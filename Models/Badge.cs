using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {
    public class Badge {
        public int BadgeId { get; set; }

        [Display(Name = "Badge Type")]
        [Required(ErrorMessage = "Please select a badge type.")]
        public int BadgeTypeId { get; set; }
        public virtual BadgeType? BadgeType { get; set; }

        [Display(Name = "Name")]
        [StringLength(100, ErrorMessage = "Badge name cannot exceed {1} characters.")]
        [RegularExpression(@"^[a-zA-Z0-9À-ÿ\s\-_.!,?()&+:;""']+$",
            ErrorMessage = "Only letters, numbers, spaces, and common punctuation are allowed.")]
        [Required(ErrorMessage = "Please enter the badge name.")]
        public required string BadgeName { get; set; }

        [Display(Name = "Description")]
        [StringLength(300, ErrorMessage = "Badge description cannot exceed {1} characters.")]
        public string? BadgeDescription { get; set; }

        [Display(Name = "Reward Points")]
        [Range(1, 1000, ErrorMessage = "Reward points must be between {1} and {2}.")]
        public int RewardPoints { get; set; }

        [Display(Name = "Activity Status")]
        public bool IsActive { get; set; } = true;

        public virtual ICollection<BadgeRequirement>? BadgeRequirements { get; set; }
        public virtual ICollection<CustomerBadge>? CustomerBadges { get; set; }

    }
}
