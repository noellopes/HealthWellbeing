using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ExameTipo
    {
        public int ExameTipoId { get; set; }

        [Required(ErrorMessage = "O nome do exame é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; } 

        [StringLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres.")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        [StringLength(100)]
        public string Especialidade { get; set; }
    }

  
}

