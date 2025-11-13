using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Specialities
    {
        [Key] // PK porque o nome foge à convenção "Id"
        public int IdEspecialidade { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(80, ErrorMessage = "Máximo 80 caracteres.")]
        public string Nome { get; set; } = "";

        [Required(ErrorMessage = "A descrição é obrigatória.")]
        [StringLength(1500, ErrorMessage = "Máximo 1500 caracteres.")]
        public string Descricao { get; set; } = "";
    }
}