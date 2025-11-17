using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Beneficio
    {
        public int BeneficioId { get; set; }

        [Required(ErrorMessage = "O nome do benefício é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome deve ter no máximo 50 caracteres.")]
        public string NomeBeneficio { get; set; }

        [Required(ErrorMessage = "A descrição do benefício é obrigatória.")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string DescricaoBeneficio { get; set; }

        // Relacionamento N:N — um benefício pode estar ligado a vários tipos de exercício
        public ICollection<TipoExercicio>? TipoExercicio { get; set; }
    }

}

