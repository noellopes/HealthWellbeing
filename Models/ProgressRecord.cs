using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace HealthWellbeing.Models
{
    public class ProgressRecord
    {
        public int ProgressRecordId { get; set; }

        // Link to Client
        [Required]
        public string ClientId { get; set; } = string.Empty;

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        // Link to Nutritionist (IdentityUser with Nutricionista role)
        [Required]
        [StringLength(450)]
        public string NutritionistId { get; set; } = string.Empty;

        [ForeignKey("NutritionistId")]
        public IdentityUser? Nutritionist { get; set; }

        // Health Metrics (nullable for extensibility)
        [Display(Name = "Peso (kg)")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? Weight { get; set; }

        [Display(Name = "Gordura Corporal (%)")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? BodyFatPercentage { get; set; }

        [Display(Name = "Colesterol (mg/dL)")]
        [Column(TypeName = "decimal(6,2)")]
        public decimal? Cholesterol { get; set; }

        [Display(Name = "IMC")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? BMI { get; set; }

        [Display(Name = "Massa Muscular (kg)")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? MuscleMass { get; set; }

        // Record info
        [Required]
        [Display(Name = "Data do Registo")]
        [DataType(DataType.Date)]
        public DateTime RecordDate { get; set; } = DateTime.Now;

        [Display(Name = "Notas")]
        [StringLength(1000)]
        public string? Notes { get; set; }

        // Audit fields
        [Display(Name = "Criado em")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string? CreatedBy { get; set; }
    }
}
