using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {

    public class EventType {
        public int EventTypeId { get; set; }
        [Required(ErrorMessage = "Please enter the event type name.")]
        [StringLength(100, ErrorMessage = "Event type name cannot exceed {1} characters.")]
        [RegularExpression(@"^[a-zA-Z0-9À-ÿ\s\-_.!,?()&+:;""']+$",
            ErrorMessage = "Only letters, numbers, spaces, and common punctuation are allowed.")]
        public string EventTypeName { get; set; }

        [Required(ErrorMessage = "Please select a scoring mode.")]
        [StringLength(50, ErrorMessage = "Scoring Mode cannot exceed {1} characters.")]
        public string EventTypeScoringMode { get; set; }

        [Display(Name = "Multiplier")]
        [Required(ErrorMessage = "Please enter a multiplier.")]
        [Range(1, 3, ErrorMessage = "Multiplier must be between {1} and {2}.")]
        [RegularExpression(@"^[0-9]{1,3}([.,][0-9]{1,2})?$",
        ErrorMessage = "Enter a number with up to 3 digits, optionally followed by a comma or dot and 2 decimal (e.g., 1.15 or 1,15).")]
        public decimal EventTypeMultiplier { get; set; }


    }
}
