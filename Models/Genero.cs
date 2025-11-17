using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Genero
    {
        public int GeneroId { get; set; }

        [Required(ErrorMessage = "O nome do gênero é obrigatório.")]
        [StringLength(50, ErrorMessage = "O nome do gênero não pode exceder 50 caracteres.")]
        public string NomeGenero { get; set; }

        public ICollection<Exercicio>? Exercicio { get; set; }
    }
}
