using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Alergia
    {
        [Key]
        public int AlergiaID { get; set; }

        [Required(ErrorMessage = "O nome da alergia é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo 100 caracteres.")]
        [Display(Name = "Nome da Alergia")]
        public string Nome { get; set; }

        [Display(Name = "Descrição")]
        public string Descricao { get; set; }

        [Required(ErrorMessage = "A gravidade é obrigatória.")]
        [StringLength(20, ErrorMessage = "A gravidade deve ter no máximo 20 caracteres.")]
        [Display(Name = "Gravidade")]
        public string Gravidade { get; set; } // -> quando implementar a classe alterar para ela, por enquanto mantem como string.

        [Required(ErrorMessage = "Os sintomas são obrigatórios.")]
        [StringLength(300, ErrorMessage = "Os sintomas devem ter no máximo 300 caracteres.")]
        [Display(Name = "Sintomas")]
        public string Sintomas { get; set; }

        [Display(Name = "Alimento Associado")]
        public int? FoodId { get; set; }
        public Food? Food { get; set; }
    }
}
