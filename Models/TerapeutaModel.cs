using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TerapeutaModel
    {
        [Key]
        public int TerapeutaId { get; set; }
        public string Nome { get; set; }
        public string Especialidade { get; set; } // Ex: Massagem, Fisioterapia, Estética, etc.
        public string Telefone { get; set; }
        public string Email { get; set; }
        public int AnosExperiencia { get; set; }
        public bool Ativo { get; set; }

        // Relação: um terapeuta pode ter vários agendamentos
        public ICollection<AgendamentoModel> Agendamentos { get; set; }
    }
}

