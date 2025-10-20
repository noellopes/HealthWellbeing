using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ServicoModel
    {
        [Key]
        public int ServicoId { get; set; }
        public string Nome { get; set; } // Ex: "Massagem Relaxante", "Banho de Ervas", etc.
        public string Descricao { get; set; }
        public decimal Preco { get; set; }
        public int DuracaoMinutos { get; set; } // Duração média do serviço
        public string Tipo { get; set; } // Ex: "Massagem", "Banho", "Tratamento", "Fisioterapia", etc.

        // Relação: um serviço pode estar associado a vários agendamentos
        public ICollection<AgendamentoModel> Agendamentos { get; set; }
    }
}

