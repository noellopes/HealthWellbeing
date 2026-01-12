using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Session
    {
        [Key]
        public int SessionId { get; set; }

        // RELAÇÃO COM O TREINO (Qual é a aula?)
        [ForeignKey("Training")]
        public int TrainingId { get; set; }
        public Training Training { get; set; }

        // RELAÇÃO COM O MEMBRO (Quem vai?)
        [ForeignKey("Member")]
        public int MemberId { get; set; }
        public Member Member { get; set; }

        // DADOS DO AGENDAMENTO
        [Required]
        [Display(Name = "Session Date")]
        public DateTime SessionDate { get; set; }

        // RATING (Avaliação posterior, opcional para já)
        [Range(1, 5)]
        public int? Rating { get; set; }
    }
}