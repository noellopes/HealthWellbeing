using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class HistoricoPontos
    {
        [Key]
        public int HistoricoPontosId { get; set; }

        [Required]
        public int ClienteBalnearioId { get; set; }
        public ClienteBalneario ClienteBalneario { get; set; }

        [Required]
        public int Pontos { get; set; }

        [Required]
        [StringLength(100)]
        public string Motivo { get; set; }

        public DateTime Data { get; set; } = DateTime.Now;
    }
}
