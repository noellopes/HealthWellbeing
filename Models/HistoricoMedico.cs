using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class HistoricoMedico
    {
        [Key]
        public int HistoricoMedicoId { get; set; }

        //FK
        [Required]
        public int UtenteBalnearioId { get; set; }
        [ForeignKey(nameof(UtenteBalnearioId))]
        public UtenteBalneario UtenteBalneario { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataRegisto { get; set; } = DateTime.Now;

        [Required]
        [StringLength(500)]
        public string Descricao { get; set; } = string.Empty;

        public string? CriadoPorUserId { get; set; }
        public IdentityUser? CriadoPorUser { get; set; }
    }
}
