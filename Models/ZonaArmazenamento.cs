using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ZonaArmazenamento
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O Nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; }

        [StringLength(200, ErrorMessage = "A Descrição não pode ter mais de 200 caracteres.")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "A Localização é obrigatória.")]
        [StringLength(150, ErrorMessage = "A Localização não pode ter mais de 150 caracteres.")]
        public string Localizacao { get; set; }

        [Range(0, 10000, ErrorMessage = "A Capacidade Máxima deve estar entre 0 e 10000.")]
        public double CapacidadeMaxima { get; set; }

        public bool Ativa { get; set; } = true;
    }
}
