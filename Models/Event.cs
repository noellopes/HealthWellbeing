using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Event : IValidatableObject
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "Event Name is required.")]
        [StringLength(100, ErrorMessage = "Event Name cannot exceed 100 characters.")]
        [Display(Name = "Event Name")]
        [RegularExpression(@"^[A-ZÀ-Ÿ][a-zA-Z0-9À-ÿ\s]*$", ErrorMessage = "Name must start with a capital letter and contain only letters and numbers.")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event Description is required.")]
        [StringLength(500, ErrorMessage = "Event Description cannot exceed 500 characters.")]
        [Display(Name = "Event Description")]
        [RegularExpression(@"^[a-zA-Z0-9À-ÿ\s.,!?:;\-']*$", ErrorMessage = "Description contains invalid characters (letters, numbers and basic punctuation allowed).")]
        public string EventDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "Event Type is required.")]
        [Display(Name = "Event Category")]
        public int EventTypeId { get; set; }

        public virtual EventType? EventType { get; set; }

        [Display(Name = "Activity Focus")]
        public int? ActivityTypeId { get; set; }

        [ForeignKey("ActivityTypeId")]
        public virtual ActivityType? ActivityType { get; set; }

        public virtual ICollection<EventActivity> EventActivities { get; set; } = new List<EventActivity>();

        [Required(ErrorMessage = "Start Date is required.")]
        [Display(Name = "Event Start")]
        [DataType(DataType.DateTime)]
        public DateTime EventStart { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [Display(Name = "Event End")]
        [DataType(DataType.DateTime)]
        public DateTime EventEnd { get; set; }

        [Display(Name = "Points")]
        public int EventPoints { get; set; }

        [Required(ErrorMessage = "Minimum Level is required.")]
        [Display(Name = "Minimum Level")]
        [Range(1, 100, ErrorMessage = "Level must be between 1 and 100.")]
        public int MinLevel { get; set; }
        public virtual ICollection<CustomerActivity>? CustomerActivities { get; set; }

        [NotMapped]
        [Display(Name = "Status")]
        public EventStatus Status
        {
            get
            {
                var now = DateTime.Now;
                if (now > EventEnd) return EventStatus.Realizado;
                if (now >= EventStart && now <= EventEnd) return EventStatus.Adecorrer;
                return EventStatus.Agendado;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EventEnd <= EventStart)
            {
                yield return new ValidationResult(
                    "End date must be later than start date.",
                    new[] { nameof(EventEnd) }
                );
            }

            if (EventId == 0 && EventStart < DateTime.Now)
            {
                yield return new ValidationResult(
                    "For new events, start date cannot be in the past.",
                    new[] { nameof(EventStart) }
                );
            }

            if (EventStart > DateTime.Now.AddYears(2))
            {
                yield return new ValidationResult(
                    "Events cannot be scheduled more than 2 years in advance.",
                    new[] { nameof(EventStart) }
                );
            }

            if ((EventEnd - EventStart).TotalMinutes < 15)
            {
                yield return new ValidationResult(
                    "The event must last at least 15 minutes.",
                    new[] { nameof(EventEnd) }
                );
            }

            if (EventStart.Hour < 6 || EventStart.Hour >= 23)
            {
                yield return new ValidationResult(
                    "The event must occur during operating hours (06:00 - 23:00).",
                    new[] { nameof(EventStart) }
                );
            }
        }
    }
}