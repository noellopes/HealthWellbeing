using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ZonaArmazenamento
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(200)]
        public string? Descricao { get; set; }

        [Required]
        [StringLength(150)]
        public string Localizacao { get; set; }

        [Range(0, 10000)]
        public double CapacidadeMaxima { get; set; }

        public bool Ativa { get; set; } = true;
    }
}
