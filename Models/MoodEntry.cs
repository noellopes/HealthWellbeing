using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class MoodEntry
    {
        [Key]
        public int MoodEntryId { get; set; }

        [Required(ErrorMessage = "Patient is required.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Entry date and time is required.")]
        [Display(Name = "Entry Date/Time")]
        public DateTime EntryDateTime { get; set; }

        [Required(ErrorMessage = "Mood score is required.")]
        [Range(1, 10, ErrorMessage = "Mood score must be between 1 and 10.")]
        [Display(Name = "Mood Score (1-10)")]
        public int MoodScore { get; set; }

        [MaxLength(100, ErrorMessage = "Emotion cannot exceed 100 characters.")]
        [Display(Name = "Emotion")]
        public string? Emotion { get; set; }

        [MaxLength(2000, ErrorMessage = "Notes cannot exceed 2000 characters.")]
        [Display(Name = "Notes")]
        public string? Notes { get; set; }

        [MaxLength(500, ErrorMessage = "Triggers cannot exceed 500 characters.")]
        [Display(Name = "Triggers")]
        public string? Triggers { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
    }
}