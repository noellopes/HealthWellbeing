using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class CrisisAlert
    {
        [Key]
        public int AlertId { get; set; }

        [Required(ErrorMessage = "Patient is required.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Alert date and time is required.")]
        [Display(Name = "Alert Date/Time")]
        public DateTime AlertDateTime { get; set; }

        [Display(Name = "Crisis Level")]
        public CrisisLevel Level { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(2000, ErrorMessage = "Description cannot exceed 2000 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; } = null!;

        [Display(Name = "Resolved")]
        public bool IsResolved { get; set; }

        [Display(Name = "Resolved Date/Time")]
        public DateTime? ResolvedDateTime { get; set; }

        [MaxLength(2000, ErrorMessage = "Resolution cannot exceed 2000 characters.")]
        [Display(Name = "Resolution")]
        public string? Resolution { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }
    }

    public enum CrisisLevel
    {
        Low,
        Medium,
        High,
        Critical
    }
}