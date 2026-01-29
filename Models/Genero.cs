using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Genero
    {
        public int GeneroId { get; set; }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; } = string.Empty;
    }


}
