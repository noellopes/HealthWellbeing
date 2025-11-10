using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ZonaArmazenamento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; }

        [StringLength(200, ErrorMessage = "A descrição não pode ter mais de 200 caracteres.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A localização é obrigatória.")]
        [StringLength(150, ErrorMessage = "A localização não pode ter mais de 150 caracteres.")]
        public string Localizacao { get; set; }

        [Range(0, 10000, ErrorMessage = "A capacidade máxima deve estar entre 0 e 10.000.")]
        [Display(Name = "Capacidade Máxima")]
        public double CapacidadeMaxima { get; set; }

        [Display(Name = "Zona Ativa")]
        public bool Ativa { get; set; } = true;
    }
}
