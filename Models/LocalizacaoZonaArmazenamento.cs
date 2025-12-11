using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class LocalizacaoZonaArmazenamento
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome da localização é obrigatório.")]
        [StringLength(150, ErrorMessage = "O nome da localização não pode ter mais de 150 caracteres.")]
        public string Nome { get; set; }
    }
}
