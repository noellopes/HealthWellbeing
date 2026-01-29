using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class HistoricoMedico
    {
        [Key]
        public int HistoricoMedicoId { get; set; }

        [Required]
        public int UtenteBalnearioId { get; set; }
        public UtenteBalneario UtenteBalneario { get; set; } = null!;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataRegisto { get; set; } = DateTime.Now;

        [Required]
        [StringLength(500)]
        public string Descricao { get; set; } = string.Empty;
    }
}
