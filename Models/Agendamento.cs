using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Agendamento
    {
        public int AgendamentoId { get; set; }

        public DateTime DataHoraInicio { get; set; }
        public DateTime DataHoraFim { get; set; }

        public string Estado { get; set; } = "Pendente";

        // Terapeuta
        public int TerapeutaId { get; set; }
        public Terapeuta Terapeuta { get; set; }

        // Serviço
        public int ServicoId { get; set; }
    }
}