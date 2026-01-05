using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class HistoricoAtividade
    {
        public int HistoricoAtividadeId { get; set; }

        [Required]
        public DateTime DataRealizacao { get; set; } = DateTime.Now;

        public int ExercicioId { get; set; }
        public Exercicio Exercicio { get; set; }

        public int UtenteGrupo7Id { get; set; }
        public UtenteGrupo7 UtenteGrupo7 { get; set; }

        public int? PlanoExerciciosId { get; set; }
    }
}
