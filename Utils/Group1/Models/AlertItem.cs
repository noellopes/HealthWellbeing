using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Utils.Group1.Models
{
    public class AlertItem : IValidatableObject
    {
        [Required]
        public required string AlertType { get; set; }

        [Required]
        public required string IconClass { get; set; }

        [Required]
        public required string Message { get; set; }

        public bool? Dismissible { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var validTypes = new[] { "primary", "secondary", "success", "danger", "warning", "info", "light", "dark" };
            if (!validTypes.Contains(AlertType?.ToLower()))
            {
                yield return new ValidationResult("Invalid alert type.", new[] { nameof(AlertType) });
            }
        }
    }
}