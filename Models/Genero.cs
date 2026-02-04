using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Genero
    {
        public int GeneroId { get; set; }

        [Required(ErrorMessage = "O nome do género é obrigatório.")]
        [StringLength(50)]
        public required string NomeGenero { get; set; }

        public ICollection<Exercicio>? Exercicio { get; set; }
    }
}