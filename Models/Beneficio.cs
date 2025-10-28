using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Beneficio
    {
        public int BeneficioId { get; set; }

        [Required(ErrorMessage = "O nome do benefício é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do benefício deve ter no máximo 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A descrição do benefício é obrigatória.")]
        [StringLength(300, ErrorMessage = "A descrição deve ter no máximo 300 caracteres.")]
        public string Descricao { get; set; }


        public bool Ativo { get; set; } = true;

        // Relacionamento N:N — um benefício pode estar ligado a vários tipos de exercício
        public ICollection<TipoExercicio>? TiposExercicio { get; set; }


    }
}
