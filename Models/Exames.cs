using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthWellBeing.Models
{
    // Renomear para Exame (singular)
    public class Exame
    {
        public int ExameId { get; set; } 

        [Required(ErrorMessage = "A data e hora da marcação são obrigatórias.")]
        public DateTime DataHoraMarcacao { get; set; }

        
        [StringLength(100)]
        public string? Notas { get; set; } 

        public ExameTipo? ExameTipo { get; set; } // Propriedade de navegação
        public int ExameTipoId { get; set; } // Chave estrangeira
    }
}