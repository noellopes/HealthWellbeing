using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Agendamento
    {
        public int AgendamentoId { get; set; }

        [Display(Name = "Data Início")]
        public DateTime DataHoraInicio { get; set; }

        [Display(Name = "Data Fim")]
        public DateTime DataHoraFim { get; set; }

        public string Estado { get; set; } = "Pendente";

        // Terapeuta
        [Display(Name = "Terapeuta")]
        public int TerapeutaId { get; set; }
        public Terapeuta Terapeuta { get; set; }
    }
}