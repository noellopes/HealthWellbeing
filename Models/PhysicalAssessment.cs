using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class PhysicalAssessment
    {
        public int PhysicalAssessmentId { get; set; }

        [Required(ErrorMessage = "Assessment date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Assessment Date")]
        public DateTime AssessmentDate { get; set; }

        [Required(ErrorMessage = "Weight is required.")]
        [Range(20, 300, ErrorMessage = "Please enter a valid weight (20-300kg).")]
        [Display(Name = "Weight (kg)")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Height is required.")]
        [Range(1, 2.5, ErrorMessage = "Please enter a valid height (1.00-2.50m).")]
        [Display(Name = "Height (m)")]
        public decimal Height { get; set; }

        [Display(Name = "Body Fat %")]
        [Range(1, 70)]
        public decimal BodyFatPercentage { get; set; }

        [Display(Name = "Muscle Mass (kg)")]
        public decimal MuscleMass { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Foreign Keys

        [Required(ErrorMessage = "Member is required.")]
        [Display(Name = "Member")]
        public int MemberId { get; set; }
        public Member? Member { get; set; }

        [Required(ErrorMessage = "Trainer is required.")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }

        // Campo calculado para o IMC
        [Display(Name = "BMI")]
        public decimal BMI
        {
            get
            {
                if (Height > 0)
                {
                    return Weight / (Height * Height);
                }
                return 0;
            }
        }
    }
}