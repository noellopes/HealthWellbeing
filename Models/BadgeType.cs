using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {
    public class BadgeType {
        public int BadgeTypeId { get; set; }

        [Display(Name = "Name")]
        [RegularExpression(@"^[a-zA-Z0-9À-ÿ\s\-_.!,?()&+:;""']+$",
            ErrorMessage = "Only letters, numbers, spaces, and common punctuation are allowed.")]
        [Required(ErrorMessage = "Please enter the badge type name.")]
        [StringLength(100, ErrorMessage = "Badge type name cannot exceed {1} characters.")]
        public required string BadgeTypeName { get; set; }

        [Display(Name = "Description")]
        [StringLength(300, ErrorMessage = "The description cannot exceed {1} characters.")]
        public string? BadgeTypeDescription { get; set; }

        public virtual ICollection<Badge>? Badges { get; set; }
    }
}
