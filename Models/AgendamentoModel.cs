using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class AgendamentoModel
    {
        [Key]
        public int AgendamentoId { get; set; }

        [Required]
        public DateTime DataHoraInicio { get; set; }

        [Required]
        public DateTime DataHoraFim { get; set; }

        [Required]
        [StringLength(30)]
        public string Estado { get; set; } = "Pendente";

        // Relações
        [Required]
        public int TerapeutaId { get; set; }
        public Terapeuta Terapeuta { get; set; }

        [Required]
        public int UtenteBalnearioId { get; set; }
        public UtenteBalneario UtenteBalneario { get; set; }

        [Required]
        public int ServicoId { get; set; }
        public Servico Servico { get; set; }
    }
}