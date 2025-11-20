using HealthWellBeing.Models;
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
        [Display(Name = "Descrição")]// Alteração do nome do cabeçalho na View
        public string Descricao { get; set; }


        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        // A validação de tamanho de string é REMOVIDA para o tipo INT.
        public int EspecialidadeId { get; set; } // FK
        public Especialidade? Especialidade { get; set; } // Propriedade de Navegação




    }

  
}

