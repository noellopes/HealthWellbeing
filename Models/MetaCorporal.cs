using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class MetaCorporal
    {
        public int MetaCorporalId { get; set; }

        // Link to Client
        [Required]
        public string ClientId { get; set; } = string.Empty;

        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        // Goal Metrics (nullable for flexibility)
        [Display(Name = "Peso Objetivo (kg)")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? PesoObjetivo { get; set; }

        [Display(Name = "Gordura Corporal Objetivo (%)")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? GorduraCorporalObjetivo { get; set; }

        [Display(Name = "Colesterol Objetivo (mg/dL)")]
        [Column(TypeName = "decimal(6,2)")]
        public decimal? ColesterolObjetivo { get; set; }

        [Display(Name = "IMC Objetivo")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? IMCObjetivo { get; set; }

        [Display(Name = "Massa Muscular Objetivo (kg)")]
        [Column(TypeName = "decimal(5,2)")]
        public decimal? MassaMuscularObjetivo { get; set; }

        // Timeline
        [Display(Name = "Data de Início")]
        [DataType(DataType.Date)]
        public DateTime DataInicio { get; set; } = DateTime.Now;

        [Display(Name = "Data Objetivo")]
        [DataType(DataType.Date)]
        public DateTime? DataObjetivo { get; set; }

        [Display(Name = "Notas")]
        [StringLength(1000)]
        public string? Notas { get; set; }

        // Audit fields
        [Display(Name = "Criado em")]
        public DateTime CriadoEm { get; set; } = DateTime.Now;

        [StringLength(450)]
        public string? CriadoPor { get; set; }

        [Display(Name = "Atualizado em")]
        public DateTime? AtualizadoEm { get; set; }

        [StringLength(450)]
        public string? AtualizadoPor { get; set; }

        // Status
        [Display(Name = "Ativo")]
        public bool Ativo { get; set; } = true;
    }
}
