using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Session
    {
        [Key]
        public int SessionId { get; set; }

        [ForeignKey("Member")]
        public int MemberId { get; set; }
        public Member Member { get; set; }

        [ForeignKey("Training")]
        public int TrainingId { get; set; }
        public Training Training { get; set; }

        [Required]
        [Display(Name = "Session Date")]
        public DateTime SessionDate { get; set; }

        // Adicionado para bater certo com o Diagrama de Classes e as Views
        [Display(Name = "Feedback")]
        public string? MemberFeedback { get; set; }

        [Range(1, 5)]
        public int? Rating { get; set; }
    }
}