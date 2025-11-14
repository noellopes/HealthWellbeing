using System.ComponentModel.DataAnnotations;
using System.Text.Json;

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

        public static string CreateAlert(string alertType, string iconClass, string message, bool? dismissible = null)
        {
            var alert = new AlertItem
            {
                AlertType = alertType,
                IconClass = iconClass,
                Message = message,
                Dismissible = dismissible
            };

            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(alert, new ValidationContext(alert), validationResults, true);

            // Return a generic alert if validation fails
            if (!isValid)
            {
                alert = new AlertItem
                {
                    AlertType = "danger",
                    IconClass = "bi bi-exclamation-triangle",
                    Message = "Invalid alert configuration",
                    Dismissible = true
                };
            }

            return JsonSerializer.Serialize(alert);
        }

        public AlertItem Clone()
        {
            return (AlertItem)this.MemberwiseClone();
        }
    }
}