using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Goal
    {
        [Key]
        public int GoalId { get; set; }

        [Required(ErrorMessage = "Patient is required.")]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Goal title is required.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        [Display(Name = "Goal Title")]
        public string Title { get; set; } = null!;

        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        [Display(Name = "Description")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Display(Name = "Target Date")]
        [DataType(DataType.Date)]
        public DateTime? TargetDate { get; set; }

        [Display(Name = "Status")]
        public GoalStatus Status { get; set; }

        [Range(0, 100, ErrorMessage = "Progress must be between 0 and 100.")]
        [Display(Name = "Progress (%)")]
        public int ProgressPercentage { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        public virtual ICollection<ProgressReport>? ProgressReports { get; set; }
    }

    public enum GoalStatus
    {
        Active,
        Completed,
        Paused,
        Abandoned
    }
}