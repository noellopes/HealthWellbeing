using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class SatisfacaoCliente
    {
        [Key]
        public int SatisfacaoClienteId { get; set; }

        // =========================
        // RELAÇÃO COM CLIENTE
        // =========================
        [Required]
        public int ClienteBalnearioId { get; set; }

        public ClienteBalneario ClienteBalneario { get; set; }

        // =========================
        // AVALIAÇÃO
        // =========================
        [Required]
        [Range(1, 5, ErrorMessage = "A avaliação deve ser entre 1 e 5.")]
        [Display(Name = "Avaliação")]
        public int Avaliacao { get; set; }

        [StringLength(500)]
        [Display(Name = "Comentário")]
        public string? Comentario { get; set; }

        // =========================
        // METADADOS
        // =========================
        [Display(Name = "Data")]
        public DateTime DataRegisto { get; set; } = DateTime.Now;
    }
}
