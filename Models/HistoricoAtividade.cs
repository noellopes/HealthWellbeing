using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class HistoricoAtividade
    {
        public int Id { get; set; }

        // ===== UTENTE =====
        [Required(ErrorMessage = "O utente é obrigatório.")]
        public string UtenteGrupo7Id { get; set; }

        public UtenteGrupo7 UtenteGrupo7 { get; set; }

        // ===== EXERCÍCIO =====
        [Required(ErrorMessage = "O exercício é obrigatório.")]
        public int ExercicioId { get; set; }

        public Exercicio Exercicio { get; set; }

        // ===== PLANO DE EXERCÍCIOS =====
        public int? PlanoExerciciosId { get; set; }

        public PlanoExercicios PlanoExercicios { get; set; }

        // ===== DATA E HORA =====
        [Required(ErrorMessage = "A data e hora são obrigatórias.")]
        public DateTime DataHora { get; set; }
    }
}
