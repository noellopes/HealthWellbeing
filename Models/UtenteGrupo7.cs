using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class UtenteGrupo7
    {
        public int UtenteGrupo7Id { get; set; }

        // Guarda o ID do utilizador logado (AspNetUsers)
        public string UserId { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [MaxLength(100)]
        public string Nome { get; set; }
        public string? ProfissionalSaudeId { get; set; }

        public ICollection<Sono>? Sonos { get; set; }


        public int? ObjetivoFisicoId { get; set; }
        public ObjetivoFisico? ObjetivoFisico { get; set; }

        public ICollection<UtenteGrupo7ProblemaSaude>? UtenteProblemasSaude { get; set; }
        public ICollection<HistoricoAtividade> HistoricoAtividades { get; set; }
        public ICollection<AvaliacaoFisica>? AvaliacaoFisicas { get; set; }
        public ICollection<PlanoExercicios>? PlanoExercicios { get; set; }
    }
}
