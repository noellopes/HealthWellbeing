using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {
    public class ScoringStrategy {
        public int ScoringStrategyId { get; set; }

        [Display(Name = "Name")]
        [Required(ErrorMessage = "Please enter the scoring strategy name.")]
        [StringLength(100, ErrorMessage = "Scoring strategy name cannot exceed {1} characters.")]
        [RegularExpression(@"^[a-zA-Z0-9À-ÿ\s\-_.!,?()&+:;""']+$",
            ErrorMessage = "Only letters, numbers, spaces, and common punctuation are allowed.")]
        public required string ScoringStrategyName { get; set; }

        [Display(Name = "Technical Code")]
        [Required(ErrorMessage = "Please enter the technical code for the scoring strategy.")]
        [StringLength(50, ErrorMessage = "Technical code cannot exceed {1} characters.")]
        [RegularExpression(@"^[A-Z_]+$", ErrorMessage = "Use only uppercase letters and underscores (e.g. TIME_BASED).")]
        public required string ScoringStrategyCode { get; set; }

        [Display(Name = "Description")]
        [StringLength(300, ErrorMessage = "Scoring strategy description cannot exceed {1} characters.")]
        public string? ScoringStrategyDescription { get; set; }
        public virtual ICollection<EventType>? EventTypes { get; set; }
    }
}
